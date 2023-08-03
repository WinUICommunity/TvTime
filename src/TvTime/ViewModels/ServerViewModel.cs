namespace TvTime.ViewModels;

public partial class ServerViewModel : BaseViewModel
{
    [ObservableProperty]
    public bool isMediaServer;

    public string Title;

    public string Server;

    public ComboBoxItem ComboBoxSelectedItem;
    public int ComboBoxSelectedIndex;
    public bool TgServerIsActive;

    public async override void OnPageLoaded(object isMediaServer)
    {
        this.IsMediaServer = Convert.ToBoolean(isMediaServer);
        IsActive = true;
        await Task.Delay(150);
        DataList = IsMediaServer ? new(ServerSettings.TVTimeServers) : new(ServerSettings.SubtitleServers);
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
                    ServerSettings.TVTimeServers = new ObservableCollection<ServerModel>(DataList.Cast<ServerModel>());
                }
                else
                {
                    ServerSettings.SubtitleServers = new ObservableCollection<ServerModel>(DataList.Cast<ServerModel>());
                }

                StatusSeverity = InfoBarSeverity.Success;
                StatusMessage = App.Current.ResourceHelper.GetString("ServerViewModel_RemoveStatus");
                IsStatusOpen = true;
            }
        }
    }

    [RelayCommand]
    private async Task OnAddItem()
    {
        var inputDialog = CreateContentDialog("", "", -1, null, true);

        inputDialog.Title = App.Current.ResourceHelper.GetString("ServerViewModel_AddStatusTitle");
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
                    ServerSettings.TVTimeServers = new ObservableCollection<ServerModel>(DataList.Cast<ServerModel>());

                }
                else
                {
                    ServerSettings.SubtitleServers = new ObservableCollection<ServerModel>(DataList.Cast<ServerModel>());
                }

                StatusSeverity = InfoBarSeverity.Success;
                StatusMessage = App.Current.ResourceHelper.GetString("ServerViewModel_AddedStatus");
                IsStatusOpen = true;
            }
            else
            {
                StatusSeverity = InfoBarSeverity.Error;
                StatusMessage = App.Current.ResourceHelper.GetString("ServerViewModel_CanNotAddStatus");
                IsStatusOpen = true;
            }
        };

        await inputDialog.ShowAsyncQueueDraggable();
    }

    [RelayCommand]
    private async Task OnUpdateItem(object sender)
    {
        var btn = sender as Button;
        if (btn.DataContext != null)
        {
            var item = btn.DataContext as ServerModel;
            var inputDialog = CreateContentDialog(item.Title, item.Server, (int) item.ServerType, null, item.IsActive);
            inputDialog.Title = App.Current.ResourceHelper.GetString("ServerViewModel_UpdateStatusTitle");
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
                        ServerSettings.TVTimeServers = new ObservableCollection<ServerModel>(DataList.Cast<ServerModel>());
                    }
                    else
                    {
                        ServerSettings.SubtitleServers = new ObservableCollection<ServerModel>(DataList.Cast<ServerModel>());
                    }

                    StatusSeverity = InfoBarSeverity.Success;
                    StatusMessage = App.Current.ResourceHelper.GetString("ServerViewModel_UpdateServerStatus");
                    IsStatusOpen = true;
                }
                else
                {
                    StatusSeverity = InfoBarSeverity.Error;
                    StatusMessage = App.Current.ResourceHelper.GetString("ServerViewModel_CanNotUpdateServerStatus");
                    IsStatusOpen = true;
                }
            };

            await inputDialog.ShowAsyncQueueDraggable();
        }
    }

    public override bool DataListFilter(object item)
    {
        var query = (ServerModel) item;
        var name = query.Title ?? "";
        var tName = query.Server ?? "";
        var txtSearch = MainPage.Instance.GetTxtSearch();
        return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            || tName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
    }

    private ContentDialog CreateContentDialog(string title, string server, int cmbSelectedIndex, object cmbSelectedItem, bool isServerActive)
    {
        var inputDialog = new ContentDialog
        {
            CloseButtonText = App.Current.ResourceHelper.GetString("ServerViewModel_CreateContentDialogCloseButton"),
            DefaultButton = ContentDialogButton.Primary,
            PrimaryButtonText = App.Current.ResourceHelper.GetString("ServerViewModel_CreateContentDialogPrimaryButton"),
            XamlRoot = App.currentWindow.Content.XamlRoot
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
            Header = App.Current.ResourceHelper.GetString("ServerViewModel_CreateContentDialogServerTitle"),
            PlaceholderText = App.Current.ResourceHelper.GetString("ServerViewModel_CreateContentDialogServerPlaceholder"),
            Text = title
        };

        var txtServer = new TextBox
        {
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Header = App.Current.ResourceHelper.GetString("ServerViewModel_CreateContentDialogServerHeader"),
            PlaceholderText = App.Current.ResourceHelper.GetString("ServerViewModel_CreateContentDialogServerUrlPlaceholder"),
            Text = server
        };

        var cmbType = new ComboBox
        {
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Header = App.Current.ResourceHelper.GetString("ServerViewModel_CreateContentDialogComboBoxHeader"),
            PlaceholderText = App.Current.ResourceHelper.GetString("ServerViewModel_CreateContentDialogComboBoxPlaceholder"),
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
            Header = App.Current.ResourceHelper.GetString("ServerViewModel_CreateContentDialogToggleSwitchHeader"),
            OffContent = App.Current.ResourceHelper.GetString("ServerViewModel_CreateContentDialogToggleSwitchOffContent"),
            OnContent = App.Current.ResourceHelper.GetString("ServerViewModel_CreateContentDialogToggleSwitchOnContent"),
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

