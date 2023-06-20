namespace TvTime.Models;

public struct MediaItem
{
    public string Server { get; set; }
    public string Title { get; set; }
    public string FileSize { get; set; }
    public string DateTime { get; set; }
    public ServerType ServerType { get; set; }
}
