using Microsoft.UI.Xaml.Media;

namespace TvTime.ViewModels;
public partial class MainViewModel : ObservableObject
{
    public IJsonNavigationViewService JsonNavigationViewService;
    public MainViewModel(IJsonNavigationViewService jsonNavigationViewService, IThemeService themeService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
        themeService.Initialize(App.currentWindow);
        themeService.ConfigBackdrop();
        themeService.ConfigElementTheme();
        themeService.ConfigBackdropFallBackColorForUnSupportedOS(Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush);
    }
}
