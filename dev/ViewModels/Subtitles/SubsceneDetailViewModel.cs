using CommunityToolkit.Labs.WinUI;
using CommunityToolkit.WinUI.UI;

using HtmlAgilityPack;

using TvTime.Database.Tables;

using Windows.System;

namespace TvTime.ViewModels;
public partial class SubsceneDetailViewModel : BaseViewModel, ITitleBarAutoSuggestBoxAware
{
    [ObservableProperty]
    public ObservableCollection<BaseMediaTable> breadcrumbBarList = new();

    [ObservableProperty]
    public ObservableCollection<TokenItem> languageTokenList;

    [ObservableProperty]
    public ObservableCollection<TokenItem> qualityTokenList;

    private DispatcherTimer dispatcherTimer = new DispatcherTimer();

    public override void OnPageLoaded(object param)
    {
        ObservableCollection<TokenItem> languageTokens = new ObservableCollection<TokenItem>();
        foreach (var item in SubtitleLanguageCollection())
        {
            if (Settings.SubtitleLanguagesCollection.Any(x => x.Equals(item.Content.ToString())))
            {
                item.IsSelected = false;
                languageTokens.Add(item);
            }
        }

        LanguageTokenList = new(languageTokens);
        LanguageTokenList.Insert(0, new TokenItem { Content = "All", IsSelected = true });

        DownloadDetails(rootMedia);
    }

    public override void OnRefresh()
    {
        DownloadDetails(rootMedia);
    }

    public override void OnIMDBDetail()
    {
        CreateIMDBDetailsWindow(rootMedia.Title);
    }

    public override void NavigateToDetails(object sender)
    {
        OnNavigateToDetailsOrDownload(sender);
    }

    private async void OnNavigateToDetailsOrDownload(object sender)
    {
        base.NavigateToDetails(sender);

        if (!Settings.UseIDMForDownloadSubtitles)
        {
            await Launcher.LaunchUriAsync(new Uri(descriptionText));
        }
        else
        {
            if (ApplicationHelper.IsNetworkAvailable())
            {
                try
                {
                    var web = new HtmlWeb();
                    var doc = await web.LoadFromWebAsync(descriptionText);

                    if (doc != null)
                    {
                        var node = doc.DocumentNode?.SelectSingleNode("//div[@class='download']//a");
                        if (node != null)
                        {
                            var downloadLink = GetServerUrlWithoutRightPart(descriptionText) + node.GetAttributeValue("href", "nothing");

                            LaunchIDM(GetIDMFilePath(), downloadLink);
                        }
                    }
                }
                catch (Exception ex)
                {
                    StatusTitle = "Error";
                    StatusMessage = ex.Message;
                    StatusSeverity = InfoBarSeverity.Error;
                    IsStatusOpen = true;
                    Logger?.Error(ex, "SubsceneDetailViewModel: OnNavigateToDetailsOrDownload");
                }
            }
            else
            {
                StatusTitle = "";
                StatusMessage = "Oh no! You're not connected to the internet.";
                StatusSeverity = InfoBarSeverity.Error;
                IsStatusOpen = true;
            }
        }
    }

    [RelayCommand]
    private void OnBreadCrumbBarItem(BreadcrumbBarItemClickedEventArgs args)
    {
        var item = (SubtitleModel) args.Item;
        BreadcrumbBarList.RemoveAt(args.Index + 1);
        DownloadDetails(item);
    }

