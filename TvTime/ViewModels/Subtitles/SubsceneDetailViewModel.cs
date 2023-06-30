using System.Diagnostics;

using CommunityToolkit.Labs.WinUI;

using Downloader;

namespace TvTime.ViewModels;
public partial class SubsceneDetailViewModel : BaseViewModel
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    [ObservableProperty]
    public ObservableCollection<ITvTimeModel> breadcrumbBarList = new();

    [ObservableProperty]
    public ObservableCollection<TokenItem> languageTokenList;

    [ObservableProperty]
    public ObservableCollection<TokenItem> qualityTokenList;

    #region Override Methods

    public override void OnPageLoaded(object param)
    {
        var languageTokens = SubtitleLanguageCollection()
            .Select(x => new TokenItem { Content = x });

        LanguageTokenList = new(languageTokens);
        LanguageTokenList.Insert(0, new TokenItem { Content = Constants.ALL_FILTER, IsSelected = true });

        DownloadDetails(rootTvTimeItem);
    }

    public override void OnRefresh()
    {
        DownloadDetails(rootTvTimeItem);
    }

    public override void OnDetail()
    {
        CreateIMDBDetailsWindow(rootTvTimeItem.Title);
    }

    public override void NavigateToDetails(object sender)
    {
        OnNavigateToDetailsOrDownload(sender);
    }

    public override bool DataListFilter(object item)
    {
        var query = (SubsceneModel) item;
        var name = query.Title ?? "";
        var lang = query.Language ?? "";
        var txtSearch = MainPage.Instance.GetTxtSearch();
        var languageItems = SubsceneDetailPage.Instance.GetLanguageTokenViewSelectedItems();
        return languageItems.Any(token => token.Content.ToString().Equals(Constants.ALL_FILTER))
            ? name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            : (name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)) &&
                (languageItems.Any(token => lang.Contains(token.Content.ToString())));
    }

    #endregion

    private async void OnNavigateToDetailsOrDownload(object sender)
    {
        base.NavigateToDetails(sender);

        if (Settings.IsSubtitleOpenInBrowser && !Settings.UseIDMForDownloade)
        {
            await Launcher.LaunchUriAsync(new Uri(descriptionText));
        }
        else
        {
            if (ApplicationHelper.IsNetworkAvailable())
            {
                try
                {
                    var web = new HtmlWeb();
                    var doc = await web.LoadFromWebAsync(descriptionText);

                    if (doc != null)
                    {
                        var node = doc.DocumentNode?.SelectSingleNode("//div[@class='download']//a");
                        if (node != null)
                        {
                            var downloadLink = GetServerUrlWithoutRightPart(descriptionText) + node.GetAttributeValue("href", "nothing");

                            // get location from config
                            var location = Settings.DefaultSubtitleDownloadPath;

                            // get location from FolderPicker
                            if (Settings.UseUserSpecifiedLocationForSubtitle)
                            {
                                var folderPickerPath = await OpenFolderPicker();
                                if (!string.IsNullOrEmpty(folderPickerPath))
                                {
                                    location = folderPickerPath;
                                }
                            }

                            if (!Settings.UseIDMForDownloade)
                            {
                                var downloader = new DownloadService();
                                downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
                                downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;
                                await downloader.DownloadFileTaskAsync(downloadLink, new DirectoryInfo(location));
                            }
                            else
                            {
                                Process.Start(GetIDMFilePath(), $"/d \"{downloadLink}\"");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    StatusTitle = "";
                    StatusMessage = ex.Message;
                    StatusSeverity = InfoBarSeverity.Error;
                    IsStatusOpen = true;
                }
            }
            else
            {
                StatusTitle = "";
                StatusMessage = Constants.InternetIsNotAvailable;
                StatusSeverity = InfoBarSeverity.Error;
                IsStatusOpen = true;
            }
        }
    }

    private void Downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Cancelled)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                StatusTitle = "Download Canceled";
                StatusMessage = "";
                StatusSeverity = InfoBarSeverity.Error;
                IsStatusOpen = true;
            });
        }
        else if (e.Error != null)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                StatusTitle = "";
                StatusMessage = e.Error.Message;
                StatusSeverity = InfoBarSeverity.Error;
                IsStatusOpen = true;
            });
        }
        else
        {
            //dispatcherQueue.TryEnqueue(() =>
            //{
            //    //var downloadedFileName = ((DownloadPackage) e.UserState).FileName;
            //    //DeCompressAndNotification(downloadedFileName, OpenFolderButton, Content.XamlRoot);
            //});
        }
    }

    private void Downloader_DownloadProgressChanged(object sender, Downloader.DownloadProgressChangedEventArgs e)
    {
        //dispatcherQueue.TryEnqueue(() =>
        //{
        //    //if (ProgressStatus.IsIndeterminate == true)
        //    //{
        //    //    ProgressStatus.IsIndeterminate = false;
        //    //}
        //    //ProgressStatus.Value = e.ProgressPercentage;
        //});
    }

    [RelayCommand]
    private void OnBreadCrumbBarItem(BreadcrumbBarItemClickedEventArgs args)
    {
        var item = (SubsceneModel) args.Item;
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
            StatusTitle = "Please Wait...";
            StatusMessage = "";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(tvTimeItem.Server);

            StatusMessage = $"Working on {tvTimeItem.Title}";

            StatusMessage = $"Parsing {tvTimeItem.Title}";

            var details = GetServerDetails(doc, tvTimeItem);
            DataList = new(details.Cast<ITvTimeModel>());
            DataListACV = new AdvancedCollectionView(DataList, true);
            currentSortDescription = new SortDescription("Title", SortDirection.Ascending);
            DataListACV.SortDescriptions.Add(currentSortDescription);
            suggestList = DataListACV.Select(x => ((SubsceneModel) x).Title).ToList();

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

    public List<SubsceneModel> GetServerDetails(HtmlDocument doc, ITvTimeModel tvTimeItem)
    {
        List<SubsceneModel> list = new List<SubsceneModel>();

        var table = doc.DocumentNode.SelectSingleNode("//table[1]//tbody");
        if (table != null)
        {
            foreach (var cell in table.SelectNodes(".//tr"))
            {
                if (cell.InnerText.Contains("There are no subtitles"))
                    break;

                var Language = cell.SelectSingleNode(".//td[@class='a1']//a//span[1]")?.InnerText.Trim();
                var Name = cell.SelectSingleNode(".//td[@class='a1']//a//span[2]")?.InnerText.Trim();
                var Translator = cell.SelectSingleNode(".//td[@class='a5']//a")?.InnerText.Trim();
                var Comment = cell.SelectSingleNode(".//td[@class='a6']//div")?.InnerText.Trim();
                if (Comment != null && Comment.Contains("&nbsp;")) Comment = Comment.Replace("&nbsp;", "");

                var Link = cell.SelectSingleNode(".//td[@class='a1']//a")?.Attributes["href"]?.Value.Trim();

                if (Name != null)
                {
                    var item = new SubsceneModel
                    {
                        Name = Name,
                        Title = Name,
                        Translator = Translator,
                        Desc = Comment,
                        Server = GetServerUrlWithoutRightPart(tvTimeItem.Server) + Link,
                        Language = Language
                    };
                    list.Add(item);
                }
            }
            return list;
        }
        else
        {
            IsActive = false;
            IsStatusOpen = true;
            StatusTitle = Constants.NotFoundOrExist;
            StatusSeverity = InfoBarSeverity.Error;
        }

        return null;
    }
}
