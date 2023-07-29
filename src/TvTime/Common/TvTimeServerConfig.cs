using Nucs.JsonSettings.Examples;
using Nucs.JsonSettings.Modulation;

namespace TvTime.Common;

public class TvTimeServerConfig : NotifiyingJsonSettings, IVersionable
{
    [EnforcedVersion("2.3.0.0")]
    public virtual Version Version { get; set; } = new Version(2, 3, 0, 0);
    public override string FileName { get; set; } = Constants.ServerConfigPath;

    private ObservableCollection<ServerModel> _TVTimeServers = new();
    public virtual ObservableCollection<ServerModel> TVTimeServers
    {
        get => _TVTimeServers;
        set
        {
            if (Equals(value, _TVTimeServers)) return;
            _TVTimeServers = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<ServerModel> _SubtitleServers = new();
    public virtual ObservableCollection<ServerModel> SubtitleServers
    {
        get => _SubtitleServers;
        set
        {
            if (Equals(value, _SubtitleServers)) return;
            _SubtitleServers = value;
            OnPropertyChanged();
        }
    }
}
