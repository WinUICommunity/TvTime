namespace TvTime.Views;

public sealed partial class BoxOfficePage : Page
{
    public BoxOfficeViewModel ViewModel { get; set; }
    public BoxOfficePage()
    {
        ViewModel = App.GetService<BoxOfficeViewModel>();
        this.InitializeComponent();
    }
}
