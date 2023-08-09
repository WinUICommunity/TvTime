using System.Windows.Input;

namespace TvTime.Views;
public sealed partial class ItemUserControl : UserControl
{
    public IBaseViewModel ViewModel
    {
        get => (IBaseViewModel) GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public ICommand SettingsCardCommand
    {
        get => (ICommand) GetValue(SettingsCardCommandProperty);
        set => SetValue(SettingsCardCommandProperty, value);
    }

    public ICommand SettingsCardDoubleClickCommand
    {
        get => (ICommand) GetValue(SettingsCardDoubleClickCommandProperty);
        set => SetValue(SettingsCardDoubleClickCommandProperty, value);
    }

    public object Description
    {
        get => (object) GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string Title
    {
        get => (string) GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Server
    {
        get => (string) GetValue(ServerProperty);
        set => SetValue(ServerProperty, value);
    }

    public string DateTime
    {
        get => (string) GetValue(DateTimeProperty);
        set => SetValue(DateTimeProperty, value);
    }

    public string FileSize
    {
        get => (string) GetValue(FileSizeProperty);
        set => SetValue(FileSizeProperty, value);
    }

    public IconElement HeaderIcon
    {
        get => (IconElement) GetValue(HeaderIconProperty);
        set => SetValue(HeaderIconProperty, value);
    }

    public IconElement ActionIcon
    {
        get => (IconElement) GetValue(ActionIconProperty);
        set => SetValue(ActionIconProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(IBaseViewModel), typeof(ItemUserControl), new PropertyMetadata(default(IBaseViewModel)));

    public static readonly DependencyProperty SettingsCardCommandProperty =
       DependencyProperty.Register("SettingsCardCommand", typeof(ICommand), typeof(ItemUserControl), new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty SettingsCardDoubleClickCommandProperty =
       DependencyProperty.Register("SettingsCardDoubleClickCommand", typeof(ICommand), typeof(ItemUserControl), new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(object), typeof(ItemUserControl), new PropertyMetadata(default(object)));

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(ItemUserControl), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty ServerProperty =
        DependencyProperty.Register("Server", typeof(string), typeof(ItemUserControl), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty DateTimeProperty =
        DependencyProperty.Register("DateTime", typeof(string), typeof(ItemUserControl), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty FileSizeProperty =
        DependencyProperty.Register("FileSize", typeof(string), typeof(ItemUserControl), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty HeaderIconProperty =
        DependencyProperty.Register("HeaderIcon", typeof(IconElement), typeof(ItemUserControl), new PropertyMetadata(default(IconElement)));

    public static readonly DependencyProperty ActionIconProperty =
        DependencyProperty.Register("ActionIcon", typeof(IconElement), typeof(ItemUserControl), new PropertyMetadata(default(IconElement)));

    public ItemUserControl()
    {
        this.InitializeComponent();

        if (ActionIcon == null)
        {
            ActionIcon = new FontIcon { Glyph = "\ue974" };
        }

        if (Settings.TvTimeLanguage.FlowDirection == FlowDirection.RightToLeft)
        {
            MenuDirectory.FlowDirection = FlowDirection.RightToLeft;
            MenuIMDB.FlowDirection = FlowDirection.RightToLeft;
            MenuFile.FlowDirection = FlowDirection.RightToLeft;
            MenuCopy.FlowDirection = FlowDirection.RightToLeft;
            SubMenuCopy.FlowDirection = FlowDirection.RightToLeft;
            SubMenuCopyAll.FlowDirection = FlowDirection.RightToLeft;
            MenuDownload.FlowDirection = FlowDirection.RightToLeft;
            SubMenuDownload.FlowDirection = FlowDirection.RightToLeft;
            SubMenuDownloadAll.FlowDirection = FlowDirection.RightToLeft;
        }
        else
        {
            MenuDirectory.FlowDirection = FlowDirection.LeftToRight;
            MenuIMDB.FlowDirection = FlowDirection.LeftToRight;
            MenuFile.FlowDirection = FlowDirection.LeftToRight;
            MenuCopy.FlowDirection = FlowDirection.LeftToRight;
            SubMenuCopy.FlowDirection = FlowDirection.LeftToRight;
            SubMenuCopyAll.FlowDirection = FlowDirection.LeftToRight;
            MenuDownload.FlowDirection = FlowDirection.LeftToRight;
            SubMenuDownload.FlowDirection = FlowDirection.LeftToRight;
            SubMenuDownloadAll.FlowDirection = FlowDirection.LeftToRight;
        }
    }
}
