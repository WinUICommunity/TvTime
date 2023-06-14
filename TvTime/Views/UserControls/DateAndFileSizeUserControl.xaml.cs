namespace TvTime.Views;
public sealed partial class DateAndFileSizeUserControl : UserControl
{
    public string DateTime
    {
        get { return (string) GetValue(DateTimeProperty); }
        set { SetValue(DateTimeProperty, value); }
    }

    public string FileSize
    {
        get { return (string) GetValue(FileSizeProperty); }
        set { SetValue(FileSizeProperty, value); }
    }

    public static readonly DependencyProperty DateTimeProperty =
        DependencyProperty.Register("DateTime", typeof(string), typeof(DateAndFileSizeUserControl), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty FileSizeProperty =
        DependencyProperty.Register("FileSize", typeof(string), typeof(DateAndFileSizeUserControl), new PropertyMetadata(default(string)));

    public DateAndFileSizeUserControl()
    {
        this.InitializeComponent();
    }
}
