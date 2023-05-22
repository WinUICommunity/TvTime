using System.Diagnostics;
using System.Text;
using Windows.ApplicationModel.DataTransfer;

namespace TvTime.Views;

public sealed partial class DetailPage : Page, INotifyPropertyChanged
{
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

    private ObservableCollection<LocalItem> breadcrumbBarList = new ObservableCollection<LocalItem>();

    public ObservableCollection<LocalItem> BreadcrumbBarList
    {
        get { return breadcrumbBarList; }
        set
        {
            if (value != this.breadcrumbBarList)
            {
                this.breadcrumbBarList = value;
                OnPropertyChanged();
            }
        }
    }


    public List<string> suggestList = new List<string>();

    private SortDescription currentSortDescription;

    private LocalItem rootLocalItem;
    public DetailPage()
    {
        this.InitializeComponent();
        Loaded += DetailPage_Loaded;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var args = e.Parameter as NavigationArgs;
        var item = (LocalItem)args.Parameter;
        rootLocalItem = item;
        breadcrumbBarList?.Clear();
    }

    private void DetailPage_Loaded(object sender, RoutedEventArgs e)
    {
        DownloadDetails(rootLocalItem);
    }
    public void Search(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        AutoSuggestBoxHelper.LoadSuggestions(sender, args, suggestList);
        DataListACV.Filter = _ => true;
        DataListACV.Filter = DataListFilter;
    }
    private async void DownloadDetails(LocalItem localItem)
    {
        try
        {
            breadcrumbBarList.AddIfNotExists(localItem);
            btnRefresh.IsEnabled = false;
            IsActive = true;
            infoStatus.IsOpen = true;
            infoStatus.Severity = InfoBarSeverity.Informational;
            infoStatus.Title = "Please Wait...";
            infoStatus.Message = "";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(localItem.Server);

            infoStatus.Message = $"Working on {localItem.Title}";
           
            string result = doc.DocumentNode?.InnerHtml?.ToString();
            infoStatus.Message = $"Parsing {localItem.Title}";

            var details = GetServerDetails(result, localItem);
            DataList = new(details);
            DataListACV = new AdvancedCollectionView(DataList, true);
            currentSortDescription = new SortDescription("Title", SortDirection.Ascending);
            DataListACV.SortDescriptions.Add(currentSortDescription);
            suggestList = DataListACV.Select(x => ((LocalItem) x).Title).ToList();

            IsActive = false;
            infoStatus.Title = "Updated Successfully";
            infoStatus.Message = "";
            infoStatus.Severity = InfoBarSeverity.Success;
            btnRefresh.IsEnabled = true;
        }
        catch (Exception ex)
        {
            btnRefresh.IsEnabled = true;
            IsActive = false;
            infoStatus.Title = "Error";
            infoStatus.Message = ex.Message + Environment.NewLine + ex.InnerException;
            infoStatus.Severity = InfoBarSeverity.Error;
        }
    }
    public List<LocalItem> GetServerDetails(string content, LocalItem localItem)
    {
        List<LocalItem> list = new List<LocalItem>();

        if (localItem.Server.Contains("DonyayeSerial"))
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var rows = doc.DocumentNode.SelectNodes("//table[@class='table']/tbody/tr");
            var ignoreLinks = new List<string> { "../"};

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
                    list.Add(new LocalItem { Title = title, DateTime = date, Server = serverUrl, FileSize = size, ServerType = ServerType.Series });
                }
            }
            return list;
        }
        else
        {
            MatchCollection m1 = Regex.Matches(content, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);

            Regex dateTimeRegex = new Regex(Constants.DateTimeRegex, RegexOptions.IgnoreCase);
            Regex fileSizeRegex = new Regex("<br>(.*?)<", RegexOptions.IgnoreCase);
            MatchCollection dateTimeMatches = dateTimeRegex.Matches(content);

            var fileSizeContent = content.Replace("<br><br>", "<br>");
            MatchCollection fileSizeMatches = fileSizeRegex.Matches(fileSizeContent);
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
                    if (localItem.Server.Contains("freelecher"))
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
                if (i.Server.Equals($"{localItem.Server}../") || i.Title.Equals("[To Parent Directory]") || (i.Server.Contains("aiocdn") && link.Contains("?C=")))
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

    private void btnRefresh_Click(object sender, RoutedEventArgs e)
    {
        DownloadDetails(rootLocalItem);
    }

    public bool DataListFilter(object item)
    {
        var query = (LocalItem) item;
        var name = query.Title ?? "";
        var tName = query.Server ?? "";
        var txtSearch = MainWindow.Instance.GetTxtSearch();
        return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            || tName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
    }

    private void breadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        var item = (LocalItem)args.Item;
        breadcrumbBarList.RemoveAt(args.Index + 1);
        DownloadDetails(item);
    }

    private async void btnOpenDirectory_Click(object sender, RoutedEventArgs e)
    {
        var menuFlyout = (sender as MenuFlyoutItem);
        var localItem = (LocalItem)menuFlyout?.Tag;
        var server = localItem.Server?.ToString();
        if (menuFlyout.Text.Contains("File"))
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

    private async void SettingsCard_Click(object sender, RoutedEventArgs e)
    {
        var setting = (sender as SettingsCard);
        var server = setting?.Description?.ToString();
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
            var localItem = new LocalItem
            {
                Server = setting?.Description?.ToString(),
                Title = setting?.Header.ToString(),
                ServerType = rootLocalItem.ServerType
            };
            DownloadDetails(localItem);
        }
    }
    private void btnCopyDownloadLink_Click(object sender, RoutedEventArgs e)
    {
        var menuFlyout = (sender as MenuFlyoutItem);
        var localItem = (LocalItem) menuFlyout?.Tag;
        var server = localItem.Server?.ToString();
        var package = new DataPackage();
        package.SetText(server);
        Clipboard.SetContent(package);
    }
    private void btnCopyAllDownloadLinks_Click(object sender, RoutedEventArgs e)
    {
        var items = listView.Items;
        var package = new DataPackage();
        StringBuilder urls = new StringBuilder();
        foreach ( var item in items )
        {
            var localItem = (LocalItem) item;
            urls.AppendLine(localItem.Server?.ToString());
        }
        package.SetText(urls?.ToString());
        Clipboard.SetContent(package);
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
    private async void btnDownloadWithIDM_Click(object sender, RoutedEventArgs e)
    {
        var idmPath = GetIDMFilePath();
        if (string.IsNullOrEmpty(idmPath))
        {
            ContentDialog contentDialog = new ContentDialog
            {
                XamlRoot = MainWindow.Instance.Content.XamlRoot,
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
            var localItem = (LocalItem) menuFlyout?.Tag;
            var server = localItem.Server?.ToString();
            Process.Start(GetIDMFilePath(), $"/d \"{server?.ToString()}\"");
        }
    }
    private async void btnDownloadAllLinksWithIDM_Click(object sender, RoutedEventArgs e)
    {
        var idmPath = GetIDMFilePath();
        if (string.IsNullOrEmpty(idmPath))
        {
            ContentDialog contentDialog = new ContentDialog
            {
                XamlRoot = MainWindow.Instance.Content.XamlRoot,
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
            var items = listView.Items;
            foreach (var item in items)
            {
                var localItem = (LocalItem) item;
                Process.Start(GetIDMFilePath(), $"/d \"{localItem.Server?.ToString()}\"");
                await Task.Delay(400);
            }
        }
    }

    private void btnDetails_Click(object sender, RoutedEventArgs e)
    {
        var window = new IMDBDetailsWindow();
        window.Title = rootLocalItem.Title;
        new ThemeManager(window, (Application.Current as App).themeManager.ThemeOptions);
        window.Activate();
    }
}
