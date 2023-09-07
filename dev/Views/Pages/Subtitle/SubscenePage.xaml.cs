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
        SubsceneItemsView.ItemTemplate = GetItemsViewDataTemplate(this);
    }

    private void ItemUserControl_Loading(FrameworkElement sender, object args)
    {
        var item = sender as ItemUserControl;
        item.ViewModel = ViewModel;
        item.SettingsCardCommand = ViewModel.SettingsCardCommand;
        item.SettingsCardDoubleClickCommand = ViewModel.SettingsCardDoubleClickCommand;
    }
}
