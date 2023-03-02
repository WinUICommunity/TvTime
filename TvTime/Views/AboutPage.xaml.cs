// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

namespace TvTime.Views;
public sealed partial class AboutPage : Page
{
    public string TvTimeVersion { get; set; }
    public AboutPage()
    {
        this.InitializeComponent();

        TvTimeVersion = $"TvTime v{VersionHelper.GetVersion()}";
    }
}
