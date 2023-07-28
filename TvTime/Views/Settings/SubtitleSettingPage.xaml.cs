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
                StatusInfo.Title = "This Language is Exist, try another";
                StatusInfo.Severity = InfoBarSeverity.Error;
            }
            else
            {
                Settings.SubtitleLanguagesCollection.Add(sender.Text);
                sender.Text = string.Empty;
                StatusInfo.Title = "Language Added Successfully!";
                StatusInfo.Severity = InfoBarSeverity.Success;
            }
        }
        else
        {
            StatusInfo.Title = "Text is Null or Empty";
            StatusInfo.Severity = InfoBarSeverity.Error;
        }
    }

    private void TokenView_TokenItemRemoving(object sender, CommunityToolkit.Labs.WinUI.TokenItemRemovingEventArgs e)
    {
        StatusInfo.Title = $"'{e.TokenItem?.Content?.ToString()}' Removed";
        StatusInfo.Severity = InfoBarSeverity.Informational;
    }
}
