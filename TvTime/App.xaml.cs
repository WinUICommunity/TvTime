// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Net;

namespace TvTime;

public partial class App : Application
{
    public App()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        this.InitializeComponent();
        CreateDirectory();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        ThemeHelper.Initialize(m_window, BackdropType.Mica);
        m_window.Activate();
    }

    private Window m_window;
}
