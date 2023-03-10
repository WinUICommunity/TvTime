using Microsoft.UI.Xaml.Data;

namespace TvTime.Common;
public class Text2BitmapIconConverter : IValueConverter
{
    private readonly string defaultIcon = "ms-appx:///Assets/Images/Fluent/media.png";
    private readonly (string, string)[] _subtitles = new[]
    {
        ("softsub","ms-appx:///Assets/Images/Fluent/softsub.png"),
        ("hardsub","ms-appx:///Assets/Images/Fluent/hardsub.png"),
        ("srt","ms-appx:///Assets/Images/Fluent/subtitle.png"),
        ("Dub","ms-appx:///Assets/Images/Fluent/sound.png"),
        ("Audio","ms-appx:///Assets/Images/Fluent/sound.png")
    };

    private readonly (string, string)[] _seasonGlyph = new[]
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
    private readonly (string, string)[] _seasonFluent = new[]
    {
        ("S00","ms-appx:///Assets/Images/Fluent/0.png"),
        ("S01","ms-appx:///Assets/Images/Fluent/1.png"),
        ("S02","ms-appx:///Assets/Images/Fluent/2.png"),
        ("S03","ms-appx:///Assets/Images/Fluent/3.png"),
        ("S04","ms-appx:///Assets/Images/Fluent/4.png"),
        ("S05","ms-appx:///Assets/Images/Fluent/5.png"),
        ("S06","ms-appx:///Assets/Images/Fluent/6.png"),
        ("S07","ms-appx:///Assets/Images/Fluent/7.png"),
        ("S08","ms-appx:///Assets/Images/Fluent/8.png"),
        ("S09","ms-appx:///Assets/Images/Fluent/9.png"),
        ("S10","ms-appx:///Assets/Images/Fluent/10.png")
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

            switch (Settings.IconPack)
            {
                case IconPack.Glyph:
                    var seasonNumberGlyph = _seasonGlyph.FirstOrDefault(x => text.ToLower().StartsWith(x.Item1, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(seasonNumberGlyph.Item2))
                    {
                        return new FontIcon { Glyph = seasonNumberGlyph.Item2 };
                    }
                    break;
                case IconPack.Fluent:
                    var seasonNumberFluent = _seasonFluent.FirstOrDefault(x => text.ToLower().StartsWith(x.Item1, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(seasonNumberFluent.Item2))
                    {
                        return new BitmapIcon { UriSource = new Uri(seasonNumberFluent.Item2), ShowAsMonochrome = false };
                    }
                    break;
            }

            
        }
        return new BitmapIcon { UriSource = new Uri(defaultIcon), ShowAsMonochrome = false };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
