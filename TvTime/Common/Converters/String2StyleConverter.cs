namespace TvTime.Common;
public class String2StyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string styleName && !string.IsNullOrEmpty(styleName))
        {
            return Application.Current.Resources[styleName] as Style;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
