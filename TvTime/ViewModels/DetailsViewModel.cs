using System.Diagnostics;
using System.Text;

using CommunityToolkit.Labs.WinUI;

using Windows.ApplicationModel.DataTransfer;

namespace TvTime.ViewModels;
public partial class DetailsViewModel : ObservableRecipient
{
    [ObservableProperty]
    public ObservableCollection<MediaItem> dataList;

    [ObservableProperty]
    public AdvancedCollectionView dataListACV;

    [ObservableProperty]
    public ObservableCollection<MediaItem> breadcrumbBarList = new ();

    [ObservableProperty]
    public bool isStatusOpen;

    [ObservableProperty]
    public string statusMessage;

    [ObservableProperty]
    public string statusTitle;

    [ObservableProperty]
    public InfoBarSeverity statusSeverity;

    public List<string> suggestList = new List<string>();
    private SortDescription currentSortDescription;

    public MediaItem rootLocalItem;

    [RelayCommand]
    private void OnPageLoaded()
    {
        DownloadDetails(rootLocalItem);
    }

    [RelayCommand]
    private void OnSegmentedItemChanged(object sender)
    {
        var segmented = sender as Segmented;
        var selectedItem = segmented.SelectedItem as SegmentedItem;
        if (selectedItem != null)
        {
            switch (selectedItem.Tag?.ToString())
            {
                case "Refresh":
                    DownloadDetails(rootLocalItem);
                    segmented.SelectedIndex = -1;
                    break;
                case "Details":
                    GoToDetails();
                    segmented.SelectedIndex = -1;
                    break;
            }
        }
    }

    [RelayCommand]
    private async void OnSettingsCard(object sender)
    {
        var item = (sender as SettingsCard);
        var textBlock = item?.Header as TextBlock;
        var title = textBlock.Text?.Trim();
        var server = string.Empty;
        switch (Settings.DescriptionTemplate)
        {
            case DescriptionTemplateType.TextBlock:
                var descriptionTextBlock = item?.Description as TextBlock;
                server = descriptionTextBlock?.Text;
                break;
            case DescriptionTemplateType.HyperLink:
                var hyperLink = item?.Description as HyperlinkButton;
                var hyperLinkContent = hyperLink?.Content as TextBlock;
                server = hyperLinkContent?.Text;
                break;
        }

        if (Constants.FileExtensions.Any(server.Contains))
        {
            if (Settings.IsFileOpenInBrowser)
            {
                await Launcher.LaunchUriAsync(new Uri(server));
            }
            else
            {
                var fileName = System.IO.Path.GetFileName(server);
                await Launcher.LaunchUriAsync(new Uri(server.Replace(fileName, "")));
            }
        }
        else
        {
            var localItem = new MediaItem
            {
                Server = server,
                Title = title,
                ServerType = rootLocalItem.ServerType
            };
            DownloadDetails(localItem);
        }
    }

    private void GoToDetails()
    {
        var window = new IMDBDetailsWindow(rootLocalItem.Title);
        new ThemeManager(window, App.Current.ThemeManager.ThemeOptions);
        window.Activate();
    }

    [RelayCommand]
    private void OnBreadCrumbBarItem(BreadcrumbBarItemClickedEventArgs args)
    {
        var item = (MediaItem) args.Item;
        BreadcrumbBarList.RemoveAt(args.Index + 1);
        DownloadDetails(item);
    }

    [RelayCommand]
    private async void OnDownload(object sender)
    {
        var idmPath = GetIDMFilePath();
        if (string.IsNullOrEmpty(idmPath))
        {
            ContentDialog contentDialog = new ContentDialog
            {
                XamlRoot = App.Current.Window.Content.XamlRoot,
                Title = "IDM not found",
                Content = new InfoBar
                {
                    Margin = new Thickness(10),
                    Severity = InfoBarSeverity.Error,
                    Title = "IDM was not found on your system, please install it first",
                    IsOpen = true,
                    IsClosable = false
                },
                PrimaryButtonText = "Ok"
            };
            await contentDialog.ShowAsyncQueue();
        }
        else
        {
            var menuFlyout = (sender as MenuFlyoutItem);
            var localItem = (MediaItem) menuFlyout?.DataContext;
            var server = localItem.Server?.ToString();
            Process.Start(GetIDMFilePath(), $"/d \"{server?.ToString()}\"");
        }
    }

    [RelayCommand]
    private async void OnDownloadAll()
    {
        var idmPath = GetIDMFilePath();
        if (string.IsNullOrEmpty(idmPath))
        {
            ContentDialog contentDialog = new ContentDialog
            {
                XamlRoot = App.Current.Window.Content.XamlRoot,
                Title = "IDM not found",
                Content = new InfoBar
                {
                    Margin = new Thickness(10),
                    Severity = InfoBarSeverity.Error,
                    Title = "IDM was not found on your system, please install it first",
                    IsOpen = true,
                    IsClosable = false
                },
                PrimaryButtonText = "Ok"
            };
            await contentDialog.ShowAsyncQueue();
        }
        else
        {
            foreach (var item in DataList)
            {
                var localItem = (MediaItem) item;
                Process.Start(GetIDMFilePath(), $"/d \"{localItem.Server?.ToString()}\"");
                await Task.Delay(450);
            }
        }
    }

