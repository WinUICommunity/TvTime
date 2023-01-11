// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Constants = TvTime.Common.Constants;
using Newtonsoft.Json;
using HtmlAgilityPack;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using TvTime.Models;
using WinUICommunity.Common.Extensions;
using CommunityToolkit.WinUI.UI;

namespace TvTime.Views;
public sealed partial class SeriesPage : Page
{
    public SeriesPage()
    {
        this.InitializeComponent();
    }
}
