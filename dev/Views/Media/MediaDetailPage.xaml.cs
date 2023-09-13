using TvTime.Database.Tables;

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
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var args = (BaseMediaTable) e.Parameter;
        ViewModel.rootMedia = args;
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
