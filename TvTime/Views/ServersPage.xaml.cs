// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.WinUI.UI;
using TvTime.Common;
using TvTime.Models;

namespace TvTime.Views;
public sealed partial class ServersPage : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ObservableCollection<ServerModel> serverList;
    public ObservableCollection<ServerModel> ServerList
    {
        get { return serverList; }
        set
        {
            if (value != this.serverList)
            {
                this.serverList = value;
                OnPropertyChanged();
            }
        }
    }
    private AdvancedCollectionView serverListACV;
    public AdvancedCollectionView ServerListACV
    {
        get { return serverListACV; }
        set
        {
            if (value != this.serverListACV)
            {
                this.serverListACV = value;
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

    private List<string> suggestList = new List<string>();
    private SortDescription currentSortDescription;

    public ServersPage()
    {
        this.InitializeComponent();
        Loaded += ServersPage_Loaded;
    }

    private void ServersPage_Loaded(object sender, RoutedEventArgs e)
    {
        IsActive = true;
        ServerList = new(Settings.Servers);
        ServerListACV = new AdvancedCollectionView(ServerList, true);
        currentSortDescription = new SortDescription("Title", SortDirection.Ascending);
        ServerListACV.SortDescriptions.Add(currentSortDescription);
        suggestList = ServerListACV.Select(x => ((ServerModel) x).Title).ToList();
        IsActive = false;
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

            ServerType = GeneralHelper.GetEnum<ServerType>(GetCurrentComboBoxItemContent())
        };
        var selectedItem = serverListView.SelectedItem as ServerModel;

        var localServers = Settings.Servers;

        var existItem = localServers.FirstOrDefault(x => x.Title.ToLower().Equals(selectedItem?.Title.ToLower()) && x.Server.ToLower().Equals(selectedItem?.Server.ToLower()));
        if (existItem is not null && tgEdit.IsOn)
        {
            var index = localServers.IndexOf(existItem);
            localServers[index] = new ServerModel
            {
                Title = txtTitle.Text,
                Server = txtServer.Text,
                ServerType = GeneralHelper.GetEnum<ServerType>(GetCurrentComboBoxItemContent())
            };
            Settings.Servers = localServers;
        }
        else
        {
            Settings.Servers.Add(server);
        }

        ServerList = new(Settings.Servers);
        IsActive = false;
        infoStatus.Severity = InfoBarSeverity.Success;
        infoStatus.Title = "New Server Added Successfully";
        infoStatus.IsOpen = true;

        txtServer.Text = string.Empty;
        txtTitle.Text = string.Empty;
    }

    private void btnRemove_Click(object sender, RoutedEventArgs e)
    {
        var localServers = Settings.Servers;

        var selectedItem = serverListView.SelectedItem as ServerModel;
        if (selectedItem is not null)
        {
            localServers.Remove(selectedItem);
            Settings.Servers = localServers;
            infoStatus.Severity = InfoBarSeverity.Success;
            infoStatus.Title = "Selected Server Removed Successfully";
            infoStatus.IsOpen = true;
            ServerList = new(Settings.Servers);
            txtServer.Text = string.Empty;
            txtTitle.Text = string.Empty;
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
        btnRemove.IsEnabled = serverListView.SelectedItems.Count > 0 ? true : false;
        var selectedItem = serverListView.SelectedItem as ServerModel;
        if (selectedItem != null)
        {
            txtTitle.Text = selectedItem.Title;
            txtServer.Text = selectedItem.Server;
            cmbType.SelectedItem = (int)selectedItem.ServerType;
        }
    }

    private void txtSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        AutoSuggestBoxHelper.LoadSuggestions(sender, args, suggestList);
        ServerListACV.Filter = _ => true;
        ServerListACV.Filter = DataListFilter;
    }
    private bool DataListFilter(object item)
    {
        var query = (ServerModel) item;
        var name = query.Title ?? "";
        var tName = query.Server ?? "";

        return name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)
            || tName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);
    }

    private void btnAddNew_Click(object sender, RoutedEventArgs e)
    {
        txtServer.Text = string.Empty;
        txtTitle.Text = string.Empty;
    }
}
