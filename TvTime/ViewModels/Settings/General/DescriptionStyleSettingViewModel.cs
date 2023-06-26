namespace TvTime.ViewModels;
public partial class DescriptionStyleSettingViewModel : ObservableRecipient
{
    [ObservableProperty]
    public double previewFontSize = GetFontSizeBasedOnTextBlockStyle(Settings.DescriptionTextBlockStyle);

    [ObservableProperty]
    public object cmbStyleSelectedItem;

    [ObservableProperty]
    public bool isEnabledSettingsCard = Settings.UseCustomFontSizeForDescription;

    [ObservableProperty]
    public bool isHyperLink = Settings.DescriptionTemplate == DescriptionTemplateType.HyperLink;

    [ObservableProperty]
    public bool isTextBlock = Settings.DescriptionTemplate == DescriptionTemplateType.TextBlock;

    [ObservableProperty]
    public ObservableCollection<string> textBlockStyles = GenerateTextBlockStyles();

    [RelayCommand]
    private void OnComboBoxTemplateTypeChanged(object sender)
    {
        var cmb = sender as ComboBox;
        if (cmb != null)
        {
            var selectedItem = cmb.SelectedItem as ComboBoxItem;
            var type = ApplicationHelper.GetEnum<DescriptionTemplateType>(selectedItem?.Tag?.ToString());
            Settings.DescriptionTemplate = type;
            IsHyperLink = type == DescriptionTemplateType.HyperLink;
            IsTextBlock = type == DescriptionTemplateType.TextBlock;
        }
    }

    [RelayCommand]
    private void OnComboBoxTextBlockStyleChanged()
    {
        if (CmbStyleSelectedItem != null)
        {
            Settings.DescriptionTextBlockStyle = CmbStyleSelectedItem.ToString();

            PreviewFontSize = Settings.UseCustomFontSizeForDescription && !double.IsNaN(Settings.DescriptionTextBlockFontSize)
                ? Settings.DescriptionTextBlockFontSize
                : GetFontSizeBasedOnTextBlockStyle(CmbStyleSelectedItem.ToString());
        }
    }

    [RelayCommand]
    private void OnCustomFontSizeToggled(object sender)
    {
        var tg = sender as ToggleSwitch;
        if (tg != null)
        {
            IsEnabledSettingsCard = tg.IsOn;
            if (tg.IsOn && !double.IsNaN(Settings.DescriptionTextBlockFontSize))
            {
                PreviewFontSize = Settings.DescriptionTextBlockFontSize;
            }
            else
            {
                PreviewFontSize = GetFontSizeBasedOnTextBlockStyle(CmbStyleSelectedItem?.ToString());
                Settings.DescriptionTextBlockFontSize = PreviewFontSize;
            }
        }
    }
}
