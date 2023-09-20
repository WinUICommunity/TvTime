namespace TvTime.Tools.Common;
public static class StringExtensions
{
    public static string TextAfter(this string value, string search)
    {
        return value.Substring(value.IndexOf(search) + search.Length);
    }
}
