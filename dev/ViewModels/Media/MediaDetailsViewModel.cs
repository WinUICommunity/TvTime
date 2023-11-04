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
    private BaseMediaTable dynamicMediaTable;
    public override void OnPageLoaded(object param)
    {
        dynamicMediaTable = rootMedia;
        DownloadDetails(rootMedia);
    }

    public override void OnRefresh()
    {
        DownloadDetails(dynamicMediaTable);
    }

    public override void OnIMDBDetail()
    {
        CreateIMDBDetailsWindow(rootMedia.Title);
    }

    public override async void NavigateToDetails(object sender)
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
            dynamicMediaTable = baseMedia;
        }
    }

    [RelayCommand]
    private void OnBreadCrumbBarItem(BreadcrumbBarItemClickedEventArgs args)
    {
        var item = (BaseMediaTable) args.Item;
        BreadcrumbBarList.RemoveAt(args.Index + 1);
        dynamicMediaTable = item;
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

    public List<BaseMediaTable> GetAllServerDetails(string content, BaseMediaTable baseMedia)
    {
        List<BaseMediaTable> list = new List<BaseMediaTable>();

        try
        {
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
                    if (string.IsNullOrEmpty(date))
                    {
                        date = node?.PreviousSibling?.InnerText?.Trim();
                    }

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

                    var finalServer = ConcatenateUrls(baseMedia.Server, href);

                    if (finalServer.Contains("dl5.dl1acemovies", StringComparison.OrdinalIgnoreCase))
                    {
                        if (title.Equals("Home") || title.Equals("dl") ||
                            title.Equals("English") || title.Equals("Series") ||
                            title.Equals("Movie") || title.Contains("Parent Directory") ||
                            BreadcrumbBarList.Any(x=>x.Title.Equals(FixTitle(title))))
                        {
                            continue;
                        }
                    }
                    
                    list.Add(new BaseMediaTable(FixTitle(title), finalServer, date, ApplicationHelper.GetFileSize(fSize), baseMedia.ServerType));
                }
            }
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, "MediaDetailsViewModel: GetAllServerDetails");
            IsStatusOpen = true;
            IsActive = false;
            StatusTitle = "Error";
            StatusMessage = ex.Message;
            StatusSeverity = InfoBarSeverity.Error;
        }
        
        return list;
    }

    public List<BaseMediaTable> GetRostamAndFbServerAndFreeLecherServerDetails(string content, BaseMediaTable baseMedia)
    {
        List<BaseMediaTable> list = new List<BaseMediaTable>();

        try
        {
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
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, "MediaDetailsViewModel: GetRostamAndFbServerServerDetails");
            IsStatusOpen = true;
            IsActive = false;
            StatusTitle = "Error";
            StatusMessage = ex.Message;
            StatusSeverity = InfoBarSeverity.Error;
        }
        
        return list;
    }

    public List<BaseMediaTable> GetDonyayeSerialServerDetails(string content, BaseMediaTable baseMedia)
    {
        List<BaseMediaTable> list = new List<BaseMediaTable>();
        try
        {
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
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, "MediaDetailsViewModel: GetDonyayeSerialServerDetails");
            IsStatusOpen = true;
            IsActive = false;
            StatusTitle = "Error";
            StatusMessage = ex.Message;
            StatusSeverity = InfoBarSeverity.Error;
        }
        return list;
    }

    public List<BaseMediaTable> GetServerDetails(string content, BaseMediaTable baseMedia)
    {
        List<BaseMediaTable> list = new List<BaseMediaTable>();

        try
        {
            if (baseMedia.Server.Contains("DonyayeSerial", StringComparison.OrdinalIgnoreCase))
            {
                return GetDonyayeSerialServerDetails(content, baseMedia);
            }
            else if (baseMedia.Server.Contains("rostam", StringComparison.OrdinalIgnoreCase) || baseMedia.Server.Contains("fbserver", StringComparison.OrdinalIgnoreCase) ||
                baseMedia.Server.Contains("freelecher", StringComparison.OrdinalIgnoreCase))
            {
                return GetRostamAndFbServerAndFreeLecherServerDetails(content, baseMedia);
            }
            else
            {
                return GetAllServerDetails(content, baseMedia);
            }
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, "MediaDetailsViewModel: GetServerDetails");
        }
        return list;
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
