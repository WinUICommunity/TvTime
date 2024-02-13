using System.Web;

using HtmlAgilityPack;

using Microsoft.UI.Dispatching;

using TvTime.Tools.Common;

namespace TvTime.ViewModels;
public partial class BoxOfficeDetailViewModel : BaseViewModel, INavigationAwareEx
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string synopsis;

    [ObservableProperty]
    private string directedBy;

    [ObservableProperty]
    private string writtenBy;

    [ObservableProperty]
    private string releaseDate;

    [ObservableProperty]
    private string runtime;

    private BoxOfficeModel boxOfficeItem;
    public void OnNavigatedFrom()
    {

    }

    public async void OnNavigatedTo(object parameter)
    {
        boxOfficeItem = (BoxOfficeModel) parameter;
        await GetInformation();
    }

    public override async void OnRefresh()
    {
        await GetInformation();
    }

    private async Task GetInformation()
    {
        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    IsStatusOpen = false;
                    IsActive = true;
                    HtmlWeb web = new HtmlWeb();
                    if (boxOfficeItem != null && !string.IsNullOrEmpty(boxOfficeItem.Link))
                    {
                        string boxOfficeItemLink = boxOfficeItem.Link;
                        if (!boxOfficeItem.Link.EndsWith("/info"))
                        {
                            boxOfficeItemLink = boxOfficeItem.Link + "/info";
                        }
                        HtmlDocument doc = await web.LoadFromWebAsync(boxOfficeItemLink);
                        if (doc != null)
                        {
                            var titleContent = doc.DocumentNode.SelectSingleNode("//meta[@name='title']").Attributes["content"]?.Value?.Trim();
                            Title = HttpUtility.HtmlDecode(titleContent);
                            var description = doc.DocumentNode.SelectSingleNode("//meta[@name='description']")?.Attributes["content"]?.Value;
                            var synopsisContent = description.TextAfter("Synopsis:")?.Trim();
                            Synopsis = HttpUtility.HtmlDecode(synopsisContent);
                            var directedByContent = doc.DocumentNode.SelectSingleNode("//th[contains(text(),'Directed by:')]/following-sibling::td")?.InnerText?.Trim();
                            DirectedBy = HttpUtility.HtmlDecode(directedByContent)?.Trim()?.Replace("\t", "")?.Replace("\n", "");
                            var writtenByContent = doc.DocumentNode.SelectSingleNode("//th[contains(text(),'Written by:')]/following-sibling::td")?.InnerText?.Trim();
                            WrittenBy = HttpUtility.HtmlDecode(writtenByContent)?.Trim()?.Replace("\t","")?.Replace("\n","");
                            var releaseDateContent = doc.DocumentNode.SelectSingleNode("//th[contains(text(),'Release date:')]/following-sibling::td")?.InnerText?.Trim();
                            ReleaseDate = HttpUtility.HtmlDecode(releaseDateContent);
                            var runtimeContent = doc.DocumentNode.SelectSingleNode("//th[contains(text(),'Runtime:')]/following-sibling::td")?.InnerText?.Trim();
                            Runtime = HttpUtility.HtmlDecode(runtimeContent);
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
}
