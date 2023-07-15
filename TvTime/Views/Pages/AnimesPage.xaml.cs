namespace TvTime.Views;
public sealed partial class AnimesPage : Page
{
    public AnimesPage()
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
