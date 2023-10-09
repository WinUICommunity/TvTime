using CommunityToolkit.Labs.WinUI;
using CommunityToolkit.WinUI.UI;

using Microsoft.EntityFrameworkCore;

using TvTime.Database;
using TvTime.Database.Tables;

namespace TvTime.Views;
public sealed partial class MediaPage : Page
{
    public ServerType PageType { get; set; }
    public static MediaPage Instance { get; set; }
    public MediaViewModel ViewModel { get; }
    public MediaPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<MediaViewModel>();
        this.InitializeComponent();
        Instance = this;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        var pageType = (e.Parameter as DataItem).Parameter?.ToString();
        this.PageType = ApplicationHelper.GetEnum<ServerType>(pageType);
    }

    public List<TokenItem> GetTokenViewSelectedItems()
    {
        return Token.SelectedItems?.Cast<TokenItem>()?.ToList();
    }

    private void token_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (ViewModel.DataListACV == null)
            {
                return;
            }

            if (Token != null)
            {
                dynamic selectedItem = e.AddedItems?.Count > 0 ? e.AddedItems[0] as TokenItem : null;

                selectedItem ??= e.RemovedItems?.Count > 0 ? e.RemovedItems[0] as TokenItem : null;

                if (selectedItem == null)
                {
                    return;
                }

                if (Token.SelectedItems.Count == 0)
                {
                    var allItem = Token.Items[0] as TokenItem;
                    allItem.IsSelected = true;
                    selectedItem = allItem;
                }

                if (selectedItem.Content?.ToString().Equals("All") && selectedItem.IsSelected)
                {
                    foreach (TokenItem item in Token.Items)
                    {
                        if (item.Content.ToString().Equals("All"))
                        {
                            continue;
                        }
                        item.IsSelected = false;
                    }

                    OnTokenFilterAll();
                }
                else if (!selectedItem.Content?.ToString()?.Equals("All"))
                {
                    foreach (TokenItem item in Token.Items)
                    {
                        if (item.Content.ToString().Equals("All") && item.IsSelected)
                        {
                            item.IsSelected = false;
                        }
                        break;
                    }

                    OnTokenFilter();
                }
            }
        }
        catch (Exception ex)
        {
            ViewModel.IsServerStatusOpen = true;
            ViewModel.StatusTitle = "Error";
            ViewModel.StatusMessage = ex.Message;
            ViewModel.StatusSeverity = InfoBarSeverity.Error;
            Logger?.Error(ex, "MediaPage: TokenView SelectionChanged");
        }
    }

    private async void OnTokenFilterAll()
    {
        try
        {
            ViewModel.IsActive = true;
            using var db = new AppDbContext();
            List<BaseMediaTable> media = new();
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

            var myDataList = media.Where(x => x.Server != null);
            ViewModel.DataList = new();
            ViewModel.DataListACV = new AdvancedCollectionView(ViewModel.DataList, true);

            using (ViewModel.DataListACV.DeferRefresh())
            {
                ViewModel.DataList.AddRange(myDataList);
            }

            ViewModel.IsActive = false;
        }
        catch (Exception ex)
        {
            ViewModel.IsServerStatusOpen = true;
            ViewModel.StatusTitle = "Error";
            ViewModel.StatusMessage = ex.Message;
            ViewModel.StatusSeverity = InfoBarSeverity.Error;
            Logger?.Error(ex, "MediaPage: OnTokenFilterAll");
            ViewModel.IsActive = false;
        }
    }

    private void OnTokenFilter()
    {
        try
        {
            ViewModel.IsActive = true;
            var items = GetTokenViewSelectedItems().ToList();

            List<BaseMediaTable> media = new();

            using var db = new AppDbContext();
            foreach (var token in items)
            {
                switch (PageType)
                {
                    case ServerType.Anime:
                        var animeResult = db.Animes.Where(x => x.Server.ToLower().Contains(token.Tag.ToString().ToLower()));
                        if (animeResult != null)
                        {
                            media.AddRange(animeResult);
                        }
                        break;
                    case ServerType.Movies:
                        var movieResult = db.Movies.Where(x => x.Server.ToLower().Contains(token.Tag.ToString().ToLower()));
                        if (movieResult != null)
                        {
                            media.AddRange(movieResult);
                        }
                        break;
                    case ServerType.Series:
                        var seriesResult = db.Series.Where(x => x.Server.ToLower().Contains(token.Tag.ToString().ToLower()));
                        if (seriesResult != null)
                        {
                            media.AddRange(seriesResult);
                        }
                        break;
                }
            }

            var myDataList = media.Where(x => x.Server != null);
            ViewModel.DataList = new();
            ViewModel.DataListACV = new AdvancedCollectionView(ViewModel.DataList, true);

            using (ViewModel.DataListACV.DeferRefresh())
            {
                ViewModel.DataList.AddRange(myDataList);
            }

            ViewModel.IsActive = false;
        }
        catch (Exception ex)
        {
            ViewModel.IsServerStatusOpen = true;
            ViewModel.StatusTitle = "Error";
            ViewModel.StatusMessage = ex.Message;
            ViewModel.StatusSeverity = InfoBarSeverity.Error;
            Logger?.Error(ex, "MediaPage: OnTokenFilter");
            ViewModel.IsActive = false;
        }
    }

    private void ItemUserControl_Loading(FrameworkElement sender, object args)
    {
        var item = sender as ItemUserControl;
        item.ViewModel = ViewModel;
        item.SettingsCardCommand = ViewModel.SettingsCardCommand;
        item.SettingsCardDoubleClickCommand = ViewModel.SettingsCardDoubleClickCommand;
        var conv = new MediaHeaderIconConverter();
        var headerIcon = conv.Convert(PageType, null, null, null);
        if (headerIcon != null)
        {
            item.HeaderIcon = (BitmapIcon) headerIcon;
        }
    }
}
