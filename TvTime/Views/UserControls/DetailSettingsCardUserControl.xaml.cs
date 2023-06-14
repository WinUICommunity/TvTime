using System.Windows.Input;

using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class DetailSettingsCardUserControl : UserControl
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

    public LocalItem LocalItem
    {
        get { return (LocalItem) GetValue(LocalItemProperty); }
        set { SetValue(LocalItemProperty, value); }
    }

    public object Description
    {
        get { return (object) GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(DetailsViewModel), typeof(DetailSettingsCardUserControl), new PropertyMetadata(default(DetailsViewModel)));

    public static readonly DependencyProperty SettingsCardCommandProperty =
       DependencyProperty.Register("SettingsCardCommand", typeof(ICommand), typeof(DetailSettingsCardUserControl), new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty LocalItemProperty =
        DependencyProperty.Register("LocalItem", typeof(LocalItem), typeof(DetailSettingsCardUserControl), new PropertyMetadata(default(LocalItem)));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(object), typeof(DetailSettingsCardUserControl), new PropertyMetadata(default(object)));


    public DetailSettingsCardUserControl()
    {
        this.InitializeComponent();
    }
}
