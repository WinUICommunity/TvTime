using CommunityToolkit.WinUI.UI;

using HtmlAgilityPack;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Animation;

namespace TvTime.ViewModels;
public partial class BoxOfficeViewModel : BaseViewModel
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    [ObservableProperty]
    public ObservableCollection<BoxOfficeModel> boxOfficeData;

    [ObservableProperty]
    public string boxOfficeTitle;

    private IJsonNavigationViewService JsonNavigationViewService;

    public BoxOfficeViewModel(IJsonNavigationViewService jsonNavigationViewService)
    {
        JsonNavigationViewService = jsonNavigationViewService;
    }

    public async override void OnPageLoaded(object param)
    {
        await GetBoxOffice();
    }

    private async Task GetBoxOffice()
    {
        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    IsActive = true;
                    IsStatusOpen = false;
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument doc = await web.LoadFromWebAsync(Constants.CineMaterialBoxOffice);

                    BoxOfficeTitle = doc.DocumentNode.SelectSingleNode("//h1[@class='page-title']").InnerText;

                    var boxOfficeDiv = doc.DocumentNode.SelectSingleNode("//div[@class='box-office']");

                    if (boxOfficeDiv != null)
                    {
                        var allDivs = boxOfficeDiv.Descendants("div");

                        Dictionary<string, BoxOfficeModel> boxOffice = new Dictionary<string, BoxOfficeModel>();
                        foreach (var divElement in allDivs)
                        {
                            // Check if the div contains an anchor element
                            var anchorElement = divElement.SelectSingleNode(".//a");

                            if (anchorElement != null)
                            {
                                var spanElement = anchorElement.SelectSingleNode(".//span");
                                string title = spanElement.InnerText.Trim();

                                string href = anchorElement.GetAttributeValue("href", "")?.Trim();
                                string src = anchorElement.SelectSingleNode("img").GetAttributeValue("src", "")?.Trim();
                                string alt = anchorElement.SelectSingleNode("img").GetAttributeValue("alt", "")?.Trim();

                                boxOffice.AddIfNotExists(title, new BoxOfficeModel
                                {
                                    Link = $"{Constants.CineMaterialBaseUrl}{href}",
                                    ImageAlt = alt,
                                    ImageSrc = $"{Constants.CineMaterialBaseUrl}{src}",
                                    Title = title
                                });
                            }
                        }

                        BoxOfficeData = new();
                        DataListACV = new AdvancedCollectionView(BoxOfficeData, true);

                        using (DataListACV.DeferRefresh())
                        {
                            BoxOfficeData.AddRange(boxOffice.Values);
                        }
                    }
                    IsActive = false;
                }
                catch (Exception ex)
                {
                    Logger?.Error(ex, "BoxOfficeViewModel: GetBoxOffice");
                    IsStatusOpen = true;
                    StatusTitle = "Error";
                    StatusMessage = ex.Message;
                    StatusSeverity = InfoBarSeverity.Error;
                }
            });
        });
    }

    public override async void OnRefresh()
    {
        await GetBoxOffice();
    }

    [RelayCommand]
    private void OnItemInvoked(ItemsViewItemInvokedEventArgs args)
    {
        var item = args.InvokedItem as BoxOfficeModel;
        if (item != null)
        {
            JsonNavigationViewService.NavigateTo(typeof(BoxOfficeDetailPage), item, false, new DrillInNavigationTransitionInfo());
        }
    }
}
