namespace TvTime.Common;

public class PageType2BitmapIconConverter : IValueConverter
{
    private readonly (string, string)[] _viewTypes = new[]
    {
        ("Series", "ms-appx:///Assets/Fluent/series.png"),
        ("Movie", "ms-appx:///Assets/Fluent/movie.png"),
        ("Anime", "ms-appx:///Assets/Fluent/anime.png")
    };
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var pageType = ((PageOrDirectoryType) value).ToString();
        if (!string.IsNullOrEmpty(pageType))
        {
            var type = _viewTypes.FirstOrDefault(x => x.Item1.Equals(pageType, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(type.Item2))
            {
                return new BitmapIcon { UriSource = new Uri(type.Item2), ShowAsMonochrome = false };
            }
        }
        return new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Fluent/series.png"), ShowAsMonochrome = false };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
