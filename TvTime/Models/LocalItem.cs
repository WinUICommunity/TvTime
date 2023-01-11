using TvTime.Common;

namespace TvTime.Models
{
    public struct LocalItem
    {
        public string Server { get; set; }
        public string Title { get; set; }
        public string FileSize { get; set; }
        public string DateTime { get; set; }
        public ServerType ServerType { get; set; }
    }
}
