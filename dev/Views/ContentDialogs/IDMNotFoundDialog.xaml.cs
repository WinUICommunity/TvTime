namespace TvTime.Views.ContentDialogs;

public sealed partial class IDMNotFoundDialog : ContentDialog
{
    public IDMNotFoundDialog()
    {
        this.InitializeComponent();
        XamlRoot = App.currentWindow.Content.XamlRoot;
    }
}
