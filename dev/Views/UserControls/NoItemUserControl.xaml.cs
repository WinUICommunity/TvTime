namespace TvTime.Views;
public sealed partial class NoItemUserControl : UserControl
{
    public string Message
    {
        get { return (string)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }

    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register("Message", typeof(string), typeof(NoItemUserControl), new PropertyMetadata("Item not found!"));

    public int Count
    {
        get => (int) GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    public static readonly DependencyProperty CountProperty =
        DependencyProperty.Register("Count", typeof(int), typeof(NoItemUserControl), new PropertyMetadata(0));

    public NoItemUserControl()
    {
        this.InitializeComponent();
    }
}
