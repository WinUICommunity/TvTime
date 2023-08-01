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
        DataContext = this;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var args = (MediaItem) e.Parameter;
        ViewModel.rootTvTimeItem = args;
        ViewModel.BreadcrumbBarList?.Clear();
    }
}
