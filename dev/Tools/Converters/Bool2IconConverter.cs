namespace TvTime.Common;
public class Bool2IconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isActive)
        {
            return isActive
                            ? new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Fluent/active.png"), ShowAsMonochrome = false }
                            : (object) new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Fluent/deActive.png"), ShowAsMonochrome = false };
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
