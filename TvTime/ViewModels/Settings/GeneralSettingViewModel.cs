namespace TvTime.ViewModels;
public partial class GeneralSettingViewModel : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<string> textBlockStyles = new ObservableCollection<string>
    {
        "BaseTextBlockStyle",
        "BodyStrongTextBlockStyle",
        "BodyTextBlockStyle", 
        "CaptionTextBlockStyle", 
        "DisplayTextBlockStyle", 
        "HeaderTextBlockStyle", 
        "SubheaderTextBlockStyle", 
        "SubtitleTextBlockStyle", 
        "TitleLargeTextBlockStyle", 
        "TitleTextBlockStyle"
    };

    [RelayCommand]
    private void OnIconPackChanged(object sender)
    {
        var cmbIconPack = sender as ComboBox;
        if (cmbIconPack != null)
        {
            var selectedItem = cmbIconPack.SelectedItem as ComboBoxItem;
            var iconPack = ApplicationHelper.GetEnum<IconPackType>(selectedItem?.Tag?.ToString());
            Settings.IconPack = iconPack;
        }
    }

    [RelayCommand]
    private void OnDescriptionTypeChanged(object sender)
    {
        var cmbDescriptionType = sender as ComboBox;
        if (cmbDescriptionType != null)
        {
            var selectedItem = cmbDescriptionType.SelectedItem as ComboBoxItem;
            var descType = ApplicationHelper.GetEnum<DescriptionType>(selectedItem?.Tag?.ToString());
            Settings.DescriptionType = descType;
        }
    }

    [RelayCommand]
    private void OnHeaderStyleChanged(object sender)
    {
        var cmbHeaderStyle = sender as ComboBox;
        if (cmbHeaderStyle != null)
        {
            var selectedItem = cmbHeaderStyle.SelectedItem?.ToString();
            Settings.SettingsCardHeaderStyle = selectedItem;
        }
    }

    [RelayCommand]
    private void OnDescriptionStyleChanged(object sender)
    {
        var cmbDescriptionStyle = sender as ComboBox;
        if (cmbDescriptionStyle != null)
        {
            var selectedItem = cmbDescriptionStyle.SelectedItem?.ToString();
            Settings.SettingsCardDescriptionStyle = selectedItem;
        }
    }
}
