namespace TvTime.Common;
public class Constants
{
    public static readonly string AppName = "TvTimeV1.0";
    public static readonly string RootDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
    public static readonly string ServerDirectoryPath = Path.Combine(RootDirectoryPath, "Servers");
    public static readonly string SeriesDirectoryPath = Path.Combine(ServerDirectoryPath, "Series");
    public static readonly string MoviesDirectoryPath = Path.Combine(ServerDirectoryPath, "Movie");
    public static readonly string AnimesDirectoryPath = Path.Combine(ServerDirectoryPath, "Anime");

    public static readonly string ServerFilePath = Path.Combine(ServerDirectoryPath, "Servers.json");
    public static readonly string AppConfigPath = Path.Combine(RootDirectoryPath, "AppConfig.json");

    public const string FileNameRegex = @"(?:hd(?:tv|cam|r)|-|/720p/|/480p/|/1080p/|- -|-  -|/|EmpireBestTv|AceMovies|AvaMovie|@Parsi_Ser|mp4|mkv|EBTV|@Gemovies|GM|AvaMovie|FB|M3|\.)";
    public const string IMDBTitleAPI = "http://www.omdbapi.com/?t={0}&apikey=2a59a17e";
    public const string IMDBBaseUrl = "https://www.imdb.com/title/{0}";

    public const string DateTimeRegex = @"(?<date>(?:\d{1,2}\/\d{1,2}\/\d{4}|\d{1,4}\/\d{1,2}\/\d{2}|\d{1,2}-(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)-\d{4}|(?:January|February|March|April|May|June|Jule|August|September|October|November|December) \d{1,2}, \d{4}))(?:\s+(?<time>\d{1,2}:\d{1,2}(?:[^\S\r\n]{0,}(?:AM|PM))?))?";

    public static readonly string[] FileExtensions = new string[] { ".mp4", ".wav", ".m4a", ".mp3", ".mkv" };
}
