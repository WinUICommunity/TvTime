namespace TvTime.ViewModels;
public partial class HeaderStyleSettingViewModel : ObservableRecipient
{
    [ObservableProperty]
    public double previewFontSize = GetFontSizeBasedOnTextBlockStyle(Settings.HeaderTextBlockStyle);

    [ObservableProperty]
    public object cmbSelectedItem;

    [ObservableProperty]
    public bool isEnabledSettingsCard = Settings.UseCustomFontSizeForHeader;

    [ObservableProperty]
    public ObservableCollection<string> textBlockStyles = GenerateTextBlockStyles();

    [RelayCommand]
    private void OnComboBoxChanged()
    {
        if (CmbSelectedItem != null)
        {
            Settings.HeaderTextBlockStyle = CmbSelectedItem.ToString();

            PreviewFontSize = Settings.UseCustomFontSizeForHeader && !double.IsNaN(Settings.HeaderTextBlockFontSize)
                ? Settings.HeaderTextBlockFontSize
                : GetFontSizeBasedOnTextBlockStyle(CmbSelectedItem.ToString());
        }
    }

    [RelayCommand]
    private void OnCustomFontSizeToggled(object sender)
    {
        var tg = sender as ToggleSwitch;
        if (tg != null)
        {
            IsEnabledSettingsCard = tg.IsOn;
            if (tg.IsOn && !double.IsNaN(Settings.HeaderTextBlockFontSize))
            {
                PreviewFontSize = Settings.HeaderTextBlockFontSize;
            }
            else
            {
                PreviewFontSize = GetFontSizeBasedOnTextBlockStyle(CmbSelectedItem?.ToString());
                Settings.HeaderTextBlockFontSize = PreviewFontSize;
            }
        }
    }
}
