using CommunityToolkit.Labs.WinUI;

namespace TvTime.Views;
public sealed partial class SubtitleSettingPage : Page
{
    public string BreadCrumbBarItemText { get; set; }

    public SubtitleSettingPage()
    {
        this.InitializeComponent();
        Loaded += SubtitleSettingPage_Loaded;
    }

    private void SubtitleSettingPage_Loaded(object sender, RoutedEventArgs e)
    {
        foreach (var item in SubtitleLanguageCollection())
        {
            if (Settings.SubtitleLanguagesCollection.Any(x => x.Equals(item.Content)))
            {
               item.IsSelected = true;
            }
            else
            {
                item.IsSelected = false;
            }

            tokenView.Items.Add(item);
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        BreadCrumbBarItemText = e.Parameter as string;
    }

    private void tokenView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItemCount = e.AddedItems.Count;
        var unSelectedItemCount = e.RemovedItems.Count;

        if (selectedItemCount > 0)
        {
            var selectedItem = e.AddedItems[0] as TokenItem;
            var itemContent = selectedItem.Content.ToString();
            if (!Settings.SubtitleLanguagesCollection.Any(x=>x.Equals(itemContent)))
            {
                Settings.SubtitleLanguagesCollection.Add(itemContent);
            }
        }

        if (unSelectedItemCount > 0)
        {
            var unSelectedItem = e.RemovedItems[0] as TokenItem;
            var removeItem = Settings.SubtitleLanguagesCollection.FirstOrDefault(x => x.Equals(unSelectedItem.Content.ToString()));
            if (removeItem != null)
            {
                Settings.SubtitleLanguagesCollection.Remove(removeItem);
            }
        }
    }
}
