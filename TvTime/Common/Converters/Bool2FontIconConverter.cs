using Microsoft.UI.Xaml.Media;

namespace TvTime.Common;
public class Bool2FontIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isActive)
        {
            if (isActive)
            {
                return new FontIcon { Glyph = "\uE73E", FontFamily = Application.Current.Resources["SymbolThemeFontFamily"] as FontFamily };
            }
            else
            {
                return new FontIcon { Glyph = "\uF140", FontFamily = Application.Current.Resources["SymbolThemeFontFamily"] as FontFamily };
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