    [RelayCommand]
    private void OnCopy(object sender)
    {
        var item = (sender as MenuFlyoutItem);
        var localItem = (MediaItem) item?.DataContext;
        var server = localItem.Server?.ToString();
        var package = new DataPackage();
        package.SetText(server);
        Clipboard.SetContent(package);
    }

    [RelayCommand]
    private void OnCopyAll()
    {
        var package = new DataPackage();
        StringBuilder urls = new StringBuilder();
        foreach (var item in DataList)
        {
            var localItem = (MediaItem) item;
            urls.AppendLine(localItem.Server?.ToString());
        }
        package.SetText(urls?.ToString());
        Clipboard.SetContent(package);
    }

    [RelayCommand]
    private async void OnOpenDirectory(object sender)
    {
        var item = (sender as MenuFlyoutItem);
        var localItem = (MediaItem) item?.DataContext;
        var server = localItem.Server?.ToString();
        if (item.Text.Contains("File"))
        {
            await Launcher.LaunchUriAsync(new Uri(server));
        }
        else
        {
            if (Constants.FileExtensions.Any(server.Contains))
            {
                var fileName = System.IO.Path.GetFileName(server);
                await Launcher.LaunchUriAsync(new Uri(server.Replace(fileName, "")));
            }
            else
            {
                await Launcher.LaunchUriAsync(new Uri(server));
            }
        }
    }

    public bool DataListFilter(object item)
    {
        var query = (MediaItem) item;
        var name = query.Title ?? "";
        var tName = query.Server ?? "";
        var txtSearch = MainPage.Instance.GetTxtSearch();
        return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            || tName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
    }

    public void Search(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        AutoSuggestBoxHelper.LoadSuggestions(sender, args, suggestList);
        DataListACV.Filter = _ => true;
        DataListACV.Filter = DataListFilter;
    }

    private string GetIDMFilePath()
    {
        string idmPathX86 = @"C:\Program Files (x86)\Internet Download Manager\IDMan.exe"; // Update with the correct path to IDM executable
        string idmPathX64 = @"C:\Program Files\Internet Download Manager\IDMan.exe"; // Update with the correct path to IDM executable
        if (File.Exists(idmPathX64))
        {
            return idmPathX64;
        }
        else if (File.Exists(idmPathX86))
        {
            return idmPathX86;
        }
        else
        {
            return null;
        }
    }

    private async void DownloadDetails(MediaItem localItem)
    {
        try
        {
            BreadcrumbBarList.AddIfNotExists(localItem);
            IsActive = true;
            IsStatusOpen = true;
            StatusSeverity = InfoBarSeverity.Informational;
            StatusTitle = "Please Wait...";
            StatusMessage = "";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(localItem.Server);

            StatusMessage = $"Working on {localItem.Title}";

            string result = doc.DocumentNode?.InnerHtml?.ToString();
            StatusMessage = $"Parsing {localItem.Title}";

            var details = GetServerDetails(result, localItem);
            DataList = new(details);
            DataListACV = new AdvancedCollectionView(DataList, true);
            currentSortDescription = new SortDescription("Title", SortDirection.Ascending);
            DataListACV.SortDescriptions.Add(currentSortDescription);
            suggestList = DataListACV.Select(x => ((MediaItem) x).Title).ToList();

            IsActive = false;
            StatusTitle = "Updated Successfully";
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

    public List<MediaItem> GetServerDetails(string content, MediaItem localItem)
    {
        List<MediaItem> list = new List<MediaItem>();

        if (localItem.Server.Contains("DonyayeSerial"))
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
                    var serverUrl = $"{localItem.Server}{linkNode?.Attributes["href"]?.Value?.Trim()}";
                    var size = sizeNode?.InnerText?.Trim();
                    list.Add(new MediaItem { Title = title, DateTime = date, Server = serverUrl, FileSize = size, ServerType = ServerType.Series });
                }
            }
            return list;
        }
        else
        {
            MatchCollection m1 = Regex.Matches(content, @"(<a.*?>.*?</a>)", RegexOptions.Singleline|RegexOptions.IgnoreCase);

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
                    if (localItem.Server.Contains("freelecher"))
                    {
                        var url = new Uri(localItem.Server).GetLeftPart(UriPartial.Authority);
                        i.Server = $"{url}{link}";
                    }
                    else if (localItem.Server.Contains("dl3.dl1acemovies") || localItem.Server.Contains("dl4.dl1acemovies"))
                    {
                        var url = new Uri(localItem.Server).GetLeftPart(UriPartial.Authority);
                        i.Server = $"{url}{link}";
                    }
                    else
                    {
                        i.Server = $"{localItem.Server}{link}";
                    }
                }

                string t = Regex.Replace(value, @"\s*<.*?>\s*", "", RegexOptions.Singleline);

                i.Title = RemoveSpecialWords(GetDecodedStringFromHtml(t));
                if (i.Server.Equals($"{localItem.Server}../") || i.Title.Equals("[To Parent Directory]") ||
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
                        i.FileSize = GetFileSize((long) Convert.ToDouble(filesize));
                    }
                }
                index++;

                i.ServerType = localItem.ServerType;
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
