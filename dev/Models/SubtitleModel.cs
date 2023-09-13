using TvTime.Database.Tables;

namespace TvTime.Models;
public class SubtitleModel : BaseMediaTable
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Translator { get; set; }
    public string Language { get; set; }
    public bool IsActive { get; set; }

    public SubtitleModel(string title, string server, string dateTime, string fileSize, ServerType serverType)
        : base(title, server, dateTime, fileSize, serverType)
    {
    }

    public SubtitleModel(string title, string server)
    {
        this.Title = title;
        this.Server = server;
        this.ServerType = ServerType.Subtitle;
    }

    public SubtitleModel(string name, string title, string server, string description, string language, string translator)
    {
        this.Name = name;
        this.Title = title;
        this.Server = server;
        this.Description = description;
        this.Language = language;
        this.Translator = translator;
        this.ServerType = ServerType.Subtitle;
    }
}
