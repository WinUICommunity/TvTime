using Microsoft.UI.Xaml.Data;

namespace TvTime.Common;
public class Text2BitmapIconConverter : IValueConverter
{
    private readonly (string, string)[] _subtitles = new[]
    {
        ("softsub","ms-appx:///Assets/Images/softsub.png"),
        ("hardsub","ms-appx:///Assets/Images/hardsub.png"),
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
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
