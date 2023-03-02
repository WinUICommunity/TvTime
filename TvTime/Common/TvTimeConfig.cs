using System.Collections.ObjectModel;
using Nucs.JsonSettings.Examples;
using Nucs.JsonSettings.Modulation;
using TvTime.Models;

namespace TvTime.Common;
public class TvTimeConfig : NotifiyingJsonSettings, IVersionable
{
    [EnforcedVersion("1.1.0.0")]
    public virtual Version Version { get; set; } = new Version(1, 1, 0, 0);
    public override string FileName { get; set; } = Constants.AppConfigPath;

    private ObservableCollection<ServerModel> _Servers = new();
    public virtual ObservableCollection<ServerModel> Servers
    {
        get => _Servers;
        set
        {
            if (Equals(value, _Servers)) return;
            _Servers = value;
            OnPropertyChanged();
        }
    }

    public virtual bool IsFileOpenInBrowser { get; set; }
}
