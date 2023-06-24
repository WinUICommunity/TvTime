namespace TvTime.Views;
public sealed partial class NoItemUserControl : UserControl
{
    public int Count
    {
        get { return (int) GetValue(CountProperty); }
        set { SetValue(CountProperty, value); }
    }

    public static readonly DependencyProperty CountProperty =
        DependencyProperty.Register("Count", typeof(int), typeof(NoItemUserControl), new PropertyMetadata(0));


    public NoItemUserControl()
    {
        this.InitializeComponent();
    }
}
