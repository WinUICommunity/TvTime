using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class MediaServersPage : Page
{
    public MediaServerViewModel ViewModel { get; }

    public MediaServersPage()
    {
        ViewModel = App.Current.Services.GetService<MediaServerViewModel>();
        this.InitializeComponent();
        DataContext = this;
    }
}
