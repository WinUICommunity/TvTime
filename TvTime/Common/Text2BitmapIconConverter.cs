using Microsoft.UI.Xaml.Data;

namespace TvTime.Common;
public class Text2BitmapIconConverter : IValueConverter
{
    private readonly string defaultIcon = "ms-appx:///Assets/Images/media.png";
    private readonly (string, string)[] _subtitles = new[]
    {
        ("softsub","ms-appx:///Assets/Images/softsub.png"),
        ("hardsub","ms-appx:///Assets/Images/hardsub.png"),
        ("srt","ms-appx:///Assets/Images/subtitle.png"),
        ("Dub","ms-appx:///Assets/Images/Sound.png"),
        ("Audio","ms-appx:///Assets/Images/Sound.png")
    };

    private readonly (string, string)[] _seasons = new[]
    {
        ("S01","\uF146"),
        ("S02","\uF147"),
        ("S03","\uF148"),
        ("S04","\uF149"),
        ("S05","\uF14A"),
        ("S06","\uF14B"),
        ("S07","\uF14C"),
        ("S08","\uF14D"),
        ("S09","\uF14E"),
        ("S10","\uF14F"),
        ("S11","\uF150"),
        ("S12","\uF151"),
        ("S13","\uF152"),
        ("S14","\uF153"),
        ("S15","\uF154"),
        ("S16","\uF155")
    };
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var text = value as string;
        if (!string.IsNullOrEmpty(text))
        {
            var subtitleType = _subtitles.FirstOrDefault(x => text.ToLower().Contains(x.Item1, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(subtitleType.Item2))
            {
                return new BitmapIcon { UriSource = new Uri(subtitleType.Item2), ShowAsMonochrome = false };
            }

            var seasonNumber = _seasons.FirstOrDefault(x => text.ToLower().StartsWith(x.Item1, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(seasonNumber.Item2))
            {
                return new FontIcon { Glyph = seasonNumber.Item2};
            }
        }
        return new BitmapIcon { UriSource = new Uri(defaultIcon), ShowAsMonochrome = false };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
