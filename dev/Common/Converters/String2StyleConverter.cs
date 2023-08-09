namespace TvTime.Common;
public class String2StyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is string styleName && !string.IsNullOrEmpty(styleName) ? Application.Current.Resources[styleName] as Style : (object) null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
