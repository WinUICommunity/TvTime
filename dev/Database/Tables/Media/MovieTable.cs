using System.ComponentModel.DataAnnotations.Schema;

namespace TvTime.Database.Tables;

[Table("Movies")]
public class MovieTable : BaseMediaTable
{
    public MovieTable(string title, string server, string dateTime, string fileSize, ServerType serverType)
        : base(title, server, dateTime, fileSize, serverType)
    {
    }
}
