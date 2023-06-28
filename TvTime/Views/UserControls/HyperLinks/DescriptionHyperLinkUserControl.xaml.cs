namespace TvTime.Views;
public sealed partial class DescriptionHyperLinkUserControl : UserControl
{
    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(DescriptionHyperLinkUserControl), new PropertyMetadata(default(string)));

    public DescriptionHyperLinkUserControl()
    {
        this.InitializeComponent();
    }

    private async void ServerHyperLink_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await Launcher.LaunchUriAsync(new Uri(Text));
        }
        catch (Exception)
        {
        }
    }
}
