using System.ComponentModel.DataAnnotations.Schema;

namespace TvTime.Database.Tables;

[Table("SubtitleServer")]
public class SubtitleServerTable : BaseServerTable
{
    public SubtitleServerTable(string title, string server, bool isActive, ServerType serverType = ServerType.Subtitle)
        : base(title, server, isActive, serverType)
    {
    }
}
