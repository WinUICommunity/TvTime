using Windows.System;

namespace TvTime.Views;

public sealed partial class ServerPage : Page
{
    public ServerViewModel ViewModel { get; set; }
    public ServerPage()
    {
        ViewModel = App.GetService<ServerViewModel>();
        this.InitializeComponent();
    }

    private void BtnUpdate_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.UpdateServerCommand.Execute((sender as Button).DataContext);
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.DeleteServerCommand.Execute((sender as Button).DataContext);
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.AddServerCommand.Execute(sender);
    }

    private async void BtnNavigateToUri_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var uri = (sender as HyperlinkButton).Tag?.ToString();
            await Launcher.LaunchUriAsync(new Uri(uri));
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, "ServerPage: Navigate To Uri for Server");
        }
    }
}
