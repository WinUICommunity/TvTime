using System.Windows.Input;

using TvTime.Database.Tables;
using TvTime.ViewModels;

using Windows.System;

namespace TvTime.Views;
public sealed partial class ItemUserControl : UserControl
{
    public event EventHandler<RoutedEventArgs> Click;
    public event EventHandler<RoutedEventArgs> DoubleClick;

    public BaseMediaTable BaseMedia
    {
        get => (BaseMediaTable) GetValue(BaseMediaProperty);
        set => SetValue(BaseMediaProperty, value);
    }

    public BaseViewModel ViewModel
    {
        get => (BaseViewModel)GetValue(ViewModelProperty);
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

    public static readonly DependencyProperty BaseMediaProperty =
        DependencyProperty.Register("BaseMedia", typeof(BaseMediaTable), typeof(ItemUserControl), new PropertyMetadata(default(BaseMediaTable)));

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(BaseViewModel), typeof(ItemUserControl), new PropertyMetadata(default(BaseViewModel)));

    public static readonly DependencyProperty SettingsCardCommandProperty =
       DependencyProperty.Register("SettingsCardCommand", typeof(ICommand), typeof(ItemUserControl), new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty SettingsCardDoubleClickCommandProperty =
       DependencyProperty.Register("SettingsCardDoubleClickCommand", typeof(ICommand), typeof(ItemUserControl), new PropertyMetadata(default(ICommand)));

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

        if (Settings.UseTruncateInDescription)
        {
            TxtDesc.TextTrimming = TextTrimming.CharacterEllipsis;
            TxtDesc.TextWrapping = TextWrapping.NoWrap;
        }
        else
        {
            TxtDesc.TextTrimming = TextTrimming.None;
            TxtDesc.TextWrapping = TextWrapping.Wrap;
        }

        if (Settings.UseTruncateInHeader)
        {
            TxtHeader.TextTrimming = TextTrimming.CharacterEllipsis;
            TxtHeader.TextWrapping = TextWrapping.NoWrap;
        }
        else
        {
            TxtHeader.TextTrimming = TextTrimming.None;
            TxtHeader.TextWrapping = TextWrapping.Wrap;
        }
    }

    private void SettingsCard_Click(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(sender, e);
    }

    private void SettingsCard_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        DoubleClick?.Invoke(sender, e);
    }

    private async void ServerHyperLink_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await Launcher.LaunchUriAsync(new Uri(Server));
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, "ItemUserControl: Navigate To Uri for Server");
        }
    }
}
