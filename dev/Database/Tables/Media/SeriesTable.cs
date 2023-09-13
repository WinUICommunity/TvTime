using System.ComponentModel.DataAnnotations.Schema;

namespace TvTime.Database.Tables;

[Table("Series")]
public class SeriesTable : BaseMediaTable
{
    public SeriesTable(string title, string server, string dateTime, string fileSize, ServerType serverType)
        : base(title, server, dateTime, fileSize, serverType)
    {
    }
}
