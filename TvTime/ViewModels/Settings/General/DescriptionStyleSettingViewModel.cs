namespace TvTime.ViewModels;
public partial class DescriptionStyleSettingViewModel : ObservableRecipient
{
    [ObservableProperty]
    public double previewFontSize = GetFontSizeBasedOnTextBlockStyle(Settings.SettingsCardDescriptionStyle);

    [ObservableProperty]
    public object cmbSelectedItem;

    [ObservableProperty]
    public bool isEnabledSettingsCard = Settings.UseDescriptionCustomFontSize;

    [ObservableProperty]
    public bool isHyperLink = Settings.DescriptionType == DescriptionType.HyperLink;

    [ObservableProperty]
    public bool isTextBlock = Settings.DescriptionType == DescriptionType.TextBlock;

    [ObservableProperty]
    public ObservableCollection<string> textBlockStyles = GenerateTextBlockStyles();

    [RelayCommand]
    private void OnComboBoxTypeChanged(object sender)
    {
        var cmbDescriptionType = sender as ComboBox;
        if (cmbDescriptionType != null)
        {
            var selectedItem = cmbDescriptionType.SelectedItem as ComboBoxItem;
            var descType = ApplicationHelper.GetEnum<DescriptionType>(selectedItem?.Tag?.ToString());
            Settings.DescriptionType = descType;
            IsHyperLink = descType == DescriptionType.HyperLink;
            IsTextBlock = descType == DescriptionType.TextBlock;
        }
    }

    [RelayCommand]
    private void OnComboBoxStyleChanged()
    {
        if (CmbSelectedItem != null)
        {
            Settings.SettingsCardDescriptionStyle = CmbSelectedItem.ToString();

            if (Settings.UseDescriptionCustomFontSize && !double.IsNaN(Settings.SettingsCardDescriptionFontSize))
            {
                PreviewFontSize = Settings.SettingsCardDescriptionFontSize;
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
            if (tg.IsOn && !double.IsNaN(Settings.SettingsCardDescriptionFontSize))
            {
                PreviewFontSize = Settings.SettingsCardDescriptionFontSize;
            }
            else
            {
                PreviewFontSize = GetFontSizeBasedOnTextBlockStyle(CmbSelectedItem?.ToString());
                Settings.SettingsCardDescriptionFontSize = PreviewFontSize;
            }
        }
    }
}
