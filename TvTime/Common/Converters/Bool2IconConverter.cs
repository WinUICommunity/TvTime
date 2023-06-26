namespace TvTime.Common;
public class Bool2IconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isActive)
        {
            if (parameter is string iconType && !string.IsNullOrEmpty(iconType))
            {
                switch (iconType)
                {
                    case "glyph":
                        return isActive
                            ? new FontIcon { Glyph = "\uE73E" }
                            : new FontIcon { Glyph = "\uF140" };
                    case "bitmap":
                        return isActive
                            ? new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Fluent/active.png"), ShowAsMonochrome = false }
                            : (object) new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Fluent/deActive.png"), ShowAsMonochrome = false };
                }
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
