using CommunityToolkit.WinUI.UI;

using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Dispatching;

using Newtonsoft.Json;

using TvTime.Database;
using TvTime.Database.Tables;
using TvTime.Views.ContentDialogs;

namespace TvTime.ViewModels;
public partial class ServerViewModel : ObservableRecipient, ITitleBarAutoSuggestBoxAware
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    [ObservableProperty]
    public ObservableCollection<BaseServerTable> serverList;

    [ObservableProperty]
    public AdvancedCollectionView serverListACV;

    [ObservableProperty]
    private int segmentedSelectedIndex = 0;

    [ObservableProperty]
    private string infoBarTitle;

    [ObservableProperty]
    private bool infoBarIsOpen;

    [ObservableProperty]
    private InfoBarSeverity infoBarSeverity;

    private string tempQuery;
    public IThemeService themeService;
    public ServerViewModel(IThemeService themeService)
    {
        this.themeService = themeService;
    }

    [RelayCommand]
    private async Task OnPageLoaded()
    {
        try
        {
            IsActive = true;
            await Task.Run(() =>
            {
                dispatcherQueue.TryEnqueue(async () =>
                {
                    using var db = new AppDbContext();
                    if (SegmentedSelectedIndex == 0)
                    {
                        ServerList = new(await db.MediaServers.ToListAsync());
                    }
                    else
                    {
                        ServerList = new(await db.SubtitleServers.ToListAsync());
                    }
                    ServerListACV = new AdvancedCollectionView(ServerList, true);
                    ServerListACV.SortDescriptions.Add(new SortDescription("Title", SortDirection.Ascending));
                });
            });
            IsActive = false;
        }
        catch (Exception ex)
        {
            IsActive = false;
            InfoBarSeverity = InfoBarSeverity.Error;
            InfoBarTitle = ex.Message;
            InfoBarIsOpen = true;
            Logger?.Error(ex, "ServerViewModel: OnPageLoad");
        }
    }

    [RelayCommand]
    private async Task OnSegmentedSelectionChanged()
    {
        await OnPageLoaded();
    }

    [RelayCommand]
    private async Task OnAddServer()
    {
        var dialog = new ServerContentDialog();
        dialog.ViewModel = this;
        dialog.ServerTitle = string.Empty;
        dialog.ServerUrl = string.Empty;
        dialog.ServerActivation = true;
        dialog.CmbServerTypeSelectedItem = null;
        dialog.Title = "Add new Server";
        dialog.PrimaryButtonClick += OnAddServerPrimaryButton;
        
        await dialog.ShowAsync();
    }

    private async void OnAddServerPrimaryButton(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        try
        {
            var dialog = sender as ServerContentDialog;
            if (!string.IsNullOrEmpty(dialog.ServerTitle) && !string.IsNullOrEmpty(dialog.ServerUrl))
            {
                using var db = new AppDbContext();

                var baseServer = new BaseServerTable(dialog.ServerTitle.Trim(), dialog.ServerUrl.Trim(), dialog.ServerActivation);

                if (SegmentedSelectedIndex == 0)
                {
                    var type = GeneralHelper.GetEnum<ServerType>((dialog.CmbServerTypeSelectedItem as ComboBoxItem).Content?.ToString());
                    baseServer.ServerType = type;
                    await db.MediaServers.AddAsync(new MediaServerTable(baseServer.Title, baseServer.Server, baseServer.IsActive, baseServer.ServerType));
                }
                else
                {
                    await db.SubtitleServers.AddAsync(new SubtitleServerTable(baseServer.Title, baseServer.Server, baseServer.IsActive));
                }

                await db.SaveChangesAsync();
                InfoBarSeverity = InfoBarSeverity.Success;
                InfoBarTitle = "New Server Added Successfully";
                InfoBarIsOpen = true;
                await OnPageLoaded();
                Search(tempQuery);
            }
            else
            {
                InfoBarSeverity = InfoBarSeverity.Error;
                InfoBarTitle = "Server Can not be Added";
                InfoBarIsOpen = true;
            }
        }
        catch (Exception ex)
        {
            InfoBarSeverity = InfoBarSeverity.Error;
            InfoBarTitle = ex.Message;
            InfoBarIsOpen = true;
            Logger?.Error(ex, "ServerViewModel: OnAddServerPrimaryButton");
        }
    }

    [RelayCommand]
    private async Task OnUpdateServer(object dataContext)
    {
        try
        {
            if (dataContext != null)
            {
                var dialog = new ServerContentDialog();
                dialog.ViewModel = this;
                dialog.Title = "Update Server";

                var server = dataContext as BaseServerTable;
                dialog.ServerTitle = server.Title;
                dialog.ServerUrl = server.Server;
                dialog.ServerActivation = server.IsActive;

                if (SegmentedSelectedIndex == 0)
                {
                    dialog.CmbServerTypeSelectedItem = server.ServerType;
                }

                dialog.PrimaryButtonClick += async (s, e) =>
                {
                    if (string.IsNullOrEmpty(dialog.ServerTitle) && string.IsNullOrEmpty(dialog.ServerUrl))
                    {
                        InfoBarSeverity = InfoBarSeverity.Error;
                        InfoBarTitle = "Server Can not be Updated";
                        InfoBarIsOpen = true;
                        return;
                    }

                    using var db = new AppDbContext();

                    if (SegmentedSelectedIndex == 0)
                    {
                        var oldMediaServer = await db.MediaServers.Where(x => x.Title == server.Title && x.Server == server.Server && x.ServerType == server.ServerType).FirstOrDefaultAsync();
                        if (oldMediaServer != null)
                        {
                            var type = GeneralHelper.GetEnum<ServerType>((dialog.CmbServerTypeSelectedItem as ComboBoxItem).Content?.ToString());

                            oldMediaServer.Title = dialog.ServerTitle;
                            oldMediaServer.Server = dialog.ServerUrl;
                            oldMediaServer.ServerType = type;
                            oldMediaServer.IsActive = dialog.ServerActivation;
                        }
                    }
                    else
                    {
                        var oldSubtitleServer = await db.SubtitleServers.Where(x => x.Title == server.Title && x.Server == server.Server).FirstOrDefaultAsync();
                        oldSubtitleServer.Title = dialog.ServerTitle;
                        oldSubtitleServer.Server = dialog.ServerUrl;
                        oldSubtitleServer.IsActive = dialog.ServerActivation;
                    }

                    await db.SaveChangesAsync();

                    InfoBarSeverity = InfoBarSeverity.Success;
                    InfoBarTitle = "Server Changed Successfully";
                    InfoBarIsOpen = true;
                    await OnPageLoaded();
                    Search(tempQuery);
                };

                await dialog.ShowAsync();
            }
        }
        catch (Exception ex)
        {
            InfoBarSeverity = InfoBarSeverity.Error;
            InfoBarTitle = ex.Message;
            InfoBarIsOpen = true;
            Logger?.Error(ex, "ServerViewModel: OnUpdateServer");
        }
    }

    [RelayCommand]
    private async Task OnDeleteServer(object dataContext)
    {
        try
        {
            if (dataContext != null)
            {
                var server = dataContext as BaseServerTable;
                using var db = new AppDbContext();
                if (SegmentedSelectedIndex == 0)
                {
                    var delete = await db.MediaServers.Where(x => x.Title == server.Title && x.Server == server.Server && x.ServerType == server.ServerType).FirstOrDefaultAsync();
                    if (delete != null)
                    {
                        db.MediaServers.Remove(delete);
                    }
                }
                else
                {
                    var delete = await db.SubtitleServers.Where(x => x.Title == server.Title && x.Server == server.Server && x.ServerType == ServerType.Subtitle).FirstOrDefaultAsync();
                    if (delete != null)
                    {
                        db.SubtitleServers.Remove(delete);
                    }
                }

                await db.SaveChangesAsync();
                InfoBarSeverity = InfoBarSeverity.Success;
                InfoBarTitle = "Selected Server Deleted Successfully";
                InfoBarIsOpen = true;
                await OnPageLoaded();
                Search(tempQuery);
            }
        }
        catch (Exception ex)
        {
            InfoBarSeverity = InfoBarSeverity.Error;
            InfoBarTitle = ex.Message;
            InfoBarIsOpen = true;
            Logger?.Error(ex, "ServerViewModel: OnDeleteServer");
        }
    }

    [RelayCommand]
    private async Task OnLoadPredefinedServer()
    {
        try
        {
            var dialog = new LoadPredefinedContentDialog();
            dialog.ThemeService = themeService;
            dialog.PrimaryButtonClick += async (s, e) =>
            {
                await OnDeleteAllServer();

                var filePath = Constants.DEFAULT_MEDIA_SERVER_PATH;

                if (SegmentedSelectedIndex != 0)
                {
                    filePath = Constants.DEFAULT_SUBTITLE_SERVER_PATH;
                }

                using var streamReader = File.OpenText(await PathHelper.GetFilePath(filePath));
                var json = await streamReader.ReadToEndAsync();
                using var db = new AppDbContext();
                if (SegmentedSelectedIndex == 0)
                {
                    var content = JsonConvert.DeserializeObject<List<MediaServerTable>>(json);

                    if (content is not null)
                    {
                        await db.MediaServers.AddRangeAsync(content);
                    }
                }
                else
                {
                    var content = JsonConvert.DeserializeObject<List<SubtitleServerTable>>(json);
                    if (content is not null)
                    {
                        await db.SubtitleServers.AddRangeAsync(content);
                    }
                }

                await db.SaveChangesAsync();
                InfoBarTitle = "Predefined Servers Loaded Successfully";
                InfoBarSeverity = InfoBarSeverity.Success;
                InfoBarIsOpen = true;
                await OnPageLoaded();
            };

            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            InfoBarSeverity = InfoBarSeverity.Error;
            InfoBarTitle = ex.Message;
            InfoBarIsOpen = true;
            Logger?.Error(ex, "ServerViewModel: OnLoadPredefinedServers");
        }
    }

    [RelayCommand]
    private async Task OnDeleteAllServer()
    {
        try
        {
            using var db = new AppDbContext();
            if (SegmentedSelectedIndex == 0)
            {
                await db.DeleteAndRecreateServerTables("MediaServer");
            }
            else
            {
                await db.DeleteAndRecreateServerTables("SubtitleServer");
            }

            InfoBarSeverity = InfoBarSeverity.Success;
            InfoBarTitle = "All Servers Deleted Successfully";
            InfoBarIsOpen = true;
            await OnPageLoaded();
            Search(tempQuery);
        }
        catch (Exception ex)
        {
            InfoBarSeverity = InfoBarSeverity.Error;
            InfoBarTitle = ex.Message;
            InfoBarIsOpen = true;
            Logger?.Error(ex, "ServerViewModel: OnDeleteAllServer");
        }
    }

    public void OnAutoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        tempQuery = sender.Text;
        Search(sender.Text);
    }

    public void OnAutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        tempQuery = sender.Text;
        Search(sender.Text);
    }

    private void Search(string query)
    {
        try
        {
            if (ServerList != null && ServerList.Any())
            {
                ServerListACV.Filter = _ => true;
                ServerListACV.Filter = item =>
                {
                    var baseServer = (BaseServerTable) item;
                    var name = baseServer.Title ?? "";
                    var tName = baseServer.Server ?? "";
                    return name.Contains(query, StringComparison.OrdinalIgnoreCase)
                        || tName.Contains(query, StringComparison.OrdinalIgnoreCase);
                };
            }
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, "ServerViewModel:Search");
        }
    }
}
