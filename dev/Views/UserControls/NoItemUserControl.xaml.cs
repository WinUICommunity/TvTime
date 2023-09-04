namespace TvTime.Views;
public sealed partial class NoItemUserControl : UserControl
{
    public bool IsActive
    {
        get { return (bool) GetValue(IsActiveProperty); }
        set { SetValue(IsActiveProperty, value); }
    }

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register("IsActive", typeof(bool), typeof(NoItemUserControl), new PropertyMetadata(default(bool)));

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
