using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TvTime.Database.Tables;
public class BaseServerTable
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Server { get; set; }
    public bool IsActive { get; set; }
    public ServerType ServerType { get; set; }

    public BaseServerTable(string title, string server, bool isActive, ServerType serverType = ServerType.Subtitle)
    {
        this.Title = title;
        this.Server = server;
        this.IsActive = isActive;
        this.ServerType = serverType;
    }
}
