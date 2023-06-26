namespace TvTime.ViewModels;

public partial class ServerViewModel : ObservableRecipient
{
    [ObservableProperty]
    public bool isMediaServer;

    public string Title;

    public string Server;

    public ComboBoxItem ComboBoxSelectedItem;
    public int ComboBoxSelectedIndex;
    public bool TgServerIsActive;

    [ObservableProperty]
    public bool infoStatusIsOpen;

    [ObservableProperty]
    public string infoStatusMessage;

    [ObservableProperty]
    public InfoBarSeverity infoStausSeverity;

    [ObservableProperty]
    public ObservableCollection<ServerModel> dataList;

    [ObservableProperty]
    public AdvancedCollectionView dataListACV;

    public List<string> suggestList = new();
    private SortDescription currentSortDescription;

    [RelayCommand]
    private async void OnPageLoaded(bool isMediaServer)
    {
        this.IsMediaServer = isMediaServer;
        IsActive = true;
        await Task.Delay(150);
        DataList = IsMediaServer ? new(Settings.Servers) : (ObservableCollection<ServerModel>) new(Settings.SubtitleServers);
        DataListACV = new AdvancedCollectionView(DataList, true);
        currentSortDescription = new SortDescription("Title", SortDirection.Ascending);
        DataListACV.SortDescriptions.Add(currentSortDescription);
        suggestList = DataListACV.Select(x => ((ServerModel) x).Title).ToList();
        IsActive = false;
    }

    [RelayCommand]
    private void OnRemoveItem(object sender)
    {
        var btn = sender as Button;
        if (btn.DataContext != null)
        {
            var item = btn.DataContext as ServerModel;
            if (item != null)
            {
                DataList?.Remove(item);

                if (IsMediaServer)
                {
                    Settings.Servers = DataList;
                }
                else
                {
                    Settings.SubtitleServers = DataList;
                }

                InfoStausSeverity = InfoBarSeverity.Success;
                InfoStatusMessage = "Selected Server Removed Successfully";
                InfoStatusIsOpen = true;
            }
        }
    }

    [RelayCommand]
    private async void OnAddItem()
    {
        var inputDialog = CreateContentDialog("", "", -1, null, true);

        inputDialog.Title = "Add new Server";
        inputDialog.PrimaryButtonClick += (s, e) =>
        {
            if (IsMediaServer && ComboBoxSelectedItem == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Server))
            {
                var server = new ServerModel
                {
                    Title = Title.Trim(),
                    Server = Server.Trim(),
                    IsActive = TgServerIsActive,
                };

                if (IsMediaServer)
                {
                    server.ServerType = ApplicationHelper.GetEnum<ServerType>(ComboBoxSelectedItem?.Content?.ToString());
                }

                DataList?.Add(server);

                if (IsMediaServer)
                {
                    Settings.Servers = DataList;

                }
                else
                {
                    Settings.SubtitleServers = DataList;
                }

                InfoStausSeverity = InfoBarSeverity.Success;
                InfoStatusMessage = "New Server Added Successfully";
                InfoStatusIsOpen = true;
            }
            else
            {
                InfoStausSeverity = InfoBarSeverity.Error;
                InfoStatusMessage = "Server Can not be Added";
                InfoStatusIsOpen = true;
            }
        };

        await inputDialog.ShowAsyncQueueDraggable();
    }

    [RelayCommand]
    private async void OnUpdateItem(object sender)
    {
        var btn = sender as Button;
        if (btn.DataContext != null)
        {
            var item = btn.DataContext as ServerModel;
            var inputDialog = CreateContentDialog(item.Title, item.Server, (int) item.ServerType, null, item.IsActive);
            inputDialog.Title = "Update Server";
            inputDialog.PrimaryButtonClick += (s, e) =>
            {
                if (IsMediaServer && ComboBoxSelectedItem == null)
                {
                    return;
                }

                var index = DataList.IndexOf(item);

                if (index > -1 && !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Server))
                {
                    var serverModel = new ServerModel
                    {
                        Title = Title.Trim(),
                        Server = Server.Trim(),
                        IsActive = TgServerIsActive
                    };

                    if (IsMediaServer)
                    {
                        serverModel.ServerType = ApplicationHelper.GetEnum<ServerType>(ComboBoxSelectedItem?.Content?.ToString());
                    }

                    DataList[index] = serverModel;

                    if (IsMediaServer)
                    {
                        Settings.Servers = DataList;
                    }
                    else
                    {
                        Settings.SubtitleServers = DataList;
                    }

                    InfoStausSeverity = InfoBarSeverity.Success;
                    InfoStatusMessage = "Server Changed Successfully";
                    InfoStatusIsOpen = true;
                }
                else
                {
                    InfoStausSeverity = InfoBarSeverity.Error;
                    InfoStatusMessage = "Server Can not be Updated";
                    InfoStatusIsOpen = true;
                }
            };

            await inputDialog.ShowAsyncQueueDraggable();
        }
    }

    public bool DataListFilter(object item)
    {
        var query = (ServerModel) item;
        var name = query.Title ?? "";
        var tName = query.Server ?? "";
        var txtSearch = MainPage.Instance.GetTxtSearch();
        return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            || tName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
    }

    public void Search(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        AutoSuggestBoxHelper.LoadSuggestions(sender, args, suggestList);
        DataListACV.Filter = _ => true;
        DataListACV.Filter = DataListFilter;
    }

    private ContentDialog CreateContentDialog(string title, string server, int cmbSelectedIndex, object cmbSelectedItem, bool isServerActive)
    {
        var inputDialog = new ContentDialog
        {
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = "Save Changes",
            XamlRoot = App.Current.Window.Content.XamlRoot
        };

        var grid = new Grid
        {
            Padding = new Thickness(10),
            Style = Application.Current.Resources["GridPanel"] as Style
        };

        var stck = new StackPanel
        {
            Spacing = 16
        };

        var txtTitle = new TextBox
        {
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Header = "Title",
            PlaceholderText = "Server Title",
            Text = title
        };

        var txtServer = new TextBox
        {
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Header = "Server Url",
            PlaceholderText = "Server Url",
            Text = server
        };

        var cmbType = new ComboBox
        {
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Header = "Choose Series, Movie or Anime",
            PlaceholderText = "Choose Series, Movie or Anime",
            SelectedIndex = cmbSelectedIndex,
            SelectedItem = cmbSelectedItem,
        };

        cmbType.Items.Add(new ComboBoxItem { Content = "Series" });
        cmbType.Items.Add(new ComboBoxItem { Content = "Movie" });
        cmbType.Items.Add(new ComboBoxItem { Content = "Anime" });

        var tgActive = new ToggleSwitch
        {
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Header = "Activate Server",
            OffContent = "DeActive",
            OnContent = "Active",
            IsOn = isServerActive
        };

        stck.Children.Add(txtTitle);
        stck.Children.Add(txtServer);

        if (IsMediaServer)
        {
            stck.Children.Add(cmbType);
        }

        stck.Children.Add(tgActive);
        grid.Children.Add(stck);
        inputDialog.Content = grid;
        inputDialog.PrimaryButtonClick += (s, e) =>
        {
            this.Title = txtTitle.Text;
            this.Server = txtServer.Text;
            this.TgServerIsActive = tgActive.IsOn;

            if (IsMediaServer)
            {
                this.ComboBoxSelectedIndex = cmbType.SelectedIndex;
                this.ComboBoxSelectedItem = (ComboBoxItem) cmbType.SelectedItem;
            }
        };
        return inputDialog;
    }
}

