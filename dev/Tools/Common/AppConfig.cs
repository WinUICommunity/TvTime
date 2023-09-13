using Nucs.JsonSettings.Examples;
using Nucs.JsonSettings.Modulation;

namespace TvTime.Common;
public class AppConfig : NotifiyingJsonSettings, IVersionable
{
    [EnforcedVersion("4.0.0.0")]
    public virtual Version Version { get; set; } = new Version(4, 0, 0, 0);
    public override string FileName { get; set; } = Constants.AppConfigPath;

    private ObservableCollection<string> _SubtitleLanguagesCollection = new();
    public virtual ObservableCollection<string> SubtitleLanguagesCollection
    {
        get => _SubtitleLanguagesCollection;
        set
        {
            if (Equals(value, _SubtitleLanguagesCollection)) return;
            _SubtitleLanguagesCollection = value;
            OnPropertyChanged();
        }
    }

    public virtual bool UseDeveloperMode { get; set; }
    public virtual bool IsFileOpenInBrowser { get; set; }
    public virtual bool HasHyperLinkBorderThickness { get; set; }
    public virtual bool UseTruncateInHeader { get; set; }
    public virtual bool UseTruncateInDescription { get; set; } = true;
    public virtual bool UseTokenViewFilter { get; set; } = true;
    public virtual bool UseDoubleClickForNavigate { get; set; }
    public virtual bool UseIDMForDownloadFiles { get; set; }
    public virtual bool UseIDMForDownloadSubtitles { get; set; }
    public virtual bool UseDefaultRegexEnabled { get; set; } = true;

    public virtual string LastUpdateCheck { get; set; }
    public virtual string DescriptionTextBlockStyle { get; set; } = "BaseTextBlockStyle";
    public virtual string HeaderTextBlockStyle { get; set; } = "SubtitleTextBlockStyle";
    public virtual string FileNameRegex { get; set; } = Constants.FileNameRegex;
}

