namespace TvTime.Views;

public sealed partial class BoxOfficeDetailPage : Page
{
    public BoxOfficeDetailViewModel ViewModel { get; set; }
    public BoxOfficeDetailPage()
    {
        ViewModel = App.GetService<BoxOfficeDetailViewModel>();
        this.InitializeComponent();
    }
}
