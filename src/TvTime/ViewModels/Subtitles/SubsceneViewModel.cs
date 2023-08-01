﻿using CommunityToolkit.Labs.WinUI;

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

    public IJsonNavigationViewService JsonNavigationViewService;

    public SubsceneViewModel(IJsonNavigationViewService jsonNavigationViewService)
    {
        JsonNavigationViewService = jsonNavigationViewService;

        var tokens = ServerSettings.SubtitleServers.Where(x => x.IsActive)
            .Select(x => new TokenItem { Content = GetServerUrlWithoutLeftAndRightPart(x.Server) });

        TokenList = new(tokens);

        var defaultTokenItem = TokenList.FirstOrDefault(x => x.Content.ToString().Contains("subscene", StringComparison.OrdinalIgnoreCase));
        TokenItemSelectedIndex = TokenList.IndexOf(defaultTokenItem);

        if (ServerSettings.SubtitleServers.Count == 0)
        {
            IsStatusOpen = true;
            StatusTitle = App.Current.Localizer.GetLocalizedString("SubsceneViewModel_StatusNoServerTitle");
            StatusMessage = App.Current.Localizer.GetLocalizedString("SubsceneViewModel_StatusNoServerMessage");
            StatusSeverity = InfoBarSeverity.Warning;
            GoToServerPage(JsonNavigationViewService, true);
        }
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

        JsonNavigationViewService.NavigateTo(typeof(SubsceneDetailPage), subtitle);
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
                if (ServerSettings.SubtitleServers.Count == 0)
                {
                    GoToServerPage(JsonNavigationViewService, true);
                    return;
                }

                IsActive = true;
                IsStatusOpen = true;
                StatusSeverity = InfoBarSeverity.Informational;
                StatusTitle = App.Current.Localizer.GetLocalizedString("SubsceneViewModel_StatusWait");
                StatusMessage = "";

                if (!string.IsNullOrEmpty(QueryText))
                {
                    IsActive = true;
                    DataList = new();
                    var baseUrl = ServerSettings?.SubtitleServers?.FirstOrDefault(x => x.Server.Contains(((TokenItem) TokenItemSelectedItem).Content.ToString(), StringComparison.OrdinalIgnoreCase));
                    var url = string.Format(Constants.SubsceneSearchAPI, baseUrl?.Server, QueryText);
                    var web = new HtmlWeb();
                    var doc = await web.LoadFromWebAsync(url);

                    var titleCollection = doc?.DocumentNode?.SelectSingleNode("//div[@class='search-result']");
                    if (titleCollection == null || titleCollection.InnerText.Contains("No results found"))
                    {
                        ShowError(App.Current.Localizer.GetLocalizedString("Constants_SubtitleNotFound"));
                    }
                    else
                    {
                        DataListACV = new AdvancedCollectionView(DataList, true);

                        using (DataListACV.DeferRefresh())
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
                                            Server = ApplicationHelper.GetDecodedStringFromHtml(baseUrl.Server + subNode?.Attributes["href"]?.Value?.Trim()),
                                            Desc = count?.InnerText?.Trim(),
                                            GroupKey = GetSubtitleKey(i)
                                        };

                                        DataList.Add(subtitle);
                                    }
                                }
                                else
                                {
                                    ShowError(App.Current.Localizer.GetLocalizedString("Constants_SubtitleNotFound"));
                                }
                            }
                        }
                        currentSortDescription = new SortDescription("Title", SortDirection.Ascending);

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
            ShowError(App.Current.Localizer.GetLocalizedString("Constants_InternetNotAvailable"));
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
        StatusTitle = App.Current.Localizer.GetLocalizedString("SubsceneViewModel_StatusError");
        StatusSeverity = InfoBarSeverity.Error;
        StatusMessage = message;
        IsStatusOpen = true;
    }
}
