namespace TvTime.Views;
public sealed partial class BreadcrumbBarUserControl : UserControl
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public string ItemText
    {
        get { return (string) GetValue(ItemTextProperty); }
        set { SetValue(ItemTextProperty, value); }
    }

    public static readonly DependencyProperty ItemTextProperty =
        DependencyProperty.Register("ItemText", typeof(string), typeof(BreadcrumbBarUserControl), new PropertyMetadata(default(string)));

    private ObservableCollection<string> breadcrumbBarCollection;

    public ObservableCollection<string> BreadcrumbBarCollection
    {
        get { return breadcrumbBarCollection; }
        set
        {
            breadcrumbBarCollection = value;
            OnPropertyChanged();
        }
    }

    public BreadcrumbBarUserControl()
    {
        this.InitializeComponent();
        BreadcrumbBarCollection = new();
        Loaded += BreadcrumbBarUserControl_Loaded;
    }

    private void BreadcrumbBarUserControl_Loaded(object sender, RoutedEventArgs e)
    {
        BreadcrumbBarCollection.Add("Settings");
        BreadcrumbBarCollection.Add(ItemText);
    }

    private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        App.Current.NavigationManager.GoBack();
    }
}

