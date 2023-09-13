using System.Text;

using CommunityToolkit.WinUI.UI;

using TvTime.Database.Tables;
using TvTime.Views.ContentDialogs;

using Windows.ApplicationModel.DataTransfer;

using Windows.System;

namespace TvTime.ViewModels;
public partial class BaseViewModel : ObservableRecipient
{
    [ObservableProperty]
    public ObservableCollection<BaseMediaTable> dataList;

    [ObservableProperty]
    public AdvancedCollectionView dataListACV;

    [ObservableProperty]
    public bool isStatusOpen;

    [ObservableProperty]
    public string statusMessage;

    [ObservableProperty]
    public string statusTitle;

    [ObservableProperty]
    public InfoBarSeverity statusSeverity;

    public BaseMediaTable rootMedia = null;

    public string headerText = string.Empty;
    public string descriptionText = string.Empty;

    #region MenuFlyoutItems
    [RelayCommand]
    private async Task OnOpenWebDirectory(object item)
    {
        try
        {
            var menuItem = item as MenuFlyoutItem;
            var media = menuItem.DataContext as BaseMediaTable;
            var server = media.Server?.ToString();
            if (menuItem.Text.Contains("File"))
            {
                await Launcher.LaunchUriAsync(new Uri(server));
            }
            else
            {
                if (Constants.FileExtensions.Any(server.Contains))
                {
                    var fileName = Path.GetFileName(server);
                    await Launcher.LaunchUriAsync(new Uri(server.Replace(fileName, "")));
                }
                else
                {
                    await Launcher.LaunchUriAsync(new Uri(server));
                }
            }
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, "BaseViewModel: OnOpenWebDirectory");
        }
    }

    [RelayCommand]
    private void OnGetIMDBDetail(object baseMedia)
    {
        var media = baseMedia as BaseMediaTable;
        if (rootMedia == null)
        {
            CreateIMDBDetailsWindow(media.Title);
        }
        else
        {
            CreateIMDBDetailsWindow(rootMedia.Title);
        }
    }

    [RelayCommand]
    private void OnCopy(object baseMedia)
    {
        var media = baseMedia as BaseMediaTable;
        var server = media.Server?.ToString();
        var package = new DataPackage();
        package.SetText(server);
        Clipboard.SetContent(package);
    }

    [RelayCommand]
    private void OnCopyAll()
    {
        if (DataList != null)
        {
            var package = new DataPackage();
            StringBuilder urls = new StringBuilder();
            foreach (var item in DataList)
            {
                urls.AppendLine(item.Server?.ToString());
            }
            package.SetText(urls?.ToString());
            Clipboard.SetContent(package);
        }
    }

    [RelayCommand]
    private async Task OnDownloadWithIDM(object baseMedia)
    {
        var idmPath = GetIDMFilePath();
        if (string.IsNullOrEmpty(idmPath))
        {
            var dialog = new IDMNotFoundDialog();
            await dialog.ShowAsync();
        }
        else
        {
            var media = baseMedia as BaseMediaTable;
            var server = media.Server?.ToString();
            LaunchIDM(GetIDMFilePath(), server?.ToString());
        }
    }

    [RelayCommand]
    private async Task OnDownloadAllWithIDM()
    {
        var idmPath = GetIDMFilePath();
        if (string.IsNullOrEmpty(idmPath))
        {
            var dialog = new IDMNotFoundDialog();
            await dialog.ShowAsync();
        }
        else
        {
            if (DataList != null)
            {
                foreach (var item in DataList)
                {
                    if (IsUrlFile(item?.Server))
                    {
                        LaunchIDM(GetIDMFilePath(), item?.Server?.ToString());
                        await Task.Delay(700);
                    }
                }
            }
        }
    }

    #endregion

    public string GetIDMFilePath()
    {
        string idmPathX86 = @"C:\Program Files (x86)\Internet Download Manager\IDMan.exe";
        string idmPathX64 = @"C:\Program Files\Internet Download Manager\IDMan.exe";
        return File.Exists(idmPathX64) ? idmPathX64 : File.Exists(idmPathX86) ? idmPathX86 : null;
    }

    [RelayCommand]
    public virtual void OnPageLoaded(object param)
    {

    }

    /// <summary>
    /// Use base for the default implementation, Use two properties headerText and descriptionText
    /// </summary>
    /// <param name="sender"></param>
    public virtual void NavigateToDetails(object sender)
    {
        var item = GetHeaderAndDescription(sender);

        headerText = item.Header;
        descriptionText = item.Description;
    }

    [RelayCommand]
    public virtual void OnSettingsCard(object sender)
    {
        if (!Settings.UseDoubleClickForNavigate)
        {
            NavigateToDetails(sender);
        }
    }

    [RelayCommand]
    public virtual void OnSettingsCardDoubleClick(object sender)
    {
        NavigateToDetails(sender);
    }

    [RelayCommand]
    public virtual void OnRefresh()
    {

    }

    [RelayCommand]
    public virtual void OnIMDBDetail()
    {

    }

    /// <summary>
    /// Use base for the default implementation, all items will be opened in IDM if UseIDMForDownloadFiles option is enabled in General Settings Page, Otherwise, they will open in the browser
    /// </summary>
    [RelayCommand]
    public async virtual Task OnDownloadAllWithIDMOrOpenInBrowser()
    {
        if (Settings.UseIDMForDownloadFiles)
        {
           await OnDownloadAllWithIDM();
        }
        else
        {
            foreach (var item in DataList)
            {
                await Launcher.LaunchUriAsync(new Uri(item?.Server));
            }
        }
    }

    public (string Header, string Description) GetHeaderAndDescription(object sender)
    {
        var item = sender as SettingsCard;
        var headerTextBlock = item?.Header as TextBlock;
        var title = headerTextBlock?.Text?.Trim();
        var server = string.Empty;
        var descriptionHyperLink = item?.Description as HyperlinkButton;
        var hyperLinkContent = descriptionHyperLink?.Content as TextBlock;
        server = hyperLinkContent?.Text;

        return (Header: title, Description: server);
    }
}
