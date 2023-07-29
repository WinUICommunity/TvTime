namespace TvTime.ViewModels;
public partial class AppLanguageSettingViewModel : ObservableRecipient
{
    [ObservableProperty]
    public ObservableCollection<TvTimeLanguage> languages = TvTimeLanguagesCollection();

    [RelayCommand]
    private void OnLanguageChanged(object sender)
    {
        
    }
}
