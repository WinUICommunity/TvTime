using Microsoft.UI.Xaml.Data;

namespace TvTime.Common;
public class Bool2VisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not null && value is string text)
        {
            if (text.ToLower().Contains(".mp4") || text.ToLower().Contains(".mkv"))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
