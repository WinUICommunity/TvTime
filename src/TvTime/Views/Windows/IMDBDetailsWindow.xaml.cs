namespace TvTime.Views;

public sealed partial class IMDBDetailsWindow : Window
{
    public IMDBDetailsWindow(string query)
    {
        this.InitializeComponent();
        appTitleBar.Window = this;
        this.AppWindow.SetIcon("Assets/Fluent/icon.ico");
        TxtSearch.Text = query;
        GetDetails();
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        GetDetails();
    }

    private void GetDetails()
    {
        ImdbDetailsPage?.ViewModel?.setQuery(TxtSearch.Text);
        ImdbDetailsPage?.ViewModel?.OnQuerySubmitted();
        this.Title = TxtSearch.Text;
        appTitleBar.Title = $"TvTime v{App.Current.TvTimeVersion} - {TxtSearch.Text}";
    }
}
