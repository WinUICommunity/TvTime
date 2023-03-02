// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using TvTime.Models;
using Windows.System;

namespace TvTime.Views;
public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
        ThemeHelper.SetComboBoxDefaultItem(cmbTheme);
    }

    private void cmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ThemeHelper.ComboBoxSelectionChanged(sender);
    }

    private async void SettingsCard_Click(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("ms-settings:personalization-colors"));
    }

    private async void btnBackup_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            savePicker.FileTypeChoices.Add("Json", new List<string>() { ".json" });
            savePicker.SuggestedFileName = "Servers";
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, WindowHelper.GetWindowHandleForCurrentWindow(MainWindow.Instance));

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                var servers = Settings.Servers;
                var json = JsonConvert.SerializeObject(servers);
                using (var outfile = new StreamWriter(file.Path))
                {
                    await outfile.WriteAsync(json);
                }
                infoBackup.Message = "Backup completed successfully";
                infoBackup.Severity = InfoBarSeverity.Success;
            }
        }
        catch (Exception ex)
        {
            infoBackup.Message = ex.Message;
            infoBackup.Severity = InfoBarSeverity.Error;
        }
    }

    private async void btnRestore_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".json");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, WindowHelper.GetWindowHandleForCurrentWindow(MainWindow.Instance));

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (var streamReader = File.OpenText(file.Path))
                {
                    var json = await streamReader.ReadToEndAsync();
                    var content = JsonConvert.DeserializeObject<ObservableCollection<ServerModel>>(json);
                    if (content is not null)
                    {
                        Settings.Servers = content;
                        infoBackup.Message = "Restore completed successfully";
                        infoBackup.Severity = InfoBarSeverity.Success;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            infoBackup.Message = ex.Message;
            infoBackup.Severity = InfoBarSeverity.Error;
        }
    }
}
