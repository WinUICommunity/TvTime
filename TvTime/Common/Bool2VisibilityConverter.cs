using Microsoft.UI.Xaml.Data;

namespace TvTime.Common;
public class Bool2VisibilityConverter : IValueConverter
{
    private readonly string[] _extensions = new[]
    {
        ".mp4",
        ".mkv",
        ".m4a",
        ".mp3",
        ".wav"
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
