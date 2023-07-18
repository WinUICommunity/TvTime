namespace TvTime.Views;
public sealed partial class SeriesPage : Page
{
    public SeriesPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (e.Content.GetType() != typeof(DetailPage))
        {
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            MainPage.Instance.ClearTxtSearch();
        }
    }
}
