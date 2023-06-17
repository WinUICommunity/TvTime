using TvTime.ViewModels;

namespace TvTime.Views;

public sealed partial class IMDBDetailsWindow : Window
{
    public IMDBDetailsViewModel ViewModel { get; }
    public IMDBDetailsWindow(string query)
    {
        ViewModel = App.Current.Services.GetService<IMDBDetailsViewModel>();
        this.InitializeComponent();
        ViewModel.setQuery(query);
        var titlebar = new TitleBarHelper(this, TitleTextBlock, AppTitleBar, LeftPaddingColumn, IconColumn, TitleColumn, LeftDragColumn, SearchColumn, RightDragColumn, RightPaddingColumn);
        this.AppWindow.SetIcon("Assets/Fluent/icon.ico");
    }
}
