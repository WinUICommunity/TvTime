// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.WinUI.UI;
using System.Runtime.CompilerServices;
using TvTime.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using WinUICommunity.Common.Extensions;
using TvTime.Common;
using WinUICommunity.SettingsUI.Controls;
using Windows.System;

namespace TvTime.Views;

public sealed partial class LocalUserControl : UserControl, INotifyPropertyChanged
{
    public string ViewType
    {
        get { return (string) GetValue(ViewTypeProperty); }
        set { SetValue(ViewTypeProperty, value); }
    }
    public static readonly DependencyProperty ViewTypeProperty =
        DependencyProperty.Register("ViewType", typeof(string), typeof(LocalUserControl), new PropertyMetadata(string.Empty));


    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool isActive;
    public bool IsActive
    {
        get { return isActive; }
        set
        {
            if (value != this.isActive)
            {
                this.isActive = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<LocalItem> dataList;
    public ObservableCollection<LocalItem> DataList
    {
        get { return dataList; }
        set
        {
            if (value != this.dataList)
            {
                this.dataList = value;
                OnPropertyChanged();
            }
        }
    }


    private AdvancedCollectionView dataListACV;
    public AdvancedCollectionView DataListACV
    {
        get { return dataListACV; }
        set
        {
            if (value != this.dataListACV)
            {
                this.dataListACV = value;
                OnPropertyChanged();
            }
        }
    }

    private List<string> suggestList = new List<string>();
    private List<string> existServer = new List<string>();

    private SortDescription currentSortDescription;

    public LocalUserControl()
    {
        this.InitializeComponent();
        DataContext = this;
        Loaded += LocalUserControl_Loaded;
    }

    private void LocalUserControl_Loaded(object sender, RoutedEventArgs e)
    {
        var files = Directory.GetFiles(Path.Combine(Constants.ServerDirectoryPath, ViewType), "*.txt");
        if (files.Count() > 0)
        {
            LoadLocalStorage();
        }
        else
        {
            DownloadServersOnLocalStorage();
        }

        if (Settings.Servers.Count == 0)
        {
            infoStatus.Title = "Server not found";
            infoStatus.Message = "Please add some Servers";
            infoStatus.Severity = InfoBarSeverity.Warning;
            btnServerStatus.Visibility = Visibility.Collapsed;
        }
    }

    private async void LoadLocalStorage()
    {
        infoStatus.IsOpen = true;
        infoStatus.Severity = InfoBarSeverity.Informational;
        infoStatus.Title = $"Loading Local {ViewType}...";
        var files = Directory.GetFiles(Path.Combine(Constants.ServerDirectoryPath, ViewType), "*.txt");
        if (files.Count() > 0)
        {
            List<LocalItem> temp = new List<LocalItem>();
            foreach (var file in files)
            {
                using var streamReader = File.OpenText(file);
                var json = await streamReader.ReadToEndAsync();
                var contents = JsonConvert.DeserializeObject<List<LocalItem>>(json);
                if (ViewType.Equals("Series"))
                {
                    var iranianSeries = contents.Where(x => x.Server.Contains("Series/Iranian")).
                    Select(i => new LocalItem
                    {
                        Server = i.Server.Replace(Path.GetFileName(i.Server), ""),
                        Title = GetSeriesTitle(i.Server.Replace(Path.GetFileName(i.Server), ""))
                                .Replace("/480p/","")
                                .Replace("/720p/","")
                                .Replace("/1080p/",""), ServerType = i.ServerType}).Distinct().FirstOrDefault();
                    contents.RemoveAll(u => u.Server.Contains("Series/Iranian"));
                    contents.Add(iranianSeries);
                }
                temp.AddRange(contents);
            }
            temp.RemoveAll(x => x.Server is null);
            DataList = new(temp);

            DataListACV = new AdvancedCollectionView(DataList, true);
            currentSortDescription = new SortDescription("Title", SortDirection.Ascending);
            DataListACV.SortDescriptions.Add(currentSortDescription);
            suggestList = DataListACV.Select(x => ((LocalItem) x).Title).ToList();
        }
        infoStatus.Title = $"{DataListACV?.Count} Local {ViewType} Added";
    }
    private string GetSeriesTitle(string title)
    {
        return title.Remove(0, title.IndexOf("Iranian/"))?.Replace("Iranian/", "")?.Replace("%20", " ");
    }
    private async void DownloadServersOnLocalStorage()
    {
        try
        {
            btnServerStatus.Visibility = Visibility.Collapsed;
            btnRefresh.IsEnabled = false;
            IsActive = true;
            var urls = Settings.Servers.Where(x => x.ServerType == GeneralHelper.GetEnum<ServerType>(ViewType)).ToList();
            infoStatus.IsOpen = true;
            infoStatus.Severity = InfoBarSeverity.Informational;
            infoStatus.Title = "Please Wait...";
            infoStatus.Message = "";

            int index = 0;
            existServer.Clear();
            foreach (var item in urls)
            {
                index++;

                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = await web.LoadFromWebAsync(item.Server);

                infoStatus.Message = $"Working on {item.Title} - {index}/{urls.Count()}";
                if (doc.DocumentNode.InnerHtml is null)
                {
                    continue;
                }
                string result = doc.DocumentNode?.InnerHtml?.ToString();
                infoStatus.Message = $"Parsing {item.Title}";

                var details = GetServerDetails(result, item);
                var json = JsonConvert.SerializeObject(details);
                infoStatus.Message = $"Serializing {item.Title}";

                var filePath = Path.Combine(Constants.ServerDirectoryPath, ViewType, $"{GetMD5Hash(item.Server)}.txt");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                using (var outfile = new StreamWriter(filePath))
                {
                    outfile.WriteLine(json);
                }

                // make sure data exist
                if (json.Length > 10)
                {
                    existServer.Add(item.Server);
                }

                infoStatus.Message = $"{item.Title} Saved";
            }
            IsActive = false;
            infoStatus.Title = "Updated Successfully";
            infoStatus.Message = "We Updated our Local Storage";
            infoStatus.Severity = InfoBarSeverity.Success;
            btnServerStatus.Visibility = Visibility.Visible;
            buttonInfoBadge.Value = existServer.Count;
            btnRefresh.IsEnabled = true;
        }
        catch (Exception ex)
        {
            btnRefresh.IsEnabled = true;
            buttonInfoBadge.Value = existServer.Count;
            IsActive = false;
            infoStatus.Title = "Error";
            infoStatus.Message = ex.Message;
            infoStatus.Severity = InfoBarSeverity.Error;
            btnServerStatus.Visibility = Visibility.Visible;
        }

        LoadLocalStorage();
    }

    public List<LocalItem> GetServerDetails(string content, ServerModel server)
    {
        List<LocalItem> list = new List<LocalItem>();

        MatchCollection m1 = Regex.Matches(content, @"(<a.*?>.*?</a>)",
            RegexOptions.Singleline);

        Regex dateTimeRegex = new Regex(Constants.DateTimeRegex, RegexOptions.IgnoreCase);
        MatchCollection dateTimeMatches = dateTimeRegex.Matches(content);

        int index = 0;
        foreach (Match m in m1)
        {
            string value = m.Groups[1].Value;
            LocalItem i = new LocalItem();

            Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
            RegexOptions.Singleline);
            if (m2.Success)
            {
                if (server.Server.Contains("freelecher"))
                {
                    var url = new Uri(server.Server).GetLeftPart(UriPartial.Authority);
                    i.Server = $"{url}{m2.Groups[1].Value}";
                }
                else
                {
                    i.Server = $"{server.Server}{m2.Groups[1].Value}";
                }
            }

            string t = Regex.Replace(value, @"\s*<.*?>\s*", "", RegexOptions.Singleline);
            i.Title = TvTimeHelper.RemoveSpecialWords(TvTimeHelper.GetDecodedStringFromHtml(t));

            if (i.Server.Equals($"{server.Server}../") || i.Title.Equals("[To Parent Directory]"))
            {
                continue;
            }

            if (dateTimeMatches.Count > 0 && index <= dateTimeMatches.Count)
            {
                var matchDate = dateTimeMatches[index].Value;
                i.DateTime = matchDate;
            }

            index++;
            i.ServerType = server.ServerType;
            list.Add(i);
        }
        return list;
    }
    private void btnRefresh_Click(object sender, RoutedEventArgs e)
    {
        CreateDirectory();
        DownloadServersOnLocalStorage();
    }

    private void btnServerStatus_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog contentDialog = new ContentDialog();
        contentDialog.XamlRoot = XamlRoot;
        contentDialog.Title = $"Server Status - {existServer.Count} Server(s) Added";
        var stck = new StackPanel
        {
            Spacing = 10,
            Margin = new Thickness(10)
        };

        foreach (var item in existServer)
        {
            var infoBar = new InfoBar();
            infoBar.Severity = InfoBarSeverity.Success;
            infoBar.Title = item;
            infoBar.IsOpen = true;
            infoBar.IsClosable = false;
            stck.Children.Add(infoBar);
        }

        contentDialog.Content = new ScrollViewer { Content = stck };

        contentDialog.PrimaryButtonText = "OK";
        contentDialog.ShowAsyncQueue();
    }

    private void txtSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        AutoSuggestBoxHelper.LoadSuggestions(sender, args, suggestList);
        DataListACV.Filter = _ => true;
        DataListACV.Filter = DataListFilter;
    }

    private bool DataListFilter(object item)
    {
        var query = (LocalItem) item;
        var name = query.Title ?? "";
        var tName = query.Server ?? "";

        return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            || tName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var setting = (sender as Button).Content as Setting;
        var item = new LocalItem
        {
            Server = setting?.Description?.ToString(),
            Title = setting?.Header,
            ServerType = GeneralHelper.GetEnum<ServerType>(ViewType)
        };
        ShellPage.Instance.Navigate(typeof(DetailPage), item);
    }
    private async void btnOpenDirectory_Click(object sender, RoutedEventArgs e)
    {
        var menuFlyout = (sender as MenuFlyoutItem);
        var localItem = (LocalItem) menuFlyout?.Tag;
        var server = localItem.Server?.ToString();
        await Launcher.LaunchUriAsync(new Uri(server));
    }
}
