using System.ComponentModel.DataAnnotations.Schema;

namespace TvTime.Database.Tables;

[Table("Anime")]
public class AnimeTable : BaseMediaTable
{
    public AnimeTable(string title, string server, string dateTime, string fileSize, ServerType serverType)
        : base(title, server, dateTime, fileSize, serverType)
    {
    }
}
