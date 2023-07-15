using CommunityToolkit.Labs.WinUI;

namespace TvTime.ViewModels;
public partial class MediaViewModel : BaseViewModel
{
    [ObservableProperty]
    public ObservableCollection<TokenItem> tokenList;

    [ObservableProperty]
    public int infoBadgeValue;

    [ObservableProperty]
    public bool isServerStatusOpen;

    private PageOrDirectoryType PageType;
    private int totalServerCount = 0;

    JsonSerializerOptions options = new() { WriteIndented = true };

    #region Override Methods
    public override void OnPageLoaded(object param)
    {
        PageType = MediaUserControl.Instance.PageType;

        var tokens = Settings.TVTimeServers.Where(x => x.IsActive && x.ServerType.ToString().Equals(GetPageType()))
            .Select(x => new TokenItem { Content = GetServerUrlWithoutLeftAndRightPart(x.Server) });

        TokenList = new(tokens);
        TokenList.Insert(0, new TokenItem { Content = Constants.ALL_FILTER, IsSelected = true });

        if (ExistDirectory(PageType))
        {
            LoadLocalStorage();
        }
        else
        {
            DownloadServersOnLocalStorage();
        }

        if (Settings.TVTimeServers.Count == 0)
        {
            IsStatusOpen = true;
            StatusTitle = "Server not found";
            StatusMessage = "No servers found, please add some servers first";
            StatusSeverity = InfoBarSeverity.Warning;
            IsServerStatusOpen = false;
            GoToServerPage();
        }
    }

    public override void OnRefresh()
    {
        DeleteDirectory(PageType);
        DownloadServersOnLocalStorage();
    }

    public override void NavigateToDetails(object sender)
    {
        base.NavigateToDetails(sender);

        var media = new MediaItem
        {
            Server = descriptionText,
            Title = headerText,
            ServerType = ApplicationHelper.GetEnum<ServerType>(PageType.ToString())
        };

        App.Current.NavigationManager.NavigateForJson(typeof(DetailPage), media);
    }

    public override void Search(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (DataList != null && DataList.Any())
        {
            var items = MediaUserControl.Instance.GetTokenSelectedItems();

            if (items.Any(token => token.Content.ToString().Equals(Constants.ALL_FILTER)))
            {
                AutoSuggestBoxHelper.LoadSuggestions(sender, args, DataList.Select(x => x.Title).ToList());
            }
            else
            {
                var filteredList = DataList.Where(x => items.Any(token => x.Server.Contains(token.Content.ToString()))).Select(x => x.Title).ToList();
                AutoSuggestBoxHelper.LoadSuggestions(sender, args, filteredList);
            }
            DataListACV.Filter = _ => true;
            DataListACV.Filter = DataListFilter;
        }
    }

