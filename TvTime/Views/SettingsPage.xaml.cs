using Microsoft.Extensions.DependencyInjection;
using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }
    public SettingsPage()
    {
        ViewModel = App.Current.Services.GetService<SettingsViewModel>();
        this.InitializeComponent();
    }
}
