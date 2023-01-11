using Microsoft.UI.Xaml.Data;

namespace TvTime.Common;
public class Ext2IconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var text = value is string ? (string) value : null;
        if (!string.IsNullOrEmpty(text))
        {
            if (text.Contains(".mp4", StringComparison.OrdinalIgnoreCase))
            {
                return new Uri("ms-appx:///Assets/Images/mp4.png");
            }
            else if (text.Contains(".mkv", StringComparison.OrdinalIgnoreCase))
            {
                return new Uri("ms-appx:///Assets/Images/mkv.png");
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