    public override bool DataListFilter(object item)
    {
        var query = (MediaItem) item;
        var name = query.Title ?? "";
        var server = query.Server ?? "";
        var txtSearch = MainPage.Instance.GetTxtSearch();
        var items = MediaUserControl.Instance.GetTokenSelectedItems();

        if (items.Any(token => token.Content.ToString().Equals(Constants.ALL_FILTER)))
        {
            return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase) ||
                server.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            if (!name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase) &&
                !server.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return items.Any(token => server.Contains(token.Content.ToString()));
        }
    }

    #endregion

    [RelayCommand]
    private void OnServerStatus()
    {
        ContentDialog contentDialog = new ContentDialog();
        contentDialog.XamlRoot = App.currentWindow.Content.XamlRoot;
        contentDialog.Title = $"Server Status - {existServer.Count} Server(s) Added";
        var stck = new StackPanel
        {
            Spacing = 10,
            Margin = new Thickness(10)
        };

        foreach (var item in existServer)
        {
            var infoBar = new InfoBar();
            infoBar.Severity = InfoBarSeverity.Success;
            infoBar.Title = item;
            infoBar.IsOpen = true;
            infoBar.IsClosable = false;
            stck.Children.Add(infoBar);
        }

        contentDialog.Content = new ScrollViewer { Content = stck };

        contentDialog.PrimaryButtonText = "OK";
        contentDialog.ShowAsyncQueue();
    }

    private void GoToServerPage()
    {
        ContentDialog contentDialog = new ContentDialog();
        contentDialog.XamlRoot = App.currentWindow.Content.XamlRoot;
        contentDialog.Title = $"Add New Servers";
        var stck = new StackPanel
        {
            Spacing = 10,
            Margin = new Thickness(10)
        };

        var infoBar = new InfoBar();
        infoBar.Severity = InfoBarSeverity.Warning;
        infoBar.Title = "No servers found, please add some servers first";
        infoBar.Message = "Would you like to go to the servers page and add some servers?";
        infoBar.IsOpen = true;
        infoBar.IsClosable = false;
        stck.Children.Add(infoBar);

        contentDialog.Content = new ScrollViewer { Content = stck };
        contentDialog.PrimaryButtonText = "Go To Servers";
        contentDialog.SecondaryButtonText = "Cancel";
        contentDialog.PrimaryButtonClick += (s, e) =>
        {
            App.Current.NavigationManager.NavigateForJson(typeof(ServersPage));
        };

        contentDialog.ShowAsyncQueue();
    }

    private async void LoadLocalStorage()
    {
        IsActive = true;
        IsStatusOpen = true;
        StatusSeverity = InfoBarSeverity.Informational;
        StatusTitle = $"Loading Local {PageType}...";
        var files = Directory.GetFiles(Path.Combine(Constants.ServerDirectoryPath, GetPageType()), "*.txt");
        if (files.Any())
        {
            var tasks = files.Select(async file =>
            {
                using FileStream openStream = File.OpenRead(file);
                return await System.Text.Json.JsonSerializer.DeserializeAsync<List<MediaItem>>(openStream, options);
            });
            var contentsList = await Task.WhenAll(tasks);
            var contents = contentsList.SelectMany(x => x);

            if (PageType == PageOrDirectoryType.Series)
            {
                // Find the items containing "Iranian/Series" and extract the base URL, and Remove duplicates from the filtered list
                var filteredContents = contents.Where(c => c.Server.Contains("Series/Iranian"))
                               .Select(c => new MediaItem
                               {
                                   Server = GetBaseUrl(c.Server),
                                   Title = GetBaseTitle(c.Title),
                                   FileSize = c.FileSize,
                                   DateTime = c.DateTime,
                                   ServerType = c.ServerType
                               }).DistinctBy(x => x.Server);

                // Merge the unique and non-filtered items

                var finalContents = contents.Where(c => !c.Server.Contains("Series/Iranian"))
                            .Concat(filteredContents.Select(u => new MediaItem
                            {
                                Server = u.Server,
                                Title = u.Title,
                                FileSize = u.FileSize,
                                DateTime = u.DateTime,
                                ServerType = u.ServerType
                            }));

                // Update the MediaItem list
                var updatedList = finalContents.Select(c => new MediaItem
                {
                    Server = c.Server,
                    Title = c.Title,
                    FileSize = c.FileSize,
                    DateTime = c.DateTime,
                    ServerType = ServerType.Series
                }).ToList();
                contents = updatedList;
            }
            var myDataList = contents.Cast<ITvTimeModel>().Where(x => x.Server != null);
            DataList = new();
            DataListACV = new AdvancedCollectionView(DataList, true);

            using (DataListACV.DeferRefresh())
            {
                DataList.AddRange(myDataList);
            }
            currentSortDescription = new SortDescription("Title", SortDirection.Ascending);

            DataListACV.SortDescriptions.Add(currentSortDescription);
        }
        if (totalServerCount > 0)
        {
            StatusMessage = $"Added {existServer.Count}/{totalServerCount} Servers";
        }
        StatusTitle = $"{DataListACV?.Count} Local {PageType} Added";
        IsActive = false;
    }

    private async void DownloadServersOnLocalStorage()
    {
        try
        {
            if (Settings.TVTimeServers.Count == 0)
            {
                GoToServerPage();
                return;
            }

            IsServerStatusOpen = false;
            IsActive = true;
            var urls = Settings.TVTimeServers.Where(x => x.ServerType == ApplicationHelper.GetEnum<ServerType>(GetPageType()) && x.IsActive == true).ToList();
            IsStatusOpen = true;
            StatusSeverity = InfoBarSeverity.Informational;
            StatusTitle = "Please Wait...";
            StatusMessage = "";
            totalServerCount = urls.Count;

            int index = 0;
            existServer.Clear();
            foreach (var item in urls)
            {
                index++;

                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = await web.LoadFromWebAsync(item.Server);

                StatusMessage = $"Working on {item.Title} - {index}/{urls.Count()}";
                if (doc.DocumentNode.InnerHtml is null)
                {
                    continue;
                }
                string result = doc.DocumentNode?.InnerHtml?.ToString();
                StatusMessage = $"Parsing {item.Title}";

                var details = GetServerDetails(result, item);

                StatusMessage = $"Serializing {item.Title}";

                var filePath = Path.Combine(Constants.ServerDirectoryPath, GetPageType(), $"{GetMD5Hash(item.Server)}.txt");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using FileStream createStream = File.Create(filePath);
                await System.Text.Json.JsonSerializer.SerializeAsync(createStream, details, options);
                await createStream.DisposeAsync();

                // make sure data exist
                if (details.Any())
                {
                    existServer.Add(item.Server);
                }

                StatusMessage = $"{item.Title} Saved";
            }
            IsActive = false;
            StatusTitle = $"Updated Successfully: Added {existServer.Count}/{totalServerCount} Servers";
            StatusMessage = "We Updated our Local Storage";
            StatusSeverity = InfoBarSeverity.Success;
            IsServerStatusOpen = true;
            InfoBadgeValue = existServer.Count;
        }
        catch (Exception ex)
        {
            InfoBadgeValue = existServer.Count;
            IsActive = false;
            StatusTitle = $"Error: Added {existServer.Count}/{totalServerCount} Servers";
            StatusMessage = ex.Message;
            StatusSeverity = InfoBarSeverity.Error;
            IsServerStatusOpen = true;
        }

        LoadLocalStorage();
    }

    public List<MediaItem> GetServerDetails(string content, ServerModel server)
    {
        List<MediaItem> list = new List<MediaItem>();

        if (server.Server.Contains("DonyayeSerial"))
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var rows = doc.DocumentNode.SelectNodes("//table[@class='table']/tbody/tr");
            var ignoreLinks = new List<string> { "../", "Home", "DonyayeSerial", "series", "movie" };

            foreach (var row in rows)
            {
                var nameNode = row.SelectSingleNode("./td[@class='n']/a/code");
                var dateNode = row.SelectSingleNode("./td[@class='m']/code");
                var linkNode = row.SelectSingleNode("./td[@class='n']/a");
                var sizeNode = row.SelectSingleNode("./td[@class='s']");
                if (linkNode != null && !ignoreLinks.Contains(linkNode.Attributes["href"].Value))
                {
                    var title = nameNode?.InnerText?.Trim();
                    var date = dateNode?.InnerText?.Trim();
                    var serverUrl = $"{server.Server}{linkNode?.Attributes["href"]?.Value?.Trim()}";
                    var size = sizeNode?.InnerText?.Trim();
                    list.Add(new MediaItem { Title = title, DateTime = date, Server = serverUrl, FileSize = size, ServerType = ServerType.Series });
                }
            }
            return list;
        }
        else
        {
            MatchCollection m1 = Regex.Matches(content, @"(<a.*?>.*?</a>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            Regex dateTimeRegex = new Regex(Constants.DateTimeRegex, RegexOptions.IgnoreCase);
            MatchCollection dateTimeMatches = dateTimeRegex.Matches(content);

            int index = 0;
            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;
                MediaItem i = new MediaItem();

                Match m2 = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                string link = string.Empty;

                if (m2.Success)
                {
                    link = m2.Groups[1].Value;
                    if (server.Server.Contains("freelecher"))
                    {
                        var url = new Uri(server.Server).GetLeftPart(UriPartial.Authority);
                        i.Server = $"{url}{link}";
                    }
                    else if (server.Server.Contains("dl3.dl1acemovies") || server.Server.Contains("dl4.dl1acemovies"))
                    {
                        var url = new Uri(server.Server).GetLeftPart(UriPartial.Authority);
                        i.Server = $"{url}{link}";
                    }
                    else
                    {
                        string slash = string.Empty;
                        if (!server.Server.EndsWith("/"))
                        {
                            slash = "/";
                        }
                        i.Server = $"{server.Server}{slash}{link}";
                    }
                }

                string t = Regex.Replace(value, @"\s*<.*?>\s*", "", RegexOptions.Singleline);
                i.Title = RemoveSpecialWords(GetDecodedStringFromHtml(t));

                if (i.Server.Equals($"{server.Server}/../") || i.Server.Equals($"{server.Server}../") ||
                    i.Title.Equals("[To Parent Directory]") ||
                    ((i.Server.Contains("aiocdn") || i.Server.Contains("fbserver")) && link.Contains("?C=")))
                {
                    continue;
                }

                if (dateTimeMatches.Count > 0 && index <= dateTimeMatches.Count)
                {
                    var matchDate = dateTimeMatches[index].Value;
                    i.DateTime = matchDate;
                }

                index++;
                i.ServerType = server.ServerType;
                list.Add(i);
            }
            return list;
        }
    }

    public async Task<IReadOnlyList<StorageFile>> GetTextFilesAsync()
    {
        StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(Path.Combine(Constants.ServerDirectoryPath, GetPageType()));
        QueryOptions queryOptions = new QueryOptions(CommonFileQuery.OrderBySearchRank, new string[] { ".txt" });

        return await folder.CreateFileQueryWithOptions(queryOptions).GetFilesAsync();
    }

    public string GetBaseUrl(string url)
    {
        string[] qualityValues = { "/1080p/", "/720p/", "/480p/" };
        string quality = string.Empty;
        foreach (string value in qualityValues)
        {
            if (url.Contains(value, StringComparison.OrdinalIgnoreCase))
            {
                quality = value;
                break;
            }
        }
        int idx = url.IndexOf(quality);
        return idx >= 0 ? url.Substring(0, idx + quality.Length) : url;
    }

    public string GetBaseTitle(string title)
    {
        return Regex.Replace(title, @"S\d{2}E\d{2}.*", "").Trim();
    }

    public string GetPageType()
    {
        return PageType.ToString();
    }
}
