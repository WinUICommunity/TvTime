using CommunityToolkit.Labs.WinUI;

namespace TvTime.ViewModels;
public partial class SubsceneViewModel : ObservableRecipient
{
    [ObservableProperty]
    public ObservableCollection<SubsceneModel> dataList;

    [ObservableProperty]
    public AdvancedCollectionView dataListACV;

    [ObservableProperty]
    public bool isStatusOpen;

    [ObservableProperty]
    public string statusMessage;

    [ObservableProperty]
    public string statusTitle;

    [ObservableProperty]
    public InfoBarSeverity statusSeverity;

    [ObservableProperty]
    public string queryText;

    public List<string> suggestList = new();
    private SortDescription currentSortDescription;

    [RelayCommand]
    private void OnSegmentedItemChanged(object sender)
    {
        var segmented = sender as Segmented;
        var selectedItem = segmented.SelectedItem as SegmentedItem;
        if (selectedItem != null)
        {
            switch (selectedItem.Tag?.ToString())
            {
                case "Refresh":
                    OnQuerySubmitted();
                    segmented.SelectedIndex = -1;
                    break;
            }
        }
    }

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
                    var url = string.Format(Constants.SubsceneSearchAPI, "https://sub.deltaleech.com", QueryText);
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
                                        Server = subNode?.Attributes["href"]?.Value?.Trim(),
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

    public void Search(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        var suggestItems = DataListACV.Select(x => ((SubsceneModel) x).Title).ToList();
        AutoSuggestBoxHelper.LoadSuggestions(sender, args, suggestItems);
        DataListACV.Filter = _ => true;
        DataListACV.Filter = DataListFilter;
    }

    public bool DataListFilter(object item)
    {
        var query = (SubsceneModel) item;
        var name = query.Title ?? "";
        var server = query.Server ?? "";
        var txtSearch = MainPage.Instance.GetTxtSearch();
        return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            || server.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
    }
}
