using CommunityToolkit.WinUI.Controls;
using CommunityToolkit.WinUI.UI;

using HtmlAgilityPack;

using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json;
using TvTime.Database;
using TvTime.Database.Tables;
using TvTime.Views.ContentDialogs;

namespace TvTime.ViewModels;
public partial class SubsceneViewModel : BaseViewModel, ITitleBarAutoSuggestBoxAware
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    [ObservableProperty]
    public ObservableCollection<SegmentedItem> segmentedItems;

    [ObservableProperty]
    public string queryText;

    [ObservableProperty]
    public int segmentedItemSelectedIndex = -1;

    [ObservableProperty]
    public object segmentedItemSelectedItem = null;

    public IJsonNavigationViewService JsonNavigationViewService;

    private IThemeService themeService;
    public SubsceneViewModel(IJsonNavigationViewService jsonNavigationViewService, IThemeService themeService)
    {
        this.themeService = themeService;
        JsonNavigationViewService = jsonNavigationViewService;
        Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    using var db = new AppDbContext();
                    if (!db.SubtitleServers.Any())
                    {
                        using var streamReader = File.OpenText(await FileLoaderHelper.GetPath(Constants.DEFAULT_SUBTITLE_SERVER_PATH));
                        var json = await streamReader.ReadToEndAsync();
                        var content = JsonConvert.DeserializeObject<List<SubtitleServerTable>>(json);
                        if (content is not null)
                        {
                            await db.SubtitleServers.AddRangeAsync(content);
                        }
                        await db.SaveChangesAsync();
                    }
                    var segments = db.SubtitleServers.Where(x => x.IsActive)
                        .Select(x => new SegmentedItem { Content = GetServerUrlWithoutLeftAndRightPart(x.Server) });

                    SegmentedItems = new(segments);

                    var defaultTokenItem = SegmentedItems.FirstOrDefault(x => x.Content.ToString().Contains("subscene", StringComparison.OrdinalIgnoreCase));
                    SegmentedItemSelectedIndex = SegmentedItems.IndexOf(defaultTokenItem);

                    if (!db.SubtitleServers.Any())
                    {
                        IsStatusOpen = true;
                        StatusTitle = "No servers found, please add some servers first";
                        StatusMessage = "Server not found";
                        StatusSeverity = InfoBarSeverity.Warning;
                        var dialog = new GoToServerContentDialog();
                        dialog.JsonNavigationViewService = jsonNavigationViewService;
                        dialog.ThemeService = themeService;
                        await dialog.ShowAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex, "SubsceneViewModel:Ctor");
                }
            });
        });
    }

    public async void OnQuerySubmitted()
    {
        if (ApplicationHelper.IsNetworkAvailable())
        {
            try
            {
                using var db = new AppDbContext();
                if (!db.SubtitleServers.Any())
                {
                    var dialog = new GoToServerContentDialog();
                    dialog.JsonNavigationViewService = JsonNavigationViewService;
                    dialog.ThemeService = themeService;
                    await dialog.ShowAsync();
                    return;
                }

                IsActive = true;
                IsStatusOpen = true;
                StatusSeverity = InfoBarSeverity.Informational;
                StatusTitle = "Please Wait...";
                StatusMessage = "";

                if (!string.IsNullOrEmpty(QueryText))
                {
                    DataList = new();
                    var baseUrl = await db.SubtitleServers.Where(x => x.Server.Contains(((SegmentedItem)SegmentedItemSelectedItem).Content.ToString())).FirstOrDefaultAsync();
                    var url = string.Format(Constants.SubsceneSearchAPI, baseUrl?.Server, QueryText);
                    var web = new HtmlWeb();
                    var doc = await web.LoadFromWebAsync(url);

                    var titleCollection = doc?.DocumentNode?.SelectSingleNode("//div[@class='search-result']");
                    if (titleCollection == null || titleCollection.InnerText.Contains("No results found"))
                    {
                        ShowError("Subtitles not found or server is unavailable, please try again!");
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
                                        var server = ApplicationHelper.GetDecodedStringFromHtml(baseUrl.Server + subNode?.Attributes["href"]?.Value?.Trim());
                                        var subtitle = new SubtitleModel(name, server);
                                        subtitle.Description = count?.InnerText?.Trim();
                                        subtitle.GroupKey = GetSubtitleKey(i);

                                        DataList.Add(subtitle);
                                    }
                                }
                                else
                                {
                                    ShowError("Subtitles not found or server is unavailable, please try again!");
                                }
                            }
                        }

                        DataListACV.SortDescriptions.Add(new SortDescription("Title", SortDirection.Ascending));
                    }
                }
                IsActive = false;
            }
            catch (Exception ex)
            {
                Logger?.Error(ex, "SubsceneViewModel: OnQuerySubmitted");
                ShowError(ex.Message);
                IsActive = false;
            }
        }
        else
        {
            ShowError("Oh no! You're not connected to the internet.");
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

    public void OnAutoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        
    }

    public void OnAutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        this.QueryText = sender.Text;
        OnQuerySubmitted();
    }

    public override void NavigateToDetails(object sender)
    {
        base.NavigateToDetails(sender);

        var subtitle = new SubtitleModel(headerText, descriptionText);

        JsonNavigationViewService.NavigateTo(typeof(SubsceneDetailPage), subtitle);
    }
}
