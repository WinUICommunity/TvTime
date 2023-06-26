﻿using CommunityToolkit.Labs.WinUI;

using TvTime.ViewModels;

namespace TvTime.Views;
public sealed partial class ServersPage : Page
{
    public static ServersPage Instance { get; private set; }
    public ServerViewModel ViewModel { get; }
    public ServersPage()
    {
        ViewModel = App.Current.Services.GetService<ServerViewModel>();
        this.InitializeComponent();
        Instance = this;
        Loaded += ServersPage_Loaded;
    }

    private void ServersPage_Loaded(object sender, RoutedEventArgs e)
    {
        SegmentedServer.SelectedIndex = 0;
    }

    private void OnSegmentedServerChanged(object sender, SelectionChangedEventArgs e)
    {
        var segmented = sender as Segmented;
        if (ServerFrame != null)
        {
            if (segmented.SelectedIndex == 0)
            {
                ServerFrame.Navigate(typeof(MediaServersPage));
            }
            else
            {
                ServerFrame.Navigate(typeof(SubtitleServersPage));
            }
        }
    }
}
