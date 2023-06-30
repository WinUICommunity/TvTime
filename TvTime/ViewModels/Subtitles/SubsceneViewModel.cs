using CommunityToolkit.Labs.WinUI;

namespace TvTime.ViewModels;
public partial class SubsceneViewModel : BaseViewModel
{
    [ObservableProperty]
    public ObservableCollection<TokenItem> tokenList;

    [ObservableProperty]
    public string queryText;

    [ObservableProperty]
    public int tokenItemSelectedIndex = -1;

    [ObservableProperty]
    public object tokenItemSelectedItem = null;

    public SubsceneViewModel()
    {
        var tokens = Settings.SubtitleServers.Where(x => x.IsActive)
            .Select(x => new TokenItem { Content = GetServerUrlWithoutLeftAndRightPart(x.Server) });

        TokenList = new(tokens);

        var defaultTokenItem = TokenList.FirstOrDefault(x => x.Content.ToString().Contains("deltaleech", StringComparison.OrdinalIgnoreCase));
        TokenItemSelectedIndex = TokenList.IndexOf(defaultTokenItem);
    }

    #region Override Methods
    public override void NavigateToDetails(object sender)
    {
        base.NavigateToDetails(sender);

        var subtitle = new SubsceneModel
        {
            Server = descriptionText,
            Title = headerText
        };

        App.Current.NavigationManager.NavigateForJson(typeof(SubsceneDetailPage), subtitle);
    }

    public override void OnRefresh()
    {
        OnQuerySubmitted();
    }

    public override bool DataListFilter(object item)
    {
        var query = (SubsceneModel) item;
        var name = query.Title ?? "";
        var server = query.Server ?? "";
        var txtSearch = MainPage.Instance.GetTxtSearch();
        return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            || server.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
    }
    #endregion

    public void setQuery(string query)
    {
        this.QueryText = query;
    }

    public async void OnQuerySubmitted()
    {
        if (ApplicationHelper.IsNetworkAvailable())
        {
            try
            {
                IsActive = true;
                IsStatusOpen = true;
                StatusSeverity = InfoBarSeverity.Informational;
                StatusTitle = "Please Wait...";
                StatusMessage = "";

                if (!string.IsNullOrEmpty(QueryText))
                {
                    IsActive = true;
                    DataList = new();
                    var baseUrl = Settings?.SubtitleServers?.FirstOrDefault(x => x.Server.Contains(((TokenItem) TokenItemSelectedItem).Content.ToString(), StringComparison.OrdinalIgnoreCase));
                    var url = string.Format(Constants.SubsceneSearchAPI, baseUrl?.Server, QueryText);
                    var web = new HtmlWeb();
                    var doc = await web.LoadFromWebAsync(url);

                    var titleCollection = doc?.DocumentNode?.SelectSingleNode("//div[@class='search-result']");
                    if (titleCollection == null || titleCollection.InnerText.Contains("No results found"))
                    {
                        ShowError(Constants.NotFoundOrExist);
                    }
                    else
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            IsStatusOpen = false;
                            var node = titleCollection.SelectSingleNode($"ul[{i}]");
                            if (node != null)
                            {
                                foreach (var item in node.SelectNodes("li"))
                                {
                                    var subNode = item?.SelectSingleNode("div//a");
                                    var count = item?.SelectSingleNode("span");
                                    if (count == null)
                                    {
                                        count = item?.SelectSingleNode("div[@class='subtle count']");
                                    }

                                    var name = subNode?.InnerText?.Trim();
                                    var subtitle = new SubsceneModel
                                    {
                                        Title = name,
                                        Server = GetDecodedStringFromHtml(baseUrl.Server + subNode?.Attributes["href"]?.Value?.Trim()),
                                        Desc = count?.InnerText?.Trim(),
                                        GroupKey = GetSubtitleKey(i)
                                    };

                                    DataList.Add(subtitle);
                                }
                            }
                            else
                            {
                                ShowError(Constants.NotFoundOrExist);
                            }
                        }
                        currentSortDescription = new SortDescription("Title", SortDirection.Ascending);

                        DataListACV = new AdvancedCollectionView(DataList, true);

                        suggestList = DataListACV.Select(x => ((SubsceneModel) x).Title).ToList();

                        DataListACV.SortDescriptions.Add(currentSortDescription);
                    }
                }
                IsActive = false;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                IsActive = false;
            }
        }
        else
        {
            ShowError(Constants.InternetIsNotAvailable);
        }
    }

    private string GetSubtitleKey(int index)
    {
        switch (index)
        {
            case 1:
                return "TVSeries";
            case 2:
                return "Close";
            case 3:
                return "Popular";
        }
        return null;
    }

    private void ShowError(string message)
    {
        StatusTitle = "Error";
        StatusSeverity = InfoBarSeverity.Error;
        StatusMessage = message;
        IsStatusOpen = true;
    }
}
