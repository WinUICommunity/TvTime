using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class ServersPage : Page
{
    public static ServersPage Instance { get; private set; }
    public ServerViewModel ViewModel { get; }
    public ServersPage()
    {
        ViewModel = App.Current.Services.GetService<ServerViewModel>();
        this.InitializeComponent();
        Instance = this;
        DataContext = this;
    }
}
