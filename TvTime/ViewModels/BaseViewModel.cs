using System.Diagnostics;
using System.Text;
using System.Windows.Input;

using CommunityToolkit.Labs.WinUI;

using Windows.ApplicationModel.DataTransfer;

namespace TvTime.ViewModels;
public partial class BaseViewModel : ObservableRecipient, IBaseViewModel
{
    [ObservableProperty]
    public ObservableCollection<ITvTimeModel> dataList;

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

    public List<string> suggestList = new();
    public List<string> existServer = new();
    public ITvTimeModel rootTvTimeItem = null;

    public SortDescription currentSortDescription;

    public ICommand MenuFlyoutItemCommand { get; }

    public string headerText = string.Empty;
    public string descriptionText = string.Empty;

    public BaseViewModel()
    {
        MenuFlyoutItemCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<object>(OnMenuFlyoutItem);
    }

    #region Virtual Methods

    /// <summary>
    /// Use base for the default implementation
    /// </summary>
    /// <param name="sender"></param>
    public virtual void OnMenuFlyoutItem(object sender)
    {
        var menuFlyout = (sender as MenuFlyoutItem);
        var tvTimeItem = (ITvTimeModel) menuFlyout?.DataContext;
        switch (menuFlyout?.Tag?.ToString())
        {
            case "OpenWebDirectory":
                OnOpenDirectory(tvTimeItem, menuFlyout);
                break;

            case "IMDB":
                OnGetIMDBDetails(tvTimeItem);
                break;

            case "OpenFile":
                OnOpenDirectory(tvTimeItem, menuFlyout);
                break;

            case "Copy":
                OnCopy(tvTimeItem);
                break;

            case "CopyAll":
                OnCopyAll();
                break;

            case "Download":
                OnDownload(tvTimeItem);
                break;

            case "DownloadAll":
                OnDownloadAll();
                break;
        }
    }

    /// <summary>
    /// Use base for the default implementation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public virtual void Search(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        AutoSuggestBoxHelper.LoadSuggestions(sender, args, suggestList);
        DataListACV.Filter = _ => true;
        DataListACV.Filter = DataListFilter;
    }

    public virtual bool DataListFilter(object item)
    {
        return false;
    }

    public virtual void OnRefresh()
    {

    }

    public virtual void OnDetail()
    {

    }

    /// <summary>
    /// Use base for the default implementation, Use two properties headerText and descriptionText
    /// </summary>
    /// <param name="sender"></param>
    public virtual void NavigateToDetails(object sender)
    {
        var item = sender as SettingsCard;
        var headerTextBlock = item?.Header as HeaderTextBlockUserControl;
        var title = headerTextBlock?.Text?.Trim();
        var server = string.Empty;
        switch (Settings.DescriptionTemplate)
        {
            case DescriptionTemplateType.TextBlock:
                var descriptionTextBlock = item?.Description as DescriptionTextBlockUserControl;
                server = descriptionTextBlock?.Text;
                break;
            case DescriptionTemplateType.HyperLink:
                var descriptionHyperLink = item?.Description as DescriptionHyperLinkUserControl;
                var hyperLink = descriptionHyperLink?.Content as HyperlinkButton;
                var hyperLinkContent = hyperLink?.Content as TextBlock;
                server = hyperLinkContent?.Text;
                break;
        }

        headerText = title;
        descriptionText = server;
    }

    [RelayCommand]
    public virtual void OnPageLoaded(object param)
    {

    }

    #endregion

    private async void OnOpenDirectory(ITvTimeModel tvTimeItem, MenuFlyoutItem item)
    {
        var server = tvTimeItem.Server?.ToString();
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

    private void OnGetIMDBDetails(ITvTimeModel tvTimeItem)
    {
        if (rootTvTimeItem == null)
        {
            CreateIMDBDetailsWindow(tvTimeItem.Title);
        }
        else
        {
            CreateIMDBDetailsWindow(rootTvTimeItem.Title);
        }
    }

    private void OnCopy(ITvTimeModel tvTimeItem)
    {
        var server = tvTimeItem.Server?.ToString();
        var package = new DataPackage();
        package.SetText(server);
        Clipboard.SetContent(package);
    }

    private void OnCopyAll()
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

    private async void OnDownload(ITvTimeModel tvTimeItem)
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
            var server = tvTimeItem.Server?.ToString();
            Process.Start(GetIDMFilePath(), $"/d \"{server?.ToString()}\"");
        }
    }

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
                Process.Start(GetIDMFilePath(), $"/d \"{item.Server?.ToString()}\"");
                await Task.Delay(500);
            }
        }
    }

    [RelayCommand]
    public void OnSegmentedItemChanged(object sender)
    {
        var segmented = sender as Segmented;
        var selectedItem = segmented.SelectedItem as SegmentedItem;
        if (selectedItem != null)
        {
            switch (selectedItem.Tag?.ToString())
            {
                case "Refresh":
                    OnRefresh();
                    segmented.SelectedIndex = -1;
                    break;
                case "Details":
                    OnDetail();
                    segmented.SelectedIndex = -1;
                    break;
            }
        }
    }

    [RelayCommand]
    private void OnSettingsCard(object sender)
    {
        if (!Settings.UseDoubleClickForNavigate)
        {
            NavigateToDetails(sender);
        }
    }

    [RelayCommand]
    private void OnSettingsCardDoubleClick(object sender)
    {
        NavigateToDetails(sender);
    }

    public string GetIDMFilePath()
    {
        string idmPathX86 = @"C:\Program Files (x86)\Internet Download Manager\IDMan.exe"; // Update with the correct path to IDM executable
        string idmPathX64 = @"C:\Program Files\Internet Download Manager\IDMan.exe"; // Update with the correct path to IDM executable
        return File.Exists(idmPathX64) ? idmPathX64 : File.Exists(idmPathX86) ? idmPathX86 : null;
    }
}
