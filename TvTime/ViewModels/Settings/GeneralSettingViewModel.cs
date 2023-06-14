namespace TvTime.ViewModels;
public partial class GeneralSettingViewModel : ObservableObject
{
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
}
