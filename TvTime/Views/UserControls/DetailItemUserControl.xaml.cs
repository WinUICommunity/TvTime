using System.Windows.Input;

using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class DetailItemUserControl : UserControl
{
    public DetailsViewModel ViewModel
    {
        get { return (DetailsViewModel) GetValue(ViewModelProperty); }
        set { SetValue(ViewModelProperty, value); }
    }

    public ICommand SettingsCardCommand
    {
        get { return (ICommand) GetValue(SettingsCardCommandProperty); }
        set { SetValue(SettingsCardCommandProperty, value); }
    }

    public LocalItem MediaItem
    {
        get { return (LocalItem) GetValue(MediaItemProperty); }
        set { SetValue(MediaItemProperty, value); }
    }

    public object Description
    {
        get { return (object) GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(DetailsViewModel), typeof(DetailItemUserControl), new PropertyMetadata(default(DetailsViewModel)));

    public static readonly DependencyProperty SettingsCardCommandProperty =
       DependencyProperty.Register("SettingsCardCommand", typeof(ICommand), typeof(DetailItemUserControl), new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty MediaItemProperty =
        DependencyProperty.Register("MediaItem", typeof(LocalItem), typeof(DetailItemUserControl), new PropertyMetadata(default(LocalItem)));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(object), typeof(DetailItemUserControl), new PropertyMetadata(default(object)));


    public DetailItemUserControl()
    {
        this.InitializeComponent();
    }
}
