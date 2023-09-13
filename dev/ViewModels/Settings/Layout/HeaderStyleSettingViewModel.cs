namespace TvTime.ViewModels;
public partial class HeaderStyleSettingViewModel : ObservableRecipient
{
    [ObservableProperty]
    public object cmbSelectedItem;

    [ObservableProperty]
    public ObservableCollection<string> textBlockStyles = GenerateTextBlockStyles();

    [RelayCommand]
    private void OnComboBoxChanged()
    {
        if (CmbSelectedItem != null)
        {
            Settings.HeaderTextBlockStyle = CmbSelectedItem.ToString();
        }
    }
}
