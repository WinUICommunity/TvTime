using System.ComponentModel.DataAnnotations.Schema;

namespace TvTime.Database.Tables;

[Table("MediaServer")]
public class MediaServerTable : BaseServerTable
{
    public MediaServerTable(string title, string server, bool isActive, ServerType serverType)
        : base(title, server, isActive, serverType)
    {
    }
    
}
