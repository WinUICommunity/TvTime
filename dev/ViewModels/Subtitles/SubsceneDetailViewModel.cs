using CommunityToolkit.Labs.WinUI;

namespace TvTime.ViewModels;
public partial class SubsceneDetailViewModel : BaseViewModel
{
    [ObservableProperty]
    public ObservableCollection<ITvTimeModel> breadcrumbBarList = new();

    [ObservableProperty]
    public ObservableCollection<TokenItem> languageTokenList;

    [ObservableProperty]
    public ObservableCollection<TokenItem> qualityTokenList;

    #region Override Methods

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
        LanguageTokenList.Insert(0, new TokenItem { Content = App.Current.ResourceHelper.GetString("Constants_AllFilter"), IsSelected = true });

        DownloadDetails(rootTvTimeItem);
    }

    public override void OnRefresh()
    {
        DownloadDetails(rootTvTimeItem);
    }

    public override void OnIMDBDetail()
    {
        CreateIMDBDetailsWindow(rootTvTimeItem.Title);
    }

    public override void NavigateToDetails(object sender)
    {
        OnNavigateToDetailsOrDownload(sender);
    }

    public override bool DataListFilter(object item)
    {
        var query = (SubsceneModel) item;
        var name = query.Title ?? "";
        var lang = query.Language ?? "";
        var txtSearch = MainPage.Instance.GetTxtSearch();
        var languageItems = SubsceneDetailPage.Instance.GetLanguageTokenViewSelectedItems();
        return languageItems.Any(token => token.Content.ToString().Equals(App.Current.ResourceHelper.GetString("Constants_AllFilter")))
            ? name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            : (name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)) &&
                (languageItems.Any(token => lang.Contains(token.Content.ToString())));
    }

    #endregion

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
                    StatusTitle = "";
                    StatusMessage = ex.Message;
                    StatusSeverity = InfoBarSeverity.Error;
                    IsStatusOpen = true;
                }
            }
            else
            {
                StatusTitle = "";
                StatusMessage = App.Current.ResourceHelper.GetString("Constants_InternetNotAvailable");
                StatusSeverity = InfoBarSeverity.Error;
                IsStatusOpen = true;
            }
        }
    }

    [RelayCommand]
    private void OnBreadCrumbBarItem(BreadcrumbBarItemClickedEventArgs args)
    {
        var item = (SubsceneModel) args.Item;
        BreadcrumbBarList.RemoveAt(args.Index + 1);
        DownloadDetails(item);
    }

    private async void DownloadDetails(ITvTimeModel tvTimeItem)
    {
        try
        {
            BreadcrumbBarList.AddIfNotExists(tvTimeItem);
            IsActive = true;
            IsStatusOpen = true;
            StatusSeverity = InfoBarSeverity.Informational;
            StatusTitle = App.Current.ResourceHelper.GetString("SubsceneDetailViewModel_StatusWait");
            StatusMessage = "";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(tvTimeItem.Server);

            StatusMessage = string.Format(App.Current.ResourceHelper.GetString("SubsceneDetailViewModel_StatusWorking"), tvTimeItem.Title);

            StatusMessage = string.Format(App.Current.ResourceHelper.GetString("SubsceneDetailViewModel_StatusParsing"), tvTimeItem.Title);

            var details = GetServerDetails(doc, tvTimeItem);
            DataList = new(details.Cast<ITvTimeModel>());
            DataListACV = new AdvancedCollectionView(DataList, true);
            currentSortDescription = new SortDescription("Title", SortDirection.Ascending);
            DataListACV.SortDescriptions.Add(currentSortDescription);
            suggestList = DataListACV.Select(x => ((SubsceneModel) x).Title).ToList();

            IsActive = false;
            StatusTitle = App.Current.ResourceHelper.GetString("SubsceneDetailViewModel_StatusUpdated");
            StatusMessage = "";
            StatusSeverity = InfoBarSeverity.Success;
        }
        catch (Exception ex)
        {
            IsStatusOpen = true;
            IsActive = false;
            StatusTitle = App.Current.ResourceHelper.GetString("SubsceneDetailViewModel_StatusError");
            StatusMessage = ex.Message + Environment.NewLine + ex.InnerException;
            StatusSeverity = InfoBarSeverity.Error;
        }
    }

    public List<SubsceneModel> GetServerDetails(HtmlDocument doc, ITvTimeModel tvTimeItem)
    {
        List<SubsceneModel> list = new List<SubsceneModel>();

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
                    var item = new SubsceneModel
                    {
                        Name = Name,
                        Title = Name,
                        Translator = Translator,
                        Desc = Comment,
                        Server = GetServerUrlWithoutRightPart(tvTimeItem.Server) + Link,
                        Language = Language
                    };
                    list.Add(item);
                }
            }
            return list;
        }
        else
        {
            IsActive = false;
            IsStatusOpen = true;
            StatusTitle = App.Current.ResourceHelper.GetString("Constants_SubtitleNotFound");
            StatusSeverity = InfoBarSeverity.Error;
        }

        return null;
    }
}
