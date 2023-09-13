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
    }

    private void ItemUserControl_Loading(FrameworkElement sender, object args)
    {
        var item = sender as ItemUserControl;
        item.ViewModel = ViewModel;
        item.SettingsCardCommand = ViewModel.SettingsCardCommand;
        item.SettingsCardDoubleClickCommand = ViewModel.SettingsCardDoubleClickCommand;
    }
}
