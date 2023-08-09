namespace TvTime.ViewModels;
public partial class AboutUsSettingViewModel : ObservableObject
{
    [ObservableProperty]
    public string tvTimeVersion = $"TvTime v{App.Current.TvTimeVersion}";
}
