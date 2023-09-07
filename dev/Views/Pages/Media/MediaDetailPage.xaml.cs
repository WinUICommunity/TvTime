using TvTime.ViewModels;

namespace TvTime.Views;

public sealed partial class MediaDetailPage : Page
{
    public static MediaDetailPage Instance { get; set; }
    public MediaDetailsViewModel ViewModel { get; }
    public MediaDetailPage()
    {
        ViewModel = App.GetService<MediaDetailsViewModel>();
        this.InitializeComponent();
        Instance = this;
        MediaDetailItemsView.ItemTemplate = GetItemsViewDataTemplate(this);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var args = (MediaItem) e.Parameter;
        ViewModel.rootTvTimeItem = args;
        ViewModel.BreadcrumbBarList?.Clear();
    }

    private void ItemUserControl_Loading(FrameworkElement sender, object args)
    {
        var item = sender as ItemUserControl;
        item.ViewModel = ViewModel;
        item.SettingsCardCommand = ViewModel.SettingsCardCommand;
        item.SettingsCardDoubleClickCommand = ViewModel.SettingsCardDoubleClickCommand;
    }
}
