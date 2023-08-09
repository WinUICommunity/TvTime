namespace TvTime.Models;
public interface ITvTimeModel
{
    public string Title { get; set; }
    public string Server { get; set; }
    public string Desc { get; set; }
    public string FileSize { get; set; }
    public string DateTime { get; set; }
    public string GroupKey { get; set; }
    public bool IsActive { get; set; }
    public ServerType ServerType { get; set; }
}
