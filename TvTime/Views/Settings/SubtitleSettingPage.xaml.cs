using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class SubtitleSettingPage : Page
{
    public SubtitleSettingViewModel ViewModel { get; }
    public string BreadCrumbBarItemText { get; set; }

    public SubtitleSettingPage()
    {
        ViewModel = App.Current.Services.GetService<SubtitleSettingViewModel>();
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = ((NavigationArgs) e.Parameter).Parameter as string;
    }

    private void SubtitleQuality_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (!string.IsNullOrEmpty(sender.Text))
        {
            var exist = Settings.SubtitleQualityCollection.Any(x => x.Equals(sender.Text));
            if (exist)
            {
                StatusInfo.Title = "This Quality is Exist, try another";
                StatusInfo.Severity = InfoBarSeverity.Error;
            }
            else
            {
                Settings.SubtitleQualityCollection.Add(sender.Text);
                sender.Text = string.Empty;
                StatusInfo.Title = "Quality Added Successfuly!";
                StatusInfo.Severity = InfoBarSeverity.Success;
            }
        }
        else
        {
            StatusInfo.Title = "Text is Null or Empty";
            StatusInfo.Severity = InfoBarSeverity.Error;
        }
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
                StatusInfo.Title = "Language Added Successfuly!";
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

    private void TGRegex_Toggled(object sender, RoutedEventArgs e)
    {
        if (TGRegex.IsOn)
        {
            Settings.SubtitleFileNameRegex = Constants.SubtitleFileNameRegex;
        }
    }
}
