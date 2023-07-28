using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class SubscenePage : Page
{
    public static SubscenePage Instance { get; private set; }
    public SubsceneViewModel ViewModel { get; set; }
    public SubscenePage()
    {
        ViewModel = App.GetService<SubsceneViewModel>();
        this.InitializeComponent();
        Instance = this;
        DataContext = this;
    }
}
