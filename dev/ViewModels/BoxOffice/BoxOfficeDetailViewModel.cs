using HtmlAgilityPack;

using Microsoft.UI.Dispatching;

using TvTime.Tools.Common;

namespace TvTime.ViewModels;
public partial class BoxOfficeDetailViewModel : BaseViewModel, INavigationAware
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
                        HtmlDocument doc = await web.LoadFromWebAsync(boxOfficeItem.Link);
                        Title = doc.DocumentNode.SelectSingleNode("//meta[@name='title']").Attributes["content"].Value;
                        var description = doc.DocumentNode.SelectSingleNode("//meta[@name='description']").Attributes["content"].Value;
                        Synopsis = description.TextAfter("Synopsis:").Trim();
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
