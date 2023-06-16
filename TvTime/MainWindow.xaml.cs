namespace TvTime;

public sealed partial class MainWindow : Window
{
    public string AppTitle { get; set; }
    public MainWindow()
    {
        this.InitializeComponent();
        this.AppTitle = this.AppWindow.Title = $"TvTime v{App.Current.TvTimeVersion}";
        this.AppWindow.SetIcon("Assets/Fluent/icon.ico");
    }
}
