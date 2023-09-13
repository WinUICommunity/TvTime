using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TvTime.Database.Tables;
public class BaseMediaTable
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Server { get; set; }
    public string DateTime { get; set; }
    public string FileSize { get; set; }
    public string GroupKey { get; set; }
    public ServerType ServerType { get; set; }

    public BaseMediaTable()
    {
        
    }

    public BaseMediaTable(string title, string server, string dateTime, string fileSize, ServerType serverType)
    {
        this.Title = title;
        this.Server = server;
        this.DateTime = dateTime;
        this.FileSize = fileSize;
        this.ServerType = serverType;
    }
}
