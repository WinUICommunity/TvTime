namespace TvTime.Common;
public class Constants
{
    public static readonly string AppName = ApplicationHelper.GetProjectNameAndVersion();
    public static readonly string RootDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);

    public static readonly string AppConfigPath = Path.Combine(RootDirectoryPath, "AppConfig.json");

    public const string FileNameRegex = @"(?:hd(?:tv|cam|r)|-|/720p/|/480p/|/1080p/|- -|-  -|/|EmpireBestTv|AceMovies|AvaMovie|@Parsi_Ser|mp4|mkv|@Gemovies|GM|AvaMovie|FB|M3|\.)";
    public const string IMDBTitleAPI = "http://www.omdbapi.com/?t={0}&apikey=2a59a17e";
    public const string IMDBBaseUrl = "https://www.imdb.com/title/{0}";
    public const string CineMaterialBaseUrl = "https://www.cinematerial.com";
    public const string CineMaterialBoxOffice = "https://www.cinematerial.com/titles/box-office";
    public const string CineMaterialPosters = "https://www.cinematerial.com/search?q={0}";

    public static readonly string[] FileExtensions = new string[] { ".mp4", ".wav", ".m4a", ".mp3", ".mkv", ".zip", ".rar", ".srt", ".aas" };

    public const string SubsceneSearchAPI = "{0}/subtitles/searchbytitle?query={1}&l=";
    public const string AppCenterKey = "9f0c2c2b-0910-4cf7-bdae-4c1841bdfb0f";
}
