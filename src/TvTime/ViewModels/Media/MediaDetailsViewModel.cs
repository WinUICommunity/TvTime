namespace TvTime.ViewModels;
public partial class MediaDetailsViewModel : BaseViewModel
{
    [ObservableProperty]
    public ObservableCollection<ITvTimeModel> breadcrumbBarList = new();

    #region Override Methods

    public override void OnPageLoaded(object param)
    {
        DownloadDetails(rootTvTimeItem);
    }

    public override void OnRefresh()
    {
        DownloadDetails(rootTvTimeItem);
    }

    public override void OnIMDBDetail()
    {
        CreateIMDBDetailsWindow(rootTvTimeItem.Title);
    }

    public override void NavigateToDetails(object sender)
    {
        OnNavigateToDetailsOrDownload(sender);
    }

    public override bool DataListFilter(object item)
    {
        var query = (MediaItem) item;
        var name = query.Title ?? "";
        var tName = query.Server ?? "";
        var txtSearch = MainPage.Instance.GetTxtSearch();
        return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            || tName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    private async void OnNavigateToDetailsOrDownload(object sender)
    {
        base.NavigateToDetails(sender);

        if (Constants.FileExtensions.Any(descriptionText.Contains))
        {
            if (Settings.IsFileOpenInBrowser && !Settings.UseIDMForDownloadFiles)
            {
                await Launcher.LaunchUriAsync(new Uri(descriptionText));
            }
            else if (Settings.UseIDMForDownloadFiles)
            {
                LaunchIDM(GetIDMFilePath(), descriptionText);
            }
            else
            {
                var fileName = System.IO.Path.GetFileName(descriptionText);
                await Launcher.LaunchUriAsync(new Uri(descriptionText.Replace(fileName, "")));
            }
        }
        else
        {
            var mediaItem = new MediaItem
            {
                Server = descriptionText,
                Title = headerText,
                ServerType = rootTvTimeItem.ServerType
            };

            DownloadDetails(mediaItem);
        }
    }

    [RelayCommand]
    private void OnBreadCrumbBarItem(BreadcrumbBarItemClickedEventArgs args)
    {
        var item = (MediaItem) args.Item;
        BreadcrumbBarList.RemoveAt(args.Index + 1);
        DownloadDetails(item);
    }

    private async void DownloadDetails(ITvTimeModel tvTimeItem)
    {
        try
        {
            BreadcrumbBarList.AddIfNotExists(tvTimeItem);
            IsActive = true;
            IsStatusOpen = true;
            StatusSeverity = InfoBarSeverity.Informational;
            StatusTitle = App.Current.ResourceHelper.GetString("MediaDetailsViewModel_StatusWait");
            StatusMessage = "";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(tvTimeItem.Server);

            StatusMessage = string.Format(App.Current.ResourceHelper.GetString("MediaDetailsViewModel_StatusWorking"), tvTimeItem.Title);

            string result = doc.DocumentNode?.InnerHtml?.ToString();
            StatusMessage = string.Format(App.Current.ResourceHelper.GetString("MediaDetailsViewModel_StatusParsing"), tvTimeItem.Title);

            var details = GetServerDetails(result, tvTimeItem);
            DataList = new(details.Cast<ITvTimeModel>());
            DataListACV = new AdvancedCollectionView(DataList, true);
            currentSortDescription = new SortDescription("Title", SortDirection.Ascending);
            DataListACV.SortDescriptions.Add(currentSortDescription);
            suggestList = DataListACV.Select(x => ((MediaItem) x).Title).ToList();

            IsActive = false;
            StatusTitle = App.Current.ResourceHelper.GetString("MediaDetailsViewModel_StatusUpdated");
            StatusMessage = "";
            StatusSeverity = InfoBarSeverity.Success;
        }
        catch (Exception ex)
        {
            IsStatusOpen = true;
            IsActive = false;
            StatusTitle = "Error";
            StatusMessage = ex.Message + Environment.NewLine + ex.InnerException;
            StatusSeverity = InfoBarSeverity.Error;
        }
    }

    public List<MediaItem> GetServerDetails(string content, ITvTimeModel tvTimeItem)
    {
        List<MediaItem> list = new List<MediaItem>();

        if (tvTimeItem.Server.Contains("DonyayeSerial"))
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var rows = doc.DocumentNode.SelectNodes("//table[@class='table']/tbody/tr");
            var ignoreLinks = new List<string> { "../" };

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
                    var serverUrl = $"{tvTimeItem.Server}{linkNode?.Attributes["href"]?.Value?.Trim()}";
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

            Regex fileSizeRegex = new Regex("<br>(.*?)<", RegexOptions.IgnoreCase);
            MatchCollection dateTimeMatches = dateTimeRegex.Matches(content);

            var fileSizeContent = content.Replace("<br><br>", "<br>");
            var fileSizeMatches = fileSizeRegex.Matches(fileSizeContent);

            int index = 0;
            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;
                MediaItem i = new MediaItem();

                Match m2 = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.Singleline);
                string link = string.Empty;
                if (m2.Success)
                {
                    link = m2.Groups[1].Value;
                    if (tvTimeItem.Server.Contains("freelecher"))
                    {
                        var url = new Uri(tvTimeItem.Server).GetLeftPart(UriPartial.Authority);
                        i.Server = $"{url}{link}";
                    }
                    else if (tvTimeItem.Server.Contains("dl3.dl1acemovies") || tvTimeItem.Server.Contains("dl4.dl1acemovies"))
                    {
                        var url = new Uri(tvTimeItem.Server).GetLeftPart(UriPartial.Authority);
                        i.Server = $"{url}{link}";
                    }
                    else
                    {
                        i.Server = $"{tvTimeItem.Server}{link}";
                    }
                }

                string t = Regex.Replace(value, @"\s*<.*?>\s*", "", RegexOptions.Singleline);

                i.Title = RemoveSpecialWords(ApplicationHelper.GetDecodedStringFromHtml(t));
                if (i.Server.Equals($"{tvTimeItem.Server}../") || i.Title.Equals("[To Parent Directory]") ||
                    ((i.Server.Contains("aiocdn") || i.Server.Contains("fbserver")) && link.Contains("?C=")))
                {
                    continue;
                }

                if (dateTimeMatches.Count > 0 && index <= dateTimeMatches.Count)
                {
                    var matchDate = dateTimeMatches[index].Value;
                    i.DateTime = matchDate;
                }
                if (Constants.FileExtensions.Any(i.Server.Contains) && fileSizeMatches.Count > 0 && index <= fileSizeMatches.Count)
                {
                    var filesize = fileSizeMatches[index].Value;
                    if (index <= fileSizeMatches.Count)
                    {
                        if (filesize.Contains("<br><", StringComparison.OrdinalIgnoreCase))
                        {
                            filesize = fileSizeMatches[index + 1].Value;
                        }
                        filesize = ReplaceForFileSize(filesize, i.DateTime);
                        i.FileSize = ApplicationHelper.GetFileSize((long) Convert.ToDouble(filesize));
                    }
                }
                index++;

                i.ServerType = tvTimeItem.ServerType;
                list.Add(i);
            }
            return list;
        }
    }

    private string ReplaceForFileSize(string fileSize, string dateTime)
    {
        return fileSize.Replace(dateTime, "")
            .Replace("<br>", "")
            .Replace("<", "")
            .Replace("Monday", "")
            .Replace("Tuesday", "")
            .Replace("Wednesday", "")
            .Replace("Thursday", "")
            .Replace("Friday", "")
            .Replace("Saturday", "")
            .Replace("Sunday", "")
            .Replace(",", "").Trim();
    }
}
