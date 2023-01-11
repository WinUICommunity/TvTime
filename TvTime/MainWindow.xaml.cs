// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

namespace TvTime;

public sealed partial class MainWindow : Window
{
    public static MainWindow Instance { get; private set; }
    public MainWindow()
    {
        this.InitializeComponent();
        Instance = this;
        TitleBarHelper.Initialize(this, TitleTextBlock, AppTitleBar, LeftPaddingColumn, IconColumn, TitleColumn, LeftDragColumn, SearchColumn, RightDragColumn, RightPaddingColumn);

    }
}
