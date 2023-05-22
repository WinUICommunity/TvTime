using System.Data;

namespace TvTime.Views;
public sealed partial class ServersPage : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ObservableCollection<ServerModel> dataList;
    public ObservableCollection<ServerModel> DataList
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

    public List<string> suggestList = new List<string>();
    private SortDescription currentSortDescription;

    public ServersPage()
    {
        this.InitializeComponent();
        Loaded += ServersPage_Loaded;
    }

    private void ServersPage_Loaded(object sender, RoutedEventArgs e)
    {
        IsActive = true;
        DataList = new(Settings.Servers);
        DataListACV = new AdvancedCollectionView(DataList, true);
        currentSortDescription = new SortDescription("Title", SortDirection.Ascending);
        DataListACV.SortDescriptions.Add(currentSortDescription);
        suggestList = DataListACV.Select(x => ((ServerModel) x).Title).ToList();
        IsActive = false;
    }
    public void Search(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        AutoSuggestBoxHelper.LoadSuggestions(sender, args, suggestList);
        DataListACV.Filter = _ => true;
        DataListACV.Filter = DataListFilter;
    }

    private void btnRemove_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        if (btn.DataContext != null)
        {
            var item = btn.DataContext as ServerModel;
            if (item != null)
            {
                DataList?.Remove(item);
                Settings.Servers = DataList;
                infoStatus.Severity = InfoBarSeverity.Success;
                infoStatus.Title = "Selected Server Removed Successfully";
                infoStatus.IsOpen = true;
                DataList = new(Settings.Servers);
            }
        }
    }

    public bool DataListFilter(object item)
    {
        var query = (ServerModel) item;
        var name = query.Title ?? "";
        var tName = query.Server ?? "";
        var txtSearch = MainWindow.Instance.GetTxtSearch();
        return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            || tName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
    }

    private async void btnAddNew_Click(object sender, RoutedEventArgs e)
    {
        txtTitle.Text = string.Empty;
        txtServer.Text = string.Empty;
        
        inputDialog.XamlRoot = MainWindow.Instance.Content.XamlRoot;
        inputDialog.PrimaryButtonClick += (s, e) =>
        {
            var currentComboBoxItem = cmbType.SelectedItem as ComboBoxItem;
            if (!string.IsNullOrEmpty(txtTitle.Text) && !string.IsNullOrEmpty(txtServer.Text) && currentComboBoxItem !=null)
            {
                var server = new ServerModel
                {
                    Title = txtTitle.Text.Trim(),
                    Server = txtServer.Text.Trim(),
                    IsActive = tgActive.IsOn,
                    ServerType = ApplicationHelper.GetEnum<ServerType>(currentComboBoxItem.Content?.ToString())
                };

                DataList?.Add(server);
                Settings.Servers = DataList;
                infoStatus.Severity = InfoBarSeverity.Success;
                infoStatus.Title = "New Server Added Successfully";
                infoStatus.IsOpen = true;
            }
            else
            {
                infoStatus.Severity = InfoBarSeverity.Error;
                infoStatus.Title = "Server Can not be Added";
                infoStatus.IsOpen = true;
            }
        };
        await inputDialog.ShowAsyncQueue();
    }

    private async void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        if (btn.DataContext != null)
        {
            var item = btn.DataContext as ServerModel;
            inputDialog.XamlRoot = MainWindow.Instance.Content.XamlRoot;
            txtTitle.Text = item.Title;
            txtServer.Text = item.Server;
            tgActive.IsOn = item.IsActive;
            cmbType.SelectedIndex = (int) item.ServerType;
            inputDialog.PrimaryButtonClick += (s, e) =>
            {
                var currentComboBoxItem = cmbType.SelectedItem as ComboBoxItem;
                var index = DataList.IndexOf(item);
                if (index > -1 && !string.IsNullOrEmpty(txtTitle.Text) && !string.IsNullOrEmpty(txtServer.Text) && currentComboBoxItem != null)
                {

                    DataList[index] = new ServerModel
                    {
                        Title = txtTitle.Text.Trim(),
                        Server = txtServer.Text.Trim(),
                        IsActive = tgActive.IsOn,
                        ServerType = ApplicationHelper.GetEnum<ServerType>(currentComboBoxItem.Content?.ToString())
                    };
                    Settings.Servers = DataList;
                    infoStatus.Severity = InfoBarSeverity.Success;
                    infoStatus.Title = "Server Changed Successfully";
                    infoStatus.IsOpen = true;
                }
                else
                {
                    infoStatus.Severity = InfoBarSeverity.Error;
                    infoStatus.Title = "Server Can not be Updated";
                    infoStatus.IsOpen = true;
                }
            };
            await inputDialog.ShowAsyncQueue();
        }
    }
}
