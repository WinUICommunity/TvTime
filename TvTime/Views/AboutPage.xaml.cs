// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Reflection;

namespace TvTime.Views;
public sealed partial class AboutPage : Page
{
    public string TvTimeVersion { get; set; }
    public AboutPage()
    {
        this.InitializeComponent();

        TvTimeVersion = $"TvTime v{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}";
    }
}
