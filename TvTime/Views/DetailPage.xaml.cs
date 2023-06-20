using TvTime.ViewModels;

namespace TvTime.Views;

public sealed partial class DetailPage : Page
{
    public static DetailPage Instance { get; set; }
    public DetailsViewModel ViewModel { get; }
    public DetailPage()
    {
        ViewModel = App.Current.Services.GetService<DetailsViewModel>();
        this.InitializeComponent();
        Instance = this;
        DataContext = this;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var args = e.Parameter as NavigationArgs;
        var item = (MediaItem)args.Parameter;
        ViewModel.rootLocalItem = item;
        ViewModel.BreadcrumbBarList?.Clear();
    }
}
