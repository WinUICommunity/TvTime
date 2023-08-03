using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class SubtitleSettingPage : Page
{
    public SubtitleSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }

    public SubtitleSettingPage()
    {
        ViewModel = App.GetService<SubtitleSettingViewModel>();
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = e.Parameter as string;
    }

    private void SubtitleLanguage_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (!string.IsNullOrEmpty(sender.Text))
        {
            var exist = Settings.SubtitleLanguagesCollection.Any(x => x.Equals(sender.Text));
            if (exist)
            {
                StatusInfo.Title = App.Current.ResourceHelper.GetString("SubtitleSettingPage_StatusLanguageExist");
                StatusInfo.Severity = InfoBarSeverity.Error;
            }
            else
            {
                Settings.SubtitleLanguagesCollection.Add(sender.Text);
                sender.Text = string.Empty;
                StatusInfo.Title = App.Current.ResourceHelper.GetString("SubtitleSettingPage_StatusLanguageAdded");
                StatusInfo.Severity = InfoBarSeverity.Success;
            }
        }
        else
        {
            StatusInfo.Title = App.Current.ResourceHelper.GetString("SubtitleSettingPage_StatusTextNull");
            StatusInfo.Severity = InfoBarSeverity.Error;
        }
    }

    private void TokenView_TokenItemRemoving(object sender, CommunityToolkit.Labs.WinUI.TokenItemRemovingEventArgs e)
    {
        StatusInfo.Title = string.Format(App.Current.ResourceHelper.GetString("SubtitleSettingPage_StatusRemoved"), e.TokenItem?.Content?.ToString());
        StatusInfo.Severity = InfoBarSeverity.Informational;
    }
}