    private async void DownloadDetails(BaseMediaTable baseMedia)
    {
        try
        {
            BreadcrumbBarList.AddIfNotExists(baseMedia);
            IsActive = true;
            IsStatusOpen = true;
            StatusSeverity = InfoBarSeverity.Informational;
            StatusTitle = "Please Wait...";
            StatusMessage = "";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(baseMedia.Server);

            StatusMessage = string.Format("Working on {0}", baseMedia.Title);

            StatusMessage = string.Format("Parsing {0}", baseMedia.Title);

            var details = GetServerDetails(doc, baseMedia);
            DataList = new(details.Cast<BaseMediaTable>());
            DataListACV = new AdvancedCollectionView(DataList, true);
            var currentSortDescription = new SortDescription("Title", SortDirection.Ascending);
            DataListACV.SortDescriptions.Add(currentSortDescription);

            IsActive = false;
            StatusTitle = "Subtitle information received successfully";
            StatusMessage = "";
            StatusSeverity = InfoBarSeverity.Success;
            AutoHideStatusInfoBar(new TimeSpan(0, 0, 4));
        }
        catch (Exception ex)
        {
            IsStatusOpen = true;
            IsActive = false;
            StatusTitle = "Error";
            StatusMessage = ex.Message + Environment.NewLine + ex.InnerException;
            StatusSeverity = InfoBarSeverity.Error;
            Logger?.Error(ex, "SubsceneDetailViewModel: DownloadDetails");
        }
    }

    public List<SubtitleModel> GetServerDetails(HtmlDocument doc, BaseMediaTable baseMedia)
    {
        List<SubtitleModel> list = new List<SubtitleModel>();

        var table = doc.DocumentNode.SelectSingleNode("//table[1]//tbody");
        if (table != null)
        {
            foreach (var cell in table.SelectNodes(".//tr"))
            {
                if (cell.InnerText.Contains("There are no subtitles"))
                    break;

                var Language = cell.SelectSingleNode(".//td[@class='a1']//a//span[1]")?.InnerText.Trim();

                // respect Subtitle Language Settings
                if (!string.IsNullOrEmpty(Language) && !Settings.SubtitleLanguagesCollection.Any(x=> Language.Contains(x, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var Name = cell.SelectSingleNode(".//td[@class='a1']//a//span[2]")?.InnerText.Trim();
                var Translator = cell.SelectSingleNode(".//td[@class='a5']//a")?.InnerText.Trim();
                var Comment = cell.SelectSingleNode(".//td[@class='a6']//div")?.InnerText.Trim();
                if (Comment != null && Comment.Contains("&nbsp;")) Comment = Comment.Replace("&nbsp;", "");

                var Link = cell.SelectSingleNode(".//td[@class='a1']//a")?.Attributes["href"]?.Value.Trim();

                if (Name != null)
                {
                    var item = new SubtitleModel(Name, Name, GetServerUrlWithoutRightPart(baseMedia.Server) + Link, Comment, Language, Translator);
                    list.Add(item);
                }
            }
            return list;
        }
        else
        {
            IsActive = false;
            IsStatusOpen = true;
            StatusTitle = "Subtitles not found or server is unavailable, please try again!";
            StatusSeverity = InfoBarSeverity.Error;
        }

        return null;
    }

    private void AutoHideStatusInfoBar(TimeSpan timeSpan)
    {
        dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Tick += (s, e) =>
        {
            dispatcherTimer?.Stop();
            dispatcherTimer = null;
            StatusMessage = "";
            StatusTitle = "";
            StatusSeverity = InfoBarSeverity.Informational;
            IsStatusOpen = false;
        };
        dispatcherTimer.Interval = timeSpan;
        dispatcherTimer.Start();
    }

    public void OnAutoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        Search(sender.Text);
    }

    public void OnAutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Search(sender.Text);
    }

    private void Search(string query)
    {
        if (DataList != null && DataList.Any())
        {
            DataListACV.Filter = _ => true;
            DataListACV.Filter = item =>
            {
                var subtitle = (SubtitleModel)item;
                var name = subtitle.Title ?? "";
                var lang = subtitle.Language ?? "";
                var languageItems = SubsceneDetailPage.Instance.GetTokenViewSelectedItems();
                return languageItems.Any(token => token.Content.ToString().Equals("All"))
                    ? name.Contains(query, StringComparison.OrdinalIgnoreCase)
                    : (name.Contains(query, StringComparison.OrdinalIgnoreCase)) &&
                        (languageItems.Any(token => lang.Contains(token.Content.ToString())));
            };
        }
    }
}
