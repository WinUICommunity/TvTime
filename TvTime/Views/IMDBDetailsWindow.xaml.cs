namespace TvTime.Views;

public sealed partial class IMDBDetailsWindow : Window
{
    public IMDBDetailsWindow()
    {
        this.InitializeComponent();
    }

    private async void GetIMDBDetails(string title)
    {
        try
        {
            this.Title = title;
            Cover.Source = null;

            var url = string.Format(Constants.IMDBTitleAPI, title);

            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.TooManyRequests || response.StatusCode == HttpStatusCode.ServiceUnavailable || response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return;
            }
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<IMDBModel>();
                if (json.Response.Contains("true", StringComparison.OrdinalIgnoreCase))
                {
                    txtImdbId.Text = string.Format(Constants.IMDBBaseUrl, json.imdbID);
                    if (json.imdbRating.Contains("N/A") || string.IsNullOrEmpty(json.imdbRating))
                    {
                        rate.Value = 0;
                    }
                    else
                    {
                        rate.Value = Convert.ToDouble(json.imdbRating, CultureInfo.InvariantCulture);
                    }
                    txtTitle.Text = json.Title;
                    txtYear.Text = json.Year;
                    txtReleased.Text = json.Released;
                    txtType.Text = json.Type;
                    txtTotalSeason.Text = json.totalSeasons;
                    txtLanguage.Text = json.Language;
                    txtCountry.Text = json.Country;
                    txtRated.Text = json.Rated;
                    txtGenre.Text = json.Genre;
                    txtDirector.Text = json.Director;
                    txtWriter.Text = json.Writer;
                    txtActors.Text = json.Actors;
                    txtPlot.Text = json.Plot;
                    if (!json.Poster.Contains("N/A"))
                    {
                        Cover.Source = new BitmapImage(new Uri(json.Poster));
                    }
                    InfoPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    InfoPanel.Visibility = Visibility.Collapsed;
                }
            }
        }
        catch (Exception)
        {
            InfoPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void txtImdbDetail_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        GetIMDBDetails(txtImdbDetail.Text);
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        GetIMDBDetails(Title);
        txtImdbDetail.Text = Title;
        var titlebar = new TitleBarHelper(this, TitleTextBlock, AppTitleBar, LeftPaddingColumn, IconColumn, TitleColumn, LeftDragColumn, SearchColumn, RightDragColumn, RightPaddingColumn);
        titlebar.RightPadding = -100;
    }
}
