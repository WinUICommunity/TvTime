using CommunityToolkit.Labs.WinUI;
using CommunityToolkit.WinUI.UI;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Animation;

using TvTime.Database;
using TvTime.Database.Tables;
using TvTime.Views.ContentDialogs;

namespace TvTime.ViewModels;
public partial class MediaViewModel : BaseViewModel, ITitleBarAutoSuggestBoxAware
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    [ObservableProperty]
    public ObservableCollection<TokenItem> tokenList;

    [ObservableProperty]
    public bool isServerStatusOpen;

    [ObservableProperty]
    public int progressBarValue;

    [ObservableProperty]
    public int progressBarMaxValue;

    [ObservableProperty]
    public bool progressBarShowError;

    private DispatcherTimer dispatcherTimer = new DispatcherTimer();
    private ServerType PageType;
    private int totalServerCount = 0;
    private List<ExceptionModel> exceptions;
    public IJsonNavigationViewService JsonNavigationViewService;
    public IThemeService themeService;

    public MediaViewModel(IJsonNavigationViewService jsonNavigationViewService, IThemeService themeService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
        this.themeService = themeService;
    }

    public override async void OnPageLoaded(object param)
    {
        IsActive = true;

        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                PageType = MediaPage.Instance.PageType;
                using var db = new AppDbContext();
                var tokens = db.MediaServers.Where(x => x.IsActive && x.ServerType == PageType).Select(x => new TokenItem { Content = GetServerUrlWithoutLeftAndRightPart(x.Server), Tag = x.Server });
                TokenList = new(tokens);
                TokenList.Insert(0, new TokenItem { Content = "All", IsSelected = true });
                if (!db.MediaServers.Any())
                {
                    IsStatusOpen = true;
                    StatusTitle = "Server not found";
                    StatusMessage = "No servers found, please add some servers first";
                    StatusSeverity = InfoBarSeverity.Warning;
                    IsServerStatusOpen = false;
                    var dialog = new GoToServerContentDialog();
                    dialog.ThemeService = themeService;
                    dialog.JsonNavigationViewService = JsonNavigationViewService;
                    await dialog.ShowAsync();
                }
                else
                {
                    await LoadLocalMedia();
                }
            });
        });

        IsActive = false;
    }

    public override void OnRefresh()
    {
        DownloadMediaIntoDatabase();
    }

    public override void NavigateToDetails(object sender)
    {
        base.NavigateToDetails(sender);

        var media = new BaseMediaTable
        {
            Server = descriptionText,
            Title = headerText,
            ServerType = PageType
        };

        JsonNavigationViewService.NavigateTo(typeof(MediaDetailPage), media, false, new DrillInNavigationTransitionInfo());
    }

    [RelayCommand]
    private async Task OnServerStatus()
    {
        var dialog = new ServerErrorsContentDialog();
        dialog.ThemeService = themeService;
        dialog.Exceptions = new(exceptions);

        await dialog.ShowAsync();
    }

    private async Task LoadLocalMedia()
    {
        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    IsActive = true;
                    IsStatusOpen = true;
                    StatusSeverity = InfoBarSeverity.Informational;
                    StatusTitle = "Loading Media, Please Wait...";

                    using var db = new AppDbContext();
                    List<BaseMediaTable> media = null;
                    switch (PageType)
                    {
                        case ServerType.Anime:
                            media = new(await db.Animes.ToListAsync());
                            break;
                        case ServerType.Movies:
                            media = new(await db.Movies.ToListAsync());
                            break;
                        case ServerType.Series:
                            media = new(await db.Series.ToListAsync());
                            break;
                    }

                    if (media != null && media.Any())
                    {
                        var myDataList = media.Where(x => x.Server != null);
                        DataList = new();
                        DataListACV = new AdvancedCollectionView(DataList, true);

                        using (DataListACV.DeferRefresh())
                        {
                            DataList.AddRange(myDataList);
                        }

                        DataListACV.SortDescriptions.Add(new SortDescription("Title", SortDirection.Ascending));
                    }

                    IsActive = false;
                    StatusSeverity = InfoBarSeverity.Success;
                    StatusTitle = $"Done, All Media Loaded! ({DataList?.Count})";
                    StatusMessage = "";
                }
                catch (Exception ex)
                {
                    IsActive = false;
                    StatusSeverity = InfoBarSeverity.Error;
                    StatusTitle = "Error";
                    StatusMessage = ex.Message;
                    Logger?.Error(ex, "MediaViewModel: LoadLocalMedia");
                }
            });
        });

        AutoHideStatusInfoBar(new TimeSpan(0, 0, 8));
    }

    private async void DownloadMediaIntoDatabase()
    {
        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    exceptions = new List<ExceptionModel>();
                    using var db = new AppDbContext();
                    if (!db.MediaServers.Any())
                    {
                        var dialog = new GoToServerContentDialog();
                        dialog.ThemeService = themeService;
                        dialog.JsonNavigationViewService = JsonNavigationViewService;
                        return;
                    }

                    await db.DeleteAndRecreateMediaTables(PageType.ToString());
                    IsServerStatusOpen = false;
                    IsActive = true;
                    var urls = await db.MediaServers.Where(x => x.ServerType == PageType && x.IsActive == true).ToListAsync();
                    IsStatusOpen = true;
                    StatusSeverity = InfoBarSeverity.Informational;
                    StatusTitle = "Please Wait...";
                    StatusMessage = "";
                    totalServerCount = urls.Count;
                    ProgressBarValue = 0;
                    ProgressBarMaxValue = urls.Count;

                    int index = 0;
                    foreach (var item in urls)
                    {
                        if (ApplicationHelper.IsNetworkAvailable())
                        {
                            try
                            {
                                ProgressBarShowError = false;
                                index++;
                                ProgressBarValue = index;
                                HtmlWeb web = new HtmlWeb();
                                HtmlDocument doc = await web.LoadFromWebAsync(item.Server);

                                StatusMessage = string.Format("Working on {0} - {1}/{2}", item.Title, index, urls.Count());
                                if (doc.DocumentNode.InnerHtml is null)
                                {
                                    continue;
                                }
                                string result = doc.DocumentNode?.InnerHtml?.ToString();

                                await GetServerDetails(result, item.Server, item.ServerType);

                                StatusSeverity = InfoBarSeverity.Informational;
                                StatusMessage = string.Format("{0} Saved", item.Title);
                            }
                            catch (Exception ex)
                            {
                                ProgressBarShowError = true;
                                exceptions.Add(new ExceptionModel(ex, item.Title, item.Server));
                                Logger?.Error(ex, "MediViewModel: DownloadMediaIntoDatabase");
                                continue;
                            }
                        }
                        else
                        {
                            IsActive = false;
                            IsStatusOpen = true;
                            StatusTitle = "No Network Connection";
                            StatusMessage = "Please Connect to Internet";
                            StatusSeverity = InfoBarSeverity.Error;
                            IsServerStatusOpen = false;
                            ProgressBarShowError = true;
                        }
                    }

                    if (ApplicationHelper.IsNetworkAvailable())
                    {
                        IsActive = false;
                        StatusTitle = "Done, We Updated our Database!";
                        StatusMessage = string.Format("Added {0}/{1} Servers", totalServerCount - exceptions.Count, totalServerCount);
                        StatusSeverity = InfoBarSeverity.Success;
                        if (exceptions.Any())
                        {
                            IsServerStatusOpen = true;
                        }
                        ProgressBarShowError = false;
                        ProgressBarValue = 0;
                        await LoadLocalMedia();
                    }
                }
                catch (Exception ex)
                {
                    IsActive = false;
                    StatusTitle = string.Format("Error: Added {0}/{1} Servers", totalServerCount - exceptions.Count, totalServerCount);
                    StatusMessage = ex.Message;
                    StatusSeverity = InfoBarSeverity.Error;
                    if (exceptions.Any())
                    {
                        IsServerStatusOpen = true;
                    }
                    ProgressBarShowError = true;
                    Logger?.Error(ex, "MediViewModel: DownloadMediaIntoDatabase2");
                }
            });
        });
    }

    public async Task GetDonyayeSerialServerDetails(string content, string server, ServerType serverType)
    {
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var rows = doc.DocumentNode.SelectNodes("//table[@class='table']/tbody/tr");
            var ignoreLinks = new List<string> { "../", "Home", "DonyayeSerial", "series", "movie" };
            using var db = new AppDbContext();

            foreach (var row in rows)
            {
                var nameNode = row.SelectSingleNode("./td[@class='n']/a/code");
                var dateNode = row.SelectSingleNode("./td[@class='m']/code");
                var linkNode = row.SelectSingleNode("./td[@class='n']/a");
                var sizeNode = row.SelectSingleNode("./td[@class='s']");
                if (linkNode != null && !ignoreLinks.Contains(linkNode.Attributes["href"].Value))
                {
                    var title = nameNode?.InnerText?.Trim();
                    var date = dateNode?.InnerText?.Trim();
                    var serverUrl = $"{server}{linkNode?.Attributes["href"]?.Value?.Trim()}";
                    var size = sizeNode?.InnerText?.Trim();

                    switch (PageType)
                    {
                        case ServerType.Anime:
                            await db.Animes.AddAsync(new AnimeTable(title, serverUrl, date, size, ServerType.Anime));
                            break;
                        case ServerType.Movies:
                            await db.Movies.AddAsync(new MovieTable(title, serverUrl, date, size, ServerType.Movies));
                            break;
                        case ServerType.Series:
                            await db.Series.AddAsync(new SeriesTable(title, serverUrl, date, size, ServerType.Series));
                            break;
                    }
                }
            }
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            IsActive = false;
            StatusTitle = "Error";
            StatusMessage = ex.Message;
            StatusSeverity = InfoBarSeverity.Error;
            if (exceptions.Any())
            {
                IsServerStatusOpen = true;
            }
            Logger?.Error(ex, "MediViewModel: GetDonyayeSerialServerDetails");
        }
    }

    public async Task GetAllServerDetails(string content, string server)
    {
        try
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            var nodes = doc?.DocumentNode?.SelectNodes("//a[@href]");

            if (nodes != null)
            {
                using var db = new AppDbContext();

                foreach (var node in nodes)
                {
                    var href = node?.GetAttributeValue("href", "");

                    var title = node?.InnerText;
                    var date = node?.NextSibling?.InnerText?.Trim();
                    if (string.IsNullOrEmpty(date))
                    {
                        date = node?.PreviousSibling?.InnerText?.Trim();
                    }

                    var dateAndSize = date?.Split("  ");
                    if (dateAndSize?.Length > 1)
                    {
                        date = dateAndSize[0];
                    }

                    if (ContinueIfWrongData(title, href, server, null))
                    {
                        continue;
                    }

                    string slash = string.Empty;
                    if (!server.EndsWith("/"))
                    {
                        slash = "/";
                    }

                    var finalServer = $"{server}{slash}{href}";

                    if (server.Contains("acemovies") && !server.Contains("dl5.dl1acemovies"))
                    {
                        var url = new Uri(server).GetLeftPart(UriPartial.Authority);
                        finalServer = $"{url}{href}";
                    }

                    if (server.Contains("dl5.dl1acemovies") && (title.Equals("Home") ||
                        title.Equals("dl") || title.Equals("English") || title.Equals("Series") ||
                        title.Equals("Movie") || title.Contains("Parent Directory")))
                    {
                        continue;
                    }

                    switch (PageType)
                    {
                        case ServerType.Anime:
                            await db.Animes.AddAsync(new AnimeTable(FixTitle(title), finalServer, date, null, ServerType.Anime));
                            break;
                        case ServerType.Movies:
                            await db.Movies.AddAsync(new MovieTable(FixTitle(title), finalServer, date, null, ServerType.Movies));
                            break;
                        case ServerType.Series:
                            await db.Series.AddAsync(new SeriesTable(FixTitle(title), finalServer, date, null, ServerType.Series));
                            break;
                    }
                }
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            IsActive = false;
            StatusTitle = "Error";
            StatusMessage = ex.Message;
            StatusSeverity = InfoBarSeverity.Error;
            if (exceptions.Any())
            {
                IsServerStatusOpen = true;
            }
            Logger?.Error(ex, "MediViewModel: GetAllServerDetails");
        }
    }

    public async Task GetServerDetails(string content, string server, ServerType serverType)
    {
        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                if (server.Contains("DonyayeSerial"))
                {
                    await GetDonyayeSerialServerDetails(content, server, serverType);
                }
                else
                {
                    await GetAllServerDetails(content, server);
                }
            });
        });
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
        Search(sender, args);
    }

    public void OnAutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Search(sender, null);
    }

    private async void Search(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        try
        {
            if (DataList != null)
            {
                bool useToken = false;
                using var db = new AppDbContext();
                var items = MediaPage.Instance.GetTokenViewSelectedItems();
                if (items.Any(token => token.Content.ToString().Equals("All")))
                {
                    if (args != null)
                    {
                        AutoSuggestBoxHelper.LoadSuggestions(sender, args, DataList.Select(x => x.Title).ToList());
                    }
                }
                else
                {
                    var filteredList = DataList.Where(x => items.Any(token => x.Server.Contains(token.Tag.ToString()))).Select(x => x.Title).ToList();
                    if (args != null)
                    {
                        AutoSuggestBoxHelper.LoadSuggestions(sender, args, filteredList);
                    }
                }

                List<BaseMediaTable> media = new();

                if (items.Any(token => token.Content.ToString().Equals("All")))
                {
                    switch (PageType)
                    {
                        case ServerType.Anime:
                            media = new(await db.Animes.Where(x => x.Title.ToLower().Contains(sender.Text.ToLower()) || x.Server.ToLower().Contains(sender.Text.ToLower())).ToListAsync());
                            break;
                        case ServerType.Movies:
                            media = new(await db.Movies.Where(x => x.Title.ToLower().Contains(sender.Text.ToLower()) || x.Server.ToLower().Contains(sender.Text.ToLower())).ToListAsync());
                            break;
                        case ServerType.Series:
                            media = new(await db.Series.Where(x => x.Title.ToLower().Contains(sender.Text.ToLower()) || x.Server.ToLower().Contains(sender.Text.ToLower())).ToListAsync());
                            break;
                    }
                }
                else
                {
                    useToken = true;
                    foreach (var token in items)
                    {
                        switch (PageType)
                        {
                            case ServerType.Anime:
                                var animeResult = db.Animes.Where(x => x.Server.ToLower().Contains(token.Tag.ToString().ToLower()) && (x.Title.ToLower().Contains(sender.Text.ToLower()) || x.Server.ToLower().Contains(sender.Text.ToLower())));
                                media.AddRange(animeResult);
                                break;
                            case ServerType.Movies:
                                var movieResult = db.Movies.Where(x => x.Server.ToLower().Contains(token.Tag.ToString().ToLower()) && (x.Title.ToLower().Contains(sender.Text.ToLower()) || x.Server.ToLower().Contains(sender.Text.ToLower())));
                                media.AddRange(movieResult);
                                break;
                            case ServerType.Series:
                                var seriesResult = db.Series.Where(x => x.Server.ToLower().Contains(token.Tag.ToString().ToLower()) && (x.Title.ToLower().Contains(sender.Text.ToLower()) || x.Server.ToLower().Contains(sender.Text.ToLower())));
                                media.AddRange(seriesResult);
                                break;
                        }
                    }
                }

                if (useToken)
                {
                    var myDataList = media.Where(x => x.Server != null);
                    DataList = new();
                    DataListACV = new AdvancedCollectionView(DataList, true);

                    using (DataListACV.DeferRefresh())
                    {
                        DataList.AddRange(myDataList);
                    }
                }
                else
                {
                    DataList = new(media);
                    DataListACV = new AdvancedCollectionView(DataList, true);
                }
            }
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, "MediaViewModel: Search");
            StatusTitle = "Error";
            StatusMessage = ex.Message;
            StatusSeverity = InfoBarSeverity.Error;
        }
    }
}
