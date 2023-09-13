namespace TvTime.Views;

public sealed partial class IMDBDetailPage : Page
{
    public static IMDBDetailPage Instance { get; set; }
    public IMDBDetailViewModel ViewModel { get; }
    public IMDBDetailPage()
    {
        ViewModel = App.GetService<IMDBDetailViewModel>();
        this.InitializeComponent();
        Instance = this;
    }
}
