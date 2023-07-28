namespace TvTime.Views;
public sealed partial class MediaServersPage : Page
{
    public static MediaServersPage Instance { get; private set; }
    public MediaServersPage()
    {
        this.InitializeComponent();
        Instance = this;
    }
}
