namespace TvTime.ViewModels;
public partial class HeaderStyleSettingViewModel : ObservableRecipient
{
    [ObservableProperty]
    public double previewFontSize = GetFontSizeBasedOnTextBlockStyle(Settings.SettingsCardHeaderStyle);

    [ObservableProperty]
    public object cmbSelectedItem;

    [ObservableProperty]
    public bool isEnabledSettingsCard = Settings.UseHeaderCustomFontSize;

    [ObservableProperty]
    public ObservableCollection<string> textBlockStyles = GenerateTextBlockStyles();

    [RelayCommand]
    private void OnComboBoxChanged()
    {
        if (CmbSelectedItem != null)
        {
            Settings.SettingsCardHeaderStyle = CmbSelectedItem.ToString();

            if (Settings.UseHeaderCustomFontSize && !double.IsNaN(Settings.SettingsCardHeaderFontSize))
            {
                PreviewFontSize = Settings.SettingsCardHeaderFontSize;
            }
            else
            {
                PreviewFontSize = GetFontSizeBasedOnTextBlockStyle(CmbSelectedItem.ToString());
            }
        }
    }

    [RelayCommand]
    private void OnCustomFontSizeToggled(object sender)
    {
        var tg = sender as ToggleSwitch;
        if (tg != null)
        {
            IsEnabledSettingsCard = tg.IsOn;
            if (tg.IsOn && !double.IsNaN(Settings.SettingsCardHeaderFontSize))
            {
                PreviewFontSize = Settings.SettingsCardHeaderFontSize;
            }
            else
            {
                PreviewFontSize = GetFontSizeBasedOnTextBlockStyle(CmbSelectedItem?.ToString());
                Settings.SettingsCardHeaderFontSize = PreviewFontSize;
            }
        }
    }
}
