namespace TvTime.ViewModels;
public partial class AppLanguageSettingViewModel : ObservableRecipient
{
    [ObservableProperty]
    public ObservableCollection<TvTimeLanguage> languages = TvTimeLanguagesCollection();

    [RelayCommand]
    private void OnLanguageChanged(object sender)
    {
        var cmb = sender as ComboBox;
        if (cmb != null)
        {
            var selectedItem = cmb.SelectedItem as TvTimeLanguage;
            if (selectedItem != null)
            {
                Settings.TvTimeLanguage = selectedItem;
                App.Current.Localizer.SetLanguage(selectedItem.LanguageCode);
                MainPage.Instance.SetFlowDirection();
            }
        }
    }
}
