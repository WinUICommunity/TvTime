using Microsoft.UI.Xaml.Data;

namespace TvTime.Common
{
    public class Text2BitmapIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var text = value is string ? (string) value : null;

            if (!string.IsNullOrEmpty(text) && text.ToLower().Contains("softsub", StringComparison.OrdinalIgnoreCase))
            {
                return new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Images/softsub.png"), ShowAsMonochrome = false };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
