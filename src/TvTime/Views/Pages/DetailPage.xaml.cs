using TvTime.ViewModels;

namespace TvTime.Views;

public sealed partial class DetailPage : Page
{
    public static DetailPage Instance { get; set; }
    public DetailsViewModel ViewModel { get; }
    public DetailPage()
    {
        ViewModel = App.GetService<DetailsViewModel>();
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
