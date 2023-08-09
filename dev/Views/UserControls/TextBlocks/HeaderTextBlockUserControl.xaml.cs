namespace TvTime.Views;
public sealed partial class HeaderTextBlockUserControl : UserControl
{
    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(HeaderTextBlockUserControl), new PropertyMetadata(default(string)));

    public HeaderTextBlockUserControl()
    {
        this.InitializeComponent();
    }
}
