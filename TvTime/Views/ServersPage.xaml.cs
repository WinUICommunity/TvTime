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

    private string GetCurrentComboBoxItemContent()
    {
        var currentComboBoxItem = cmbType.SelectedItem as ComboBoxItem;
        return currentComboBoxItem.Content?.ToString();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        IsActive = true;
        var server = new ServerModel
        {
            Title = txtTitle.Text,
            Server = txtServer.Text,
            IsActive = tgActive.IsOn,
            ServerType = ApplicationHelper.GetEnum<ServerType>(GetCurrentComboBoxItemContent())
        };
        var selectedItem = serverListView.SelectedItem as ServerModel;

        var existItem = DataList.FirstOrDefault(x => x.Title.ToLower().Equals(selectedItem?.Title.ToLower()) && x.Server.ToLower().Equals(selectedItem?.Server.ToLower()));
        if (existItem is not null && tgEdit.IsOn)
        {
            var index = DataList.IndexOf(existItem);
            DataList[index] = new ServerModel
            {
                Title = txtTitle.Text,
                Server = txtServer.Text,
                ServerType = ApplicationHelper.GetEnum<ServerType>(GetCurrentComboBoxItemContent()),
                IsActive = tgActive.IsOn
            };
        }
        else
        {
            DataList?.Add(server);
        }

        Settings.Servers = DataList;

        IsActive = false;
        infoStatus.Severity = InfoBarSeverity.Success;
        infoStatus.Title = "New Server Added Successfully";
        infoStatus.IsOpen = true;

        txtServer.Text = string.Empty;
        txtTitle.Text = string.Empty;
        tgActive.IsOn = true;
    }

    private void btnRemove_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = serverListView.SelectedItem as ServerModel;
        if (selectedItem is not null)
        {
            DataList?.Remove(selectedItem);
            Settings.Servers = DataList;
            infoStatus.Severity = InfoBarSeverity.Success;
            infoStatus.Title = "Selected Server Removed Successfully";
            infoStatus.IsOpen = true;
            DataList = new(Settings.Servers);
            txtServer.Text = string.Empty;
            txtTitle.Text = string.Empty;
            tgActive.IsOn = true;
        }
        else
        {
            infoStatus.Severity = InfoBarSeverity.Error;
            infoStatus.Title = "Selected Server Cant be Removed";
            infoStatus.IsOpen = true;
        }
    }

    private void serverListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        btnRemove.IsEnabled = serverListView.SelectedItems.Count > 0;
        var selectedItem = serverListView.SelectedItem as ServerModel;
        if (selectedItem != null)
        {
            txtTitle.Text = selectedItem.Title;
            txtServer.Text = selectedItem.Server;
            cmbType.SelectedItem = cmbType.Items.OfType<ComboBoxItem>().FirstOrDefault(x => x.Tag.ToString() == ((int) selectedItem.ServerType).ToString());
            tgActive.IsOn = selectedItem.IsActive;
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

    private void btnAddNew_Click(object sender, RoutedEventArgs e)
    {
        txtServer.Text = string.Empty;
        txtTitle.Text = string.Empty;
        tgActive.IsOn = true;
    }
}
