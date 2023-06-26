using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class IMDBDetailsPage : Page
{
    public static IMDBDetailsPage Instance { get; set; }
    public IMDBDetailsViewModel ViewModel { get; }

    public IMDBDetailsPage()
    {
        ViewModel = App.Current.Services.GetService<IMDBDetailsViewModel>();
        this.InitializeComponent();
        Instance = this;
    }
}
