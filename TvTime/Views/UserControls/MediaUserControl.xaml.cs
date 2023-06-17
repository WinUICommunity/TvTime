using TvTime.ViewModels;

namespace TvTime.Views;

public sealed partial class MediaUserControl : UserControl
{
    public PageOrDirectoryType PageType
    {
        get { return (PageOrDirectoryType) GetValue(PageTypeProperty); }
        set { SetValue(PageTypeProperty, value); }
    }

    public static readonly DependencyProperty PageTypeProperty =
        DependencyProperty.Register("PageType", typeof(PageOrDirectoryType), typeof(MediaUserControl), new PropertyMetadata(null));

    public static MediaUserControl Instance { get; set; }
    public MediaViewModel ViewModel { get; }

    public MediaUserControl()
    {
        ViewModel = App.Current.Services.GetService<MediaViewModel>();
        this.InitializeComponent();
        Instance = this;
        DataContext = this;
    }
}
