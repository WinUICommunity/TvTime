using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
            var iconPack = ApplicationHelper.GetEnum<IconPack>(selectedItem?.Tag?.ToString());
            Settings.IconPack = iconPack;
        }
    }
}
