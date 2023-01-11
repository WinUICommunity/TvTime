using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace TvTime.Common
{
    public class ViewType2BitmapIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not null && value is string viewType)
            {
                switch (viewType)
                {
                    case "Series":
                        return new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Images/series.png"), ShowAsMonochrome = false };

                    case "Movie":
                        return new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Images/movie.png"), ShowAsMonochrome = false };

                    case "Anime":
                        return new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Images/anime.png"), ShowAsMonochrome = false };
                }
            }
            return new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/Images/series.png"), ShowAsMonochrome = false };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
