using Microsoft.UI.Xaml.Media;

namespace TvTime.ViewModels;
public partial class IMDBDetailsViewModel : ObservableRecipient
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
                var json = await response.Content.ReadFromJsonAsync<IMDBModel>();
                if (json.Response.Contains("true", StringComparison.OrdinalIgnoreCase))
                {
                    MediaIMDBId = new Uri(string.Format(Constants.IMDBBaseUrl, json.imdbID));
                    if (json.imdbRating.Contains("N/A") || string.IsNullOrEmpty(json.imdbRating))
                    {
                        MediaRateValue = 0;
                    }
                    else
                    {
                        MediaRateValue = Convert.ToDouble(json.imdbRating, CultureInfo.InvariantCulture);
                    }
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
        catch (Exception)
        {
            IsActive = false;
        }
    }
}
