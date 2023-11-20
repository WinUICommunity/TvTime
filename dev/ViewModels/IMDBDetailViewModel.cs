using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using System.Globalization;
using System.Net.Http.Json;
using System.Net;

namespace TvTime.ViewModels;
public partial class IMDBDetailViewModel : ObservableRecipient, ITitleBarAutoSuggestBoxAware
{
    [ObservableProperty]
    public ImageSource mediaCover;

    [ObservableProperty]
    public Uri mediaIMDBId;

    [ObservableProperty]
    public double mediaRateValue;

    [ObservableProperty]
    public string mediaTitle;

    [ObservableProperty]
    public string mediaYear;

    [ObservableProperty]
    public string mediaReleased;

    [ObservableProperty]
    public string mediaType;

    [ObservableProperty]
    public string mediaTotalSeason;

    [ObservableProperty]
    public string mediaLanguage;

    [ObservableProperty]
    public string mediaCountry;

    [ObservableProperty]
    public string mediaRated;

    [ObservableProperty]
    public string mediaGener;

    [ObservableProperty]
    public string mediaDirector;

    [ObservableProperty]
    public string mediaWriter;

    [ObservableProperty]
    public string mediaActors;

    [ObservableProperty]
    public string mediaPlot;

    [ObservableProperty]
    public string query;

    public void setQuery(string query)
    {
        this.Query = query;
    }

    public void OnQuerySubmitted()
    {
        GetIMDBDetails(Query);
    }

    [RelayCommand]
    private void OnGridLoaded()
    {
        GetIMDBDetails(Query);
    }

    private async void GetIMDBDetails(string title)
    {
        try
        {
            if (NetworkHelper.IsNetworkAvailable())
            {
                IsActive = false;

                MediaCover = null;
                var url = string.Format(Constants.IMDBTitleAPI, title);
                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.TooManyRequests || response.StatusCode == HttpStatusCode.ServiceUnavailable || response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return;
                }
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<IMDBDetail>();
                    if (json.Response.Contains("true", StringComparison.OrdinalIgnoreCase))
                    {
                        MediaIMDBId = new Uri(string.Format(Constants.IMDBBaseUrl, json.imdbID));
                        MediaRateValue = json.imdbRating.Contains("N/A") || string.IsNullOrEmpty(json.imdbRating)
                            ? 0
                            : Convert.ToDouble(json.imdbRating, CultureInfo.InvariantCulture);
                        MediaTitle = json.Title;
                        MediaYear = json.Year;
                        MediaReleased = json.Released;
                        MediaType = json.Type;
                        MediaTotalSeason = json.totalSeasons;
                        MediaLanguage = json.Language;
                        MediaCountry = json.Country;
                        MediaRated = json.Rated;
                        MediaGener = json.Genre;
                        MediaDirector = json.Director;
                        MediaWriter = json.Writer;
                        MediaActors = json.Actors;
                        MediaPlot = json.Plot;
                        if (!json.Poster.Contains("N/A"))
                        {
                            MediaCover = new BitmapImage(new Uri(json.Poster));
                        }
                        IsActive = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            IsActive = false;
            Logger?.Error(ex, "IMDBDetailViewModel: GetIMDBDetails");
        }
    }

    public void OnAutoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        Search(sender.Text);
    }

    public void OnAutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Search(sender.Text);
    }

    private void Search(string query)
    {
        setQuery(query);
        OnQuerySubmitted();
    }
}
