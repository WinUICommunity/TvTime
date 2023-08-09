namespace TvTime.ViewModels;
public partial class BackupSettingViewModel : ObservableObject
{
    private bool isMediaServer;

    [ObservableProperty]
    public string statusText = App.Current.ResourceHelper.GetString("BackupSettingPage_InfoBarTitle/Title");

    [ObservableProperty]
    public InfoBarSeverity statusSeverity = InfoBarSeverity.Informational;

    [RelayCommand]
    private async void OnBackupServer(object isMediaServer)
    {
        try
        {
            this.isMediaServer = Convert.ToBoolean(isMediaServer);
            var fileName = $"TvTime-MediaServers-{DateTime.Now:yyyy-MM-dd HH-mm-ss}";

            if (!this.isMediaServer)
            {
                fileName = $"TvTime-SubtitleServers-{DateTime.Now:yyyy-MM-dd HH-mm-ss}";
            }

            var fileTypeChoices = new Dictionary<string, IList<string>>
            {
                { "Json", new List<string>() { ".json" } }
            };
            var file = await ApplicationHelper.PickSaveFileAsync(App.currentWindow, fileTypeChoices, fileName);

            if (file != null)
            {
                var servers = ServerSettings.TVTimeServers;

                if (!this.isMediaServer)
                {
                    servers = ServerSettings.SubtitleServers;
                }

                var json = JsonConvert.SerializeObject(servers, Formatting.Indented);
                using (var outfile = new StreamWriter(file.Path))
                {
                    await outfile.WriteAsync(json);
                }

                StatusText = App.Current.ResourceHelper.GetString("BackupSettingViewModel_BackupServerCompleted");
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
    private async void OnRestoreServer(object isMediaServer)
    {
        try
        {
            this.isMediaServer = Convert.ToBoolean(isMediaServer);

            var file = await ApplicationHelper.PickSingleFileAsync(App.currentWindow, new string[] { ".json" });
            if (file != null)
            {
                using var streamReader = File.OpenText(file.Path);
                var json = await streamReader.ReadToEndAsync();
                var content = JsonConvert.DeserializeObject<ObservableCollection<ServerModel>>(json);
                if (content is not null)
                {
                    if (this.isMediaServer)
                    {
                        ServerSettings.TVTimeServers = content;
                    }
                    else
                    {
                        ServerSettings.SubtitleServers = content;
                    }

                    StatusText = App.Current.ResourceHelper.GetString("BackupSettingViewModel_RestoreServerCompleted");
                    StatusSeverity = InfoBarSeverity.Success;
                }
            }
        }
        catch (Exception ex)
        {
            StatusText = ex.Message;
            StatusSeverity = InfoBarSeverity.Error;
        }
    }

    [RelayCommand]
    private async void OnBackupSettings()
    {
        try
        {
            var fileTypeChoices = new Dictionary<string, IList<string>>
            {
                { "Json", new List<string>() { ".json" } }
            };
            var suggestedFileName = $"TvTime-Settings-{DateTime.Now:yyyy-MM-dd HH-mm-ss}";

            var file = await ApplicationHelper.PickSaveFileAsync(App.currentWindow, fileTypeChoices, suggestedFileName);

            if (file != null)
            {
                var json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                using (var outfile = new StreamWriter(file.Path))
                {
                    await outfile.WriteAsync(json);
                }
                StatusText = App.Current.ResourceHelper.GetString("BackupSettingViewModel_BackupConfigCompleted");
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
    private async void OnRestoreSettings()
    {
        try
        {
            var file = await ApplicationHelper.PickSingleFileAsync(App.currentWindow, new string[] { ".json" });

            if (file != null)
            {
                using var streamReader = File.OpenText(file.Path);
                var json = await streamReader.ReadToEndAsync();
                var content = JsonConvert.DeserializeObject<TvTimeConfig>(json);
                if (content is not null)
                {
                    Settings = content;
                    Settings.Save();
                    StatusText = App.Current.ResourceHelper.GetString("BackupSettingViewModel_RestoreConfigCompleted");
                    StatusSeverity = InfoBarSeverity.Success;
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
