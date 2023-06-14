using CommunityToolkit.Labs.WinUI;

namespace TvTime.Views;

public sealed partial class LocalUserControl : UserControl, INotifyPropertyChanged
{
    public PageOrDirectoryType PageType
    {
        get { return (PageOrDirectoryType) GetValue(PageTypeProperty); }
        set { SetValue(PageTypeProperty, value); }
    }
    public static readonly DependencyProperty PageTypeProperty =
        DependencyProperty.Register("PageType", typeof(PageOrDirectoryType), typeof(LocalUserControl), new PropertyMetadata(null));


    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool isActive;
    public bool IsActive
    {
        get { return isActive; }
        set
        {
            if (value != this.isActive)
            {
                this.isActive = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<LocalItem> dataList;
    public ObservableCollection<LocalItem> DataList
    {
        get { return dataList; }
        set
        {
            if (value != this.dataList)
            {
                this.dataList = value;
                OnPropertyChanged();
            }
        }
    }


    private AdvancedCollectionView dataListACV;
    public AdvancedCollectionView DataListACV
    {
        get { return dataListACV; }
        set
        {
            if (value != this.dataListACV)
            {
                this.dataListACV = value;
                OnPropertyChanged();
            }
        }
    }

    public List<string> suggestList = new List<string>();
    private List<string> existServer = new List<string>();

    private SortDescription currentSortDescription;
    JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
    private int totalServerCount = 0;

    public LocalUserControl()
    {
        this.InitializeComponent();
        DataContext = this;
        Loaded += LocalUserControl_Loaded;
    }

    public void Search(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        AutoSuggestBoxHelper.LoadSuggestions(sender, args, suggestList);
        DataListACV.Filter = _ => true;
        DataListACV.Filter = DataListFilter;
    }

    public string GetPageType()
    {
        return PageType.ToString();
    }

    private void LocalUserControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (ExistDirectory(PageType))
        {
            LoadLocalStorage();
        }
        else
        {
            DownloadServersOnLocalStorage();
        }

        if (Settings.Servers.Count == 0)
        {
            infoStatus.Title = "Server not found";
            infoStatus.Message = "Please add some Servers";
            infoStatus.Severity = InfoBarSeverity.Warning;
            btnServerStatus.Visibility = Visibility.Collapsed;
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
        if (idx >= 0)
            return url.Substring(0, idx + quality.Length);
        else
            return url;
    }

    public string GetBaseTitle(string title)
    {
        return Regex.Replace(title, @"S\d{2}E\d{2}.*", "").Trim();
    }
    private async void LoadLocalStorage()
    {
        IsActive = true;
        infoStatus.IsOpen = true;
        infoStatus.Severity = InfoBarSeverity.Informational;
        infoStatus.Title = $"Loading Local {PageType}...";
        var files = Directory.GetFiles(Path.Combine(Constants.ServerDirectoryPath, GetPageType()), "*.txt");
        if (files.Any())
        {
            var tasks = files.Select(async file =>
            {
                using FileStream openStream = File.OpenRead(file);
                return await System.Text.Json.JsonSerializer.DeserializeAsync<List<LocalItem>>(openStream, options);
            });
            var contentsList = await Task.WhenAll(tasks);
            var contents = contentsList.SelectMany(x => x);

            if (PageType == PageOrDirectoryType.Series)
            {
                // Find the items containing "Iranian/Series" and extract the base URL, and Remove duplicates from the filtered list
                var filteredContents = contents.Where(c => c.Server.Contains("Series/Iranian"))
                               .Select(c => new LocalItem {
                                   Server = GetBaseUrl(c.Server),
                                   Title = GetBaseTitle(c.Title),
                                   FileSize = c.FileSize,
                                   DateTime = c.DateTime,
                                   ServerType = c.ServerType
                               }).DistinctBy(x=>x.Server);

                // Merge the unique and non-filtered items

                var finalContents = contents.Where(c => !c.Server.Contains("Series/Iranian"))
                            .Concat(filteredContents.Select(u => new LocalItem {
                                Server = u.Server,
                                Title = u.Title,
                                FileSize = u.FileSize,
                                DateTime = u.DateTime,
                                ServerType = u.ServerType
                            }));

                // Update the LocalItem list
                var updatedList = finalContents.Select(c => new LocalItem
                {
                    Server = c.Server,
                    Title = c.Title,
                    FileSize = c.FileSize,
                    DateTime = c.DateTime,
                    ServerType = ServerType.Series
                }).ToList();
                contents = updatedList;
            }
            var myDataList = contents.Where(x => x.Server != null);
            DataList = new();
            DataList.AddRange(myDataList);
            currentSortDescription = new SortDescription("Title", SortDirection.Ascending);
            suggestList = myDataList.Select(x => ((LocalItem) x).Title).ToList();

            DataListACV = new AdvancedCollectionView(DataList, true);
            DataListACV.SortDescriptions.Add(currentSortDescription);
        }
        if (totalServerCount > 0)
        {
            infoStatus.Message = $"Added {existServer.Count}/{totalServerCount} Servers";
        }
        infoStatus.Title = $"{DataListACV?.Count} Local {PageType} Added";
        IsActive = false;
    }
    private string GetSeriesTitle(string title)
    {
        return title.Remove(0, title.IndexOf("Iranian/"))?.Replace("Iranian/", "")?.Replace("%20", " ");
    }
    private async void DownloadServersOnLocalStorage()
    {
        try
        {
            btnServerStatus.Visibility = Visibility.Collapsed;
            segRefresh.IsEnabled = false;
            IsActive = true;
            var urls = Settings.Servers.Where(x => x.ServerType == ApplicationHelper.GetEnum<ServerType>(GetPageType()) && x.IsActive == true).ToList();
            infoStatus.IsOpen = true;
            infoStatus.Severity = InfoBarSeverity.Informational;
            infoStatus.Title = "Please Wait...";
            infoStatus.Message = "";
            totalServerCount = urls.Count;

            int index = 0;
            existServer.Clear();
            foreach (var item in urls)
            {
                index++;

                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = await web.LoadFromWebAsync(item.Server);

                infoStatus.Message = $"Working on {item.Title} - {index}/{urls.Count()}";
                if (doc.DocumentNode.InnerHtml is null)
                {
                    continue;
                }
                string result = doc.DocumentNode?.InnerHtml?.ToString();
                infoStatus.Message = $"Parsing {item.Title}";

                var details = GetServerDetails(result, item);

                infoStatus.Message = $"Serializing {item.Title}";

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

                infoStatus.Message = $"{item.Title} Saved";
            }
            IsActive = false;
            infoStatus.Title = $"Updated Successfully: Added {existServer.Count}/{totalServerCount} Servers";
            infoStatus.Message = "We Updated our Local Storage";
            infoStatus.Severity = InfoBarSeverity.Success;
            btnServerStatus.Visibility = Visibility.Visible;
            buttonInfoBadge.Value = existServer.Count;
            segRefresh.IsEnabled = true;
        }
        catch (Exception ex)
        {
            segRefresh.IsEnabled = true;
            buttonInfoBadge.Value = existServer.Count;
            IsActive = false;
            infoStatus.Title = $"Error: Added {existServer.Count}/{totalServerCount} Servers";
            infoStatus.Message = ex.Message;
            infoStatus.Severity = InfoBarSeverity.Error;
            btnServerStatus.Visibility = Visibility.Visible;
        }

        LoadLocalStorage();
    }

    public List<LocalItem> GetServerDetails(string content, ServerModel server)
    {
        List<LocalItem> list = new List<LocalItem>();

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
                    list.Add(new LocalItem { Title = title, DateTime = date, Server = serverUrl, FileSize = size, ServerType = ServerType.Series });
                }
            }
            return list;
        }
        else
        {
            MatchCollection m1 = Regex.Matches(content, @"(<a.*?>.*?</a>)",
            RegexOptions.Singleline);

            Regex dateTimeRegex = new Regex(Constants.DateTimeRegex, RegexOptions.IgnoreCase);
            MatchCollection dateTimeMatches = dateTimeRegex.Matches(content);

            int index = 0;
            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;
                LocalItem i = new LocalItem();

                Match m2 = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.Singleline);

                string link = string.Empty;

                if (m2.Success)
                {
                    link = m2.Groups[1].Value;
                    if (server.Server.Contains("freelecher"))
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

                if (i.Server.Equals($"{server.Server}/../") || i.Server.Equals($"{server.Server}../") || i.Title.Equals("[To Parent Directory]") || (i.Server.Contains("aiocdn") && link.Contains("?C=")))
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
    
    private void btnServerStatus_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog contentDialog = new ContentDialog();
        contentDialog.XamlRoot = XamlRoot;
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

    public bool DataListFilter(object item)
    {
        var query = (LocalItem) item;
        var name = query.Title ?? "";
        var tName = query.Server ?? "";
        var txtSearch = MainPage.Instance.GetTxtSearch();
        return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            || tName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
    }

    private async void btnOpenDirectory_Click(object sender, RoutedEventArgs e)
    {
        var menuFlyout = (sender as MenuFlyoutItem);
        var localItem = (LocalItem) menuFlyout?.DataContext;
        var server = localItem.Server?.ToString();
        await Launcher.LaunchUriAsync(new Uri(server));
    }

    private void SettingsCard_Click(object sender, RoutedEventArgs e)
    {
        var setting = (sender as SettingsCard);
        var textBlock = setting?.Header as TextBlock;
        var title = textBlock.Text.Trim();
        var server = string.Empty;

        switch (Settings.DescriptionType)
        {
            case DescriptionType.TextBlock:
                server = setting?.Description?.ToString();
                break;
            case DescriptionType.HyperLink:
                var hyperLink = setting?.Description as HyperlinkButton;
                server = hyperLink?.Content?.ToString();
                break;
        }

        var item = new LocalItem
        {
            Server = server,
            Title = title,
            ServerType = ApplicationHelper.GetEnum<ServerType>(GetPageType())
        };

        App.Current.NavigationManager.NavigateForJson(typeof(DetailPage), item);
    }

    private void segmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = segmented.SelectedItem as SegmentedItem;
        if (item != null)
        {
            switch (item.Tag?.ToString())
            {
                case "Refresh":
                    DeleteDirectory(PageType);
                    DownloadServersOnLocalStorage();
                    segmented.SelectedIndex = -1;
                    break;
            }
        }
    }
}
