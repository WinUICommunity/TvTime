using Microsoft.UI.Xaml.Media;

namespace TvTime.ViewModels;
public partial class MainViewModel : ObservableObject
{
    public IJsonNavigationViewService JsonNavigationViewService;
    public MainViewModel(IJsonNavigationViewService jsonNavigationViewService, IThemeService themeService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
        themeService.Initialize(App.currentWindow);
        themeService.ConfigBackdrop(BackdropType.Mica);
        themeService.ConfigElementTheme(ElementTheme.Default);
        themeService.ConfigBackdropFallBackColorForWindow10(Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush);
    }
}
