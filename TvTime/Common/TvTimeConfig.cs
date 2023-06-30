using Nucs.JsonSettings.Examples;
using Nucs.JsonSettings.Modulation;

namespace TvTime.Common;
public class TvTimeConfig : NotifiyingJsonSettings, IVersionable
{
    [EnforcedVersion("2.1.0.0")]
    public virtual Version Version { get; set; } = new Version(2, 1, 0, 0);
    public override string FileName { get; set; } = Constants.AppConfigPath;

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

    public virtual bool IsFileOpenInBrowser { get; set; }
    public virtual bool IsSubtitleOpenInBrowser { get; set; }
    public virtual bool UseCustomFontSizeForDescription { get; set; } = false;
    public virtual bool UseCustomFontSizeForHeader { get; set; } = false;
    public virtual bool HasHyperLinkBorderThickness { get; set; } = false;
    public virtual bool UseTruncateInHeader { get; set; } = false;
    public virtual bool UseTruncateInDescription { get; set; } = false;
    public virtual bool UseTokenViewFilter { get; set; } = true;
    public virtual bool UseSound { get; set; } = true;
    public virtual bool UseDoubleClickForNavigate { get; set; } = false;
    public virtual bool UseAutoDecompressSubtitles { get; set; } = false;
    public virtual bool UseIDMForDownloadFiles { get; set; } = false;
    public virtual bool UseIDMForDownloadSubtitles { get; set; } = false;
    public virtual bool UseDefaultRegexEnabled { get; set; } = true;
    public virtual bool UseUserSpecifiedLocationForSubtitle { get; set; } = false;

    public virtual string LastUpdateCheck { get; set; }
    public virtual string DescriptionTextBlockStyle { get; set; } = "BaseTextBlockStyle";
    public virtual string HeaderTextBlockStyle { get; set; } = "SubtitleTextBlockStyle";
    public virtual string FileNameRegex { get; set; } = Constants.FileNameRegex;
    public virtual string DefaultSubtitleDownloadPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\";

    public virtual double DescriptionTextBlockFontSize { get; set; } = 12;
    public virtual double HeaderTextBlockFontSize { get; set; } = 20;

    public virtual DescriptionTemplateType DescriptionTemplate { get; set; } = DescriptionTemplateType.HyperLink;
    public virtual IconPackType IconPack { get; set; } = IconPackType.Glyph;
}
