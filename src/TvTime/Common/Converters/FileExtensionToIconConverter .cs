namespace TvTime.Common;
public class FileExtension2IconConverter : IValueConverter
{
    private readonly (string, string)[] _extensions = new[]
    {
        (".avi", "ms-appx:///Assets/Fluent/media.png"),
        (".mp4", "ms-appx:///Assets/Fluent/media.png"),
        (".mkv", "ms-appx:///Assets/Fluent/media.png"),
        (".mp3", "ms-appx:///Assets/Fluent/sound.png"),
        (".m4a", "ms-appx:///Assets/Fluent/sound.png"),
        (".wav", "ms-appx:///Assets/Fluent/sound.png"),
        (".zip", "ms-appx:///Assets/Fluent/archive.png"),
        (".rar", "ms-appx:///Assets/Fluent/archive.png"),
        (".srt", "ms-appx:///Assets/Fluent/subtitle.png"),
        (".aas", "ms-appx:///Assets/Fluent/subtitle.png")
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

