namespace TvTime.Views;
public sealed partial class MoviesPage : Page
{
    public MoviesPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (e.Content.GetType() != typeof(DetailPage))
        {
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }
    }
}
