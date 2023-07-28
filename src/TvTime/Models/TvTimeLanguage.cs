namespace TvTime.Models;
public class TvTimeLanguage
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string LanguageCode { get; set; }

    public TvTimeLanguage(string title, string languageCode)
    {
        this.Title = title;
        this.LanguageCode = languageCode;
    }
}
