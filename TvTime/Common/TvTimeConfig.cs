using Nucs.JsonSettings.Examples;
using Nucs.JsonSettings.Modulation;

namespace TvTime.Common;
public class TvTimeConfig : NotifiyingJsonSettings, IVersionable
{
    [EnforcedVersion("2.1.0.0")]
    public virtual Version Version { get; set; } = new Version(2, 1, 0, 0);
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
    public virtual string LastUpdateCheck { get; set; }
    public virtual string DescriptionTextBlockStyle { get; set; } = "BaseTextBlockStyle";
    public virtual double DescriptionTextBlockFontSize { get; set; } = 12;
    public virtual bool UseCustomFontSizeForDescription { get; set; } = false;
    public virtual string HeaderTextBlockStyle { get; set; } = "SubtitleTextBlockStyle";
    public virtual double HeaderTextBlockFontSize { get; set; } = 20;
    public virtual bool UseCustomFontSizeForHeader { get; set; } = false;
    public virtual bool HasHyperLinkBorderThickness { get; set; } = false;
    public virtual bool UseTruncateInHeader { get; set; } = false;
    public virtual bool UseTruncateInDescription { get; set; } = false;
    public virtual bool UseTokenViewFilter { get; set; } = true;
    public virtual DescriptionTemplateType DescriptionTemplate { get; set; } = DescriptionTemplateType.HyperLink;
    public virtual IconPackType IconPack { get; set; } = IconPackType.Glyph;
}
