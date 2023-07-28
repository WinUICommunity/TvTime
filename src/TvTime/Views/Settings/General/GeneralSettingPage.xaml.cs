using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class GeneralSettingPage : Page
{
    public GeneralSettingViewModel ViewModel { get; }
    public static GeneralSettingPage Instance { get; set; }
    public GeneralSettingPage()
    {
        ViewModel = App.GetService<GeneralSettingViewModel>();
        this.InitializeComponent();
        Instance = this;
        Loaded += GeneralSettingPage_Loaded;
    }

    private void GeneralSettingPage_Loaded(object sender, RoutedEventArgs e)
    {
        var iconPack = Settings.IconPack;
        CmbIconPack.SelectedItem = CmbIconPack.Items.FirstOrDefault(x => ((ComboBoxItem) x).Tag.ToString() == iconPack.ToString());
    }

    private void Sound_Toggled(object sender, RoutedEventArgs e)
    {
        ElementSoundPlayer.State = TGSound.IsOn ? ElementSoundPlayerState.On : ElementSoundPlayerState.Off;
    }

    private void TGRegex_Toggled(object sender, RoutedEventArgs e)
    {
        if (TGRegex.IsOn)
        {
            Settings.FileNameRegex = Constants.FileNameRegex;
        }
    }
}
