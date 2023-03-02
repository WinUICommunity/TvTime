using Microsoft.UI.Xaml.Data;

namespace TvTime.Common;
public class FileExtension2IconConverter : IValueConverter
{
    private readonly (string, string)[] _extensions = new[]
    {
        (".mp4", "ms-appx:///Assets/Images/mp4.png"),
        (".mkv", "ms-appx:///Assets/Images/mkv.png"),
        (".mp3", "ms-appx:///Assets/Images/audio.png"),
        (".m4a", "ms-appx:///Assets/Images/audio.png"),
        (".wav", "ms-appx:///Assets/Images/audio.png"),
    };
    public object Convert(object value, Type targetType, object parameter, string language)
    {

        var text = value as string;
        if (!string.IsNullOrEmpty(text))
        {
            var extension = _extensions.FirstOrDefault(x => text.EndsWith(x.Item1, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(extension.Item2))
            {
                return new BitmapIcon { ShowAsMonochrome = false, UriSource = new Uri(extension.Item2) };
            }
        }
        return new FontIcon { Glyph = "\ue974" };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

