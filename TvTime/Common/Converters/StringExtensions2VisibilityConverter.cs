namespace TvTime.Common;
public class StringExtensions2VisibilityConverter : IValueConverter
{
    private readonly string[] _extensions = new[]
    {
        ".avi",
        ".mp4",
        ".mkv",
        ".m4a",
        ".mp3",
        ".wav",
        ".rar",
        ".zip",
        ".srt",
        ".aas"
    };

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var text = value as string;
        if (!string.IsNullOrEmpty(text))
        {
            var extension = _extensions.FirstOrDefault(x => text.EndsWith(x, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(extension))
            {
                return Visibility.Visible;
            }
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
