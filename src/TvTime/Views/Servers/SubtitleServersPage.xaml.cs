namespace TvTime.Views;
public sealed partial class SubtitleServersPage : Page
{
    public static SubtitleServersPage Instance { get; private set; }
    public SubtitleServersPage()
    {
        this.InitializeComponent();
        Instance = this;
    }
}
