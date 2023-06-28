namespace TvTime.Views;
public sealed partial class DescriptionTextBlockUserControl : UserControl
{
    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(DescriptionTextBlockUserControl), new PropertyMetadata(default(string)));


    public DescriptionTextBlockUserControl()
    {
        this.InitializeComponent();
    }
}
