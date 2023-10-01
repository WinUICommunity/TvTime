using System.Text.RegularExpressions;

using CommunityToolkit.WinUI.UI;

using HtmlAgilityPack;

using TvTime.Database.Tables;

using Windows.System;

namespace TvTime.ViewModels;
public partial class MediaDetailsViewModel : BaseViewModel, ITitleBarAutoSuggestBoxAware
{
    [ObservableProperty]
    public ObservableCollection<BaseMediaTable> breadcrumbBarList = new();

    private DispatcherTimer dispatcherTimer = new DispatcherTimer();

    public override void OnPageLoaded(object param)
    {
        DownloadDetails(rootMedia);
    }

    public override void OnRefresh()
    {
        DownloadDetails(rootMedia);
    }

    public override void OnIMDBDetail()
    {
        CreateIMDBDetailsWindow(rootMedia.Title);
    }

    public override void NavigateToDetails(object sender)
    {
        OnNavigateToDetailsOrDownload(sender);
    }

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
                var fileName = Path.GetFileName(descriptionText);
                await Launcher.LaunchUriAsync(new Uri(descriptionText.Replace(fileName, "")));
            }
        }
        else
        {
            var baseMedia = new BaseMediaTable(headerText, descriptionText, null, null, rootMedia.ServerType);
            DownloadDetails(baseMedia);
            rootMedia.Server = descriptionText;
        }
    }

    [RelayCommand]
    private void OnBreadCrumbBarItem(BreadcrumbBarItemClickedEventArgs args)
    {
        var item = (BaseMediaTable) args.Item;
        BreadcrumbBarList.RemoveAt(args.Index + 1);
        DownloadDetails(item);
    }

    private async void DownloadDetails(BaseMediaTable baseMedia)
    {
        try
        {
            BreadcrumbBarList.AddIfNotExists(baseMedia);
            IsActive = true;
            IsStatusOpen = true;
            StatusSeverity = InfoBarSeverity.Informational;
            StatusTitle = "Please Wait...";
            StatusMessage = "";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(baseMedia.Server);

            StatusMessage = string.Format("Working on {0}", baseMedia.Title);

            string result = doc.DocumentNode?.InnerHtml?.ToString();
            StatusMessage = string.Format("Parsing {0}", baseMedia.Title);

            var details = GetServerDetails(result, baseMedia);
            DataList = new(details);
            DataListACV = new AdvancedCollectionView(DataList, true);
            DataListACV.SortDescriptions.Add(new SortDescription("Title", SortDirection.Ascending));

            IsActive = false;
            StatusTitle = "Media information received successfully";
            StatusMessage = "";
            StatusSeverity = InfoBarSeverity.Success;
            AutoHideStatusInfoBar(new TimeSpan(0, 0, 4));
        }
        catch (Exception ex)
        {
            IsStatusOpen = true;
            IsActive = false;
            StatusTitle = "Error";
            StatusMessage = ex.Message + Environment.NewLine + ex.InnerException;
            StatusSeverity = InfoBarSeverity.Error;
            Logger?.Error(ex, "MediaDetailsViewModel: DownloadDetails");
        }
    }
    public bool ContinueIfWrongData(string title, string server, string link, BaseMediaTable baseMedia)
    {
        if (string.IsNullOrEmpty(title) || server.Equals($"{baseMedia.Server}../") ||
            title.Equals("[To Parent Directory]") || title.Equals("../") || server.Contains("?C=") ||
            ((server.Contains("fbserver")) && link.Contains("?C=")))
        {
            return true;
        }
        return false;
    }

    public string FixTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
            return title;

        title = Regex.Replace(title, @"\s*<.*?>\s*", "", RegexOptions.Singleline);
        title = title.Replace(".E..&gt;", "").Replace(">", "");
        title = RemoveSpecialWords(ApplicationHelper.GetDecodedStringFromHtml(title));
        return title;
    }

    public List<BaseMediaTable> GetFilmbbinServerDetails(string content, BaseMediaTable baseMedia)
    {
        List<BaseMediaTable> list = new List<BaseMediaTable>();
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(content);
        var nodes = doc?.DocumentNode?.SelectNodes("//a[@href]");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                var href = node?.GetAttributeValue("href", "");

                var title = node?.InnerText;
                var date = node?.NextSibling?.InnerText?.Trim();
                long fSize = 0;
                var dateAndSize = date?.Split("  ");
                if (dateAndSize?.Length > 1)
                {
                    date = dateAndSize[0];
                    long.TryParse(dateAndSize[dateAndSize.Length - 1], out fSize);
                }

                if (ContinueIfWrongData(title, href, href, baseMedia))
                {
                    continue;
                }

                list.Add(new BaseMediaTable(FixTitle(title), $"{baseMedia.Server}{href}", date, ApplicationHelper.GetFileSize(fSize), baseMedia.ServerType));
            }
        }
        return list;
    }

    public List<BaseMediaTable> GetRostamServerDetails(string content, BaseMediaTable baseMedia)
    {
        List<BaseMediaTable> list = new List<BaseMediaTable>();
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(content);
        var nodes = doc?.DocumentNode?.SelectNodes("//tr[td[@class='link']]");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                var href = node?.SelectSingleNode("td[@class='link']/a")?.GetAttributeValue("href", "");

                var title = node?.SelectSingleNode("td[@class='link']/a")?.GetAttributeValue("title", "");

                var size = node?.SelectSingleNode("td[@class='size']")?.InnerText;

                var date = node?.SelectSingleNode("td[@class='date']")?.InnerText;

                if (ContinueIfWrongData(FixTitle(title), href, href, baseMedia))
                {
                    continue;
                }

                list.Add(new BaseMediaTable(FixTitle(title), $"{baseMedia.Server}{href}", date, size, baseMedia.ServerType));
            }
        }
        return list;
    }

    public List<BaseMediaTable> GetDonyayeSerialServerDetails(string content, BaseMediaTable baseMedia)
    {
        List<BaseMediaTable> list = new List<BaseMediaTable>();
        var doc = new HtmlDocument();
        doc.LoadHtml(content);
        var rows = doc.DocumentNode.SelectNodes("//table[@class='table']/tbody/tr");
        var ignoreLinks = new List<string> { "../" };

        if (rows != null)
        {
            foreach (var row in rows)
            {
                var nameNode = row?.SelectSingleNode("./td[@class='n']/a/code");
                var dateNode = row?.SelectSingleNode("./td[@class='m']/code");
                var linkNode = row?.SelectSingleNode("./td[@class='n']/a");
                var sizeNode = row?.SelectSingleNode("./td[@class='s']");
                if (linkNode != null && !ignoreLinks.Contains(linkNode?.Attributes["href"]?.Value))
                {
                    var title = nameNode?.InnerText?.Trim();
                    var date = dateNode?.InnerText?.Trim();
                    var serverUrl = $"{baseMedia.Server}{linkNode?.Attributes["href"]?.Value?.Trim()}";
                    var size = sizeNode?.InnerText?.Trim();

                    list.Add(new BaseMediaTable(title, serverUrl, date, size, ServerType.Series));
                }
            }
        }
        return list;
    }

    public List<BaseMediaTable> GetServerDetails(string content, BaseMediaTable baseMedia)
    {
        List<BaseMediaTable> list = new List<BaseMediaTable>();

        try
        {
            if (baseMedia.Server.Contains("DonyayeSerial"))
            {
                return GetDonyayeSerialServerDetails(content, baseMedia);
            }
            else if (baseMedia.Server.Contains("rostam"))
            {
                return GetRostamServerDetails(content, baseMedia);
            }
            else if (baseMedia.Server.Contains("filmbbin"))
            {
                return GetFilmbbinServerDetails(content, baseMedia);
            }
            else
            {
                MatchCollection m1 = Regex.Matches(content, @"(<a.*?>.*?</a>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                Regex dateTimeRegex = new Regex(Constants.DateTimeRegex, RegexOptions.IgnoreCase);

                Regex fileSizeRegex = new Regex("<br>(.*?)<", RegexOptions.IgnoreCase);
                Regex freelecherFileSizeRegex = new Regex(@"<\/a>([^<]+)", RegexOptions.IgnoreCase);
                MatchCollection dateTimeMatches = dateTimeRegex.Matches(content);

                var fileSizeContent = content.Replace("<br><br>", "<br>");
                var fileSizeMatches = fileSizeRegex.Matches(fileSizeContent);

                if (fileSizeMatches.Count == 0)
                {
                    fileSizeMatches = freelecherFileSizeRegex.Matches(fileSizeContent);
                }
                List<Match> fileSizeMatchesList = new List<Match>(fileSizeMatches.Cast<Match>());

                int index = 0;
                foreach (Match m in m1)
                {
                    string value = m.Groups[1].Value;
                    BaseMediaTable i = new BaseMediaTable();

                    Match m2 = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.Singleline);
                    string link = string.Empty;
                    if (m2.Success)
                    {
                        link = m2.Groups[1].Value;
                        if (baseMedia.Server.Contains("freelecher") && !baseMedia.Server.Contains("https://dl.freelecher") && !baseMedia.Server.Contains("https://dl4.freelecher") && !baseMedia.Server.Contains("https://dl3.freelecher"))
                        {
                            var url = new Uri(baseMedia.Server).GetLeftPart(UriPartial.Authority);
                            i.Server = $"{url}{link}";
                        }
                        else if (baseMedia.Server.Contains("dl3.dl1acemovies") || baseMedia.Server.Contains("dl4.dl1acemovies"))
                        {
                            var url = new Uri(baseMedia.Server).GetLeftPart(UriPartial.Authority);
                            i.Server = $"{url}{link}";
                        }
                        else
                        {
                            i.Server = $"{baseMedia.Server}{link}";
                        }
                    }

                    string title = FixTitle(value);

                    if (ContinueIfWrongData(title, i.Server, link, baseMedia))
                    {
                        continue;
                    }

                    if (dateTimeMatches.Count > 0 && index <= dateTimeMatches.Count)
                    {
                        var matchDate = dateTimeMatches[index].Value;
                        i.DateTime = matchDate;
                    }

                    if (Constants.FileExtensions.Any(i.Server.Contains) && fileSizeMatchesList.Count > 0 && index <= fileSizeMatchesList.Count)
                    {
                        var filesize = fileSizeMatchesList[index].Value;
                        if (baseMedia.Server.Contains("https://dl.freelecher") || baseMedia.Server.Contains("https://dl4.freelecher") || baseMedia.Server.Contains("https://dl3.freelecher"))
                        {
                            filesize = fileSizeMatchesList.Where(x=>x.Value.Contains(i.DateTime)).FirstOrDefault().Value;
                            filesize = filesize.Replace(i.DateTime, "").Replace("</a>","").Trim();
                        }

                        if (index <= fileSizeMatchesList.Count)
                        {
                            if (filesize.Contains("<br><", StringComparison.OrdinalIgnoreCase))
                            {
                                filesize = fileSizeMatchesList[index + 1].Value;
                            }
                            filesize = ReplaceForFileSize(filesize, i.DateTime);
                            long fSize;
                            long.TryParse(filesize, out fSize);
                            i.FileSize = ApplicationHelper.GetFileSize(fSize);
                        }
                    }
                    index++;

                    i.ServerType = baseMedia.ServerType;
                    list.Add(i);
                }
                return list;
            }
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, "MediaDetailsViewModel: GetServerDetails");
        }
        return list;
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

    private void AutoHideStatusInfoBar(TimeSpan timeSpan)
    {
        dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Tick += (s, e) =>
        {
            dispatcherTimer?.Stop();
            dispatcherTimer = null;
            StatusMessage = "";
            StatusTitle = "";
            StatusSeverity = InfoBarSeverity.Informational;
            IsStatusOpen = false;
        };
        dispatcherTimer.Interval = timeSpan;
        dispatcherTimer.Start();
    }

    public void OnAutoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        Search(sender.Text);
    }

    public void OnAutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Search(sender.Text);
    }

    private void Search(string query)
    {
        if (DataList != null && DataList.Any())
        {
            DataListACV.Filter = _ => true;
            DataListACV.Filter = item =>
            {
                var baseMedia = (BaseMediaTable)item;
                var name = baseMedia.Title ?? "";
                var tName = baseMedia.Server ?? "";
                return name.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || tName.Contains(query, StringComparison.OrdinalIgnoreCase);
            };
        }
    }
}
