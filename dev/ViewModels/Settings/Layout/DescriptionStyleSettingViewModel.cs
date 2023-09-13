namespace TvTime.ViewModels;
public partial class DescriptionStyleSettingViewModel : ObservableRecipient
{
    [ObservableProperty]
    public object cmbStyleSelectedItem;

    [ObservableProperty]
    public ObservableCollection<string> textBlockStyles = GenerateTextBlockStyles();

    [RelayCommand]
    private void OnComboBoxTextBlockStyleChanged()
    {
        if (CmbStyleSelectedItem != null)
        {
            Settings.DescriptionTextBlockStyle = CmbStyleSelectedItem.ToString();
        }
    }
}
