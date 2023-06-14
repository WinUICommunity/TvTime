namespace TvTime.ViewModels;
public partial class BackupSettingViewModel : ObservableObject
{
    [ObservableProperty]
    public string statusText = "Status";

    [ObservableProperty]
    public InfoBarSeverity statusSeverity = InfoBarSeverity.Informational;

    [RelayCommand]
    private async void OnBackupServer()
    {
        try
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            savePicker.FileTypeChoices.Add("Json", new List<string>() { ".json" });
            savePicker.SuggestedFileName = "Servers";
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, WindowHelper.GetWindowHandleForCurrentWindow(App.Current.Window));

            StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                var servers = Settings.Servers;
                var json = JsonConvert.SerializeObject(servers);
                using (var outfile = new StreamWriter(file.Path))
                {
                    await outfile.WriteAsync(json);
                }
                StatusText = "Backup completed successfully";
                StatusSeverity = InfoBarSeverity.Success;
            }
        }
        catch (Exception ex)
        {
            StatusText = ex.Message;
            StatusSeverity = InfoBarSeverity.Error;
        }
    }

    [RelayCommand]
    private async void OnRestoreServer()
    {
        try
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".json");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, WindowHelper.GetWindowHandleForCurrentWindow(App.Current.Window));

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (var streamReader = File.OpenText(file.Path))
                {
                    var json = await streamReader.ReadToEndAsync();
                    var content = JsonConvert.DeserializeObject<ObservableCollection<ServerModel>>(json);
                    if (content is not null)
                    {
                        Settings.Servers = content;
                        StatusText = "Restore completed successfully";
                        StatusSeverity = InfoBarSeverity.Success;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            StatusText = ex.Message;
            StatusSeverity = InfoBarSeverity.Error;
        }
    }
}
