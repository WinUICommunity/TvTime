namespace TvTime.Common;
public class Constants
{
    public static readonly string AppName = ApplicationHelper.GetProjectNameAndVersion();
    public static readonly string RootDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
    public static readonly string ServerDirectoryPath = Path.Combine(RootDirectoryPath, "MediaServers");
    public static readonly string SeriesDirectoryPath = Path.Combine(ServerDirectoryPath, "Series");
    public static readonly string MoviesDirectoryPath = Path.Combine(ServerDirectoryPath, "Movie");
    public static readonly string AnimesDirectoryPath = Path.Combine(ServerDirectoryPath, "Anime");

    public static readonly string AppConfigPath = Path.Combine(RootDirectoryPath, "AppConfig.json");

    public const string FileNameRegex = @"(?:hd(?:tv|cam|r)|-|/720p/|/480p/|/1080p/|- -|-  -|/|EmpireBestTv|AceMovies|AvaMovie|@Parsi_Ser|mp4|mkv|EBTV|@Gemovies|GM|AvaMovie|FB|M3|\.)";
    public const string SubtitleFileNameRegex = @"(?:hd(?:tv|cam|r)|e(?:xm|vo)|RMT|DDP?5(?:\.1)?|YTS|Turkish|VideoFlix|Gisaengchung|Korean|8CH|BluRay|-|XVid|A(?:c3|VS)|web(?:-?(?:rip|dl))?|fgt|mp3|cmrg|pahe|10bit|(?:720|480|1080)[pi]?|H\.?26[45]|x26[45]|\d{3,}MB|H(?:MAX|EVC)|PS(?:A|iG)|RARBG|[26]CH|(?:CAM)?Rip|RM(?:X|Team)|msd|sva|mkvcage|megusta|tbs|amz|shitbox|nitro|Mr(?:Movie|CS)|BWBP|NT[bG]|Atmos|MZABI|20(?:1\d|2[01])|\/|GalaxyRG|YTS(?:\.(?:LT|MX))?|DV|ION10|SYNCOPY|Phoenix|Minx|AFG|Cakes|@Gemovies|GM|AvaMovie|FB|M3|\.)";
    public const string IMDBTitleAPI = "http://www.omdbapi.com/?t={0}&apikey=2a59a17e";
    public const string IMDBBaseUrl = "https://www.imdb.com/title/{0}";
    public const string TVTIME_REPO = "https://github.com/winUICommunity/TvTime";
    public const string ALL_FILTER = "All";

    public const string DateTimeRegex = @"(?<date>(?:\d{1,2}\/\d{1,2}\/\d{4}|\d{1,4}\/\d{1,2}\/\d{2}|\d{1,2}-(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)-\d{4}|(?:January|February|March|April|May|June|July|August|September|October|November|December) \d{1,2}, \d{4}|\d{4}-[A-Za-z]{3}-\d{2} \d{2}:\d{2}))(?:\s+(?<time>\d{1,2}:\d{1,2}(?:[^\S\r\n]{0,}(?:AM|PM))?))?";

    public static readonly string[] FileExtensions = new string[] { ".mp4", ".wav", ".m4a", ".mp3", ".mkv", ".zip", ".rar", ".srt", ".aas" };

    public const string NotFoundOrExist = "Subtitles not found or server is unavailable, please try again!";
    public const string InternetIsNotAvailable = "Oh no! You're not connected to the internet.";

    public const string SubsceneSearchAPI = "{0}/subtitles/searchbytitle?query={1}&l=";
    public const string ESubtitleSearchAPI = "https://esubtitle.com/?s={0}";
    public const string WorldSubtitleSearchAPI = "http://worldsubtitle.site/?s={0}";
    public const string WorldSubtitlePageSearchAPI = "http://worldsubtitle.site/page/{0}?s=";
    public const string ISubtitleSearchAPI = "https://isubtitles.org/search?kwd={0}";
    public const string ISubtitleBaseUrl = "https://isubtitles.org";
}
