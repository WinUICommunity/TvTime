using System.Web;
using Nucs.JsonSettings;
using Nucs.JsonSettings.Autosave;
using Nucs.JsonSettings.Fluent;
using Nucs.JsonSettings.Modulation;
using Nucs.JsonSettings.Modulation.Recovery;
using TvTime.Models;

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace TvTime.Common;
public static class TvTimeHelper
{
    public static TvTimeConfig Settings = JsonSettings.Configure<TvTimeConfig>()
                               .WithRecovery(RecoveryAction.RenameAndLoadDefault)
                               .WithVersioning(VersioningResultAction.RenameAndLoadDefault)
                               .LoadNow()
                               .EnableAutosave();

    public static string GetMD5Hash(String strMsg)
    {
        string strAlgName = HashAlgorithmNames.Md5;
        IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(strMsg, BinaryStringEncoding.Utf8);

        HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(strAlgName);

        IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

        if (buffHash.Length != objAlgProv.HashLength)
        {
            throw new Exception("There was an error creating the hash");
        }

        string hex = CryptographicBuffer.EncodeToHexString(buffHash);

        return hex;
    }
    public static string GetSHA256Hash(String strMsg)
    {
        string strAlgName = HashAlgorithmNames.Sha256;
        IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(strMsg, BinaryStringEncoding.Utf8);

        HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(strAlgName);

        IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

        if (buffHash.Length != objAlgProv.HashLength)
        {
            throw new Exception("There was an error creating the hash");
        }

        string hex = CryptographicBuffer.EncodeToHexString(buffHash);

        return hex;
    }

    public static string GetDecodedStringFromHtml(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var decoded = HttpUtility.HtmlDecode(text);
        var result = decoded != text;
        return result ? decoded : text;
    }

    public static string RemoveSpecialWords(string stringToClean)
    {
        if (!string.IsNullOrEmpty(stringToClean))
        {
            Regex wordFilter = new Regex(Constants.FileNameRegex, RegexOptions.IgnoreCase);
            var cleaned = wordFilter.Replace(stringToClean, " ").Trim();
            cleaned = Regex.Replace(cleaned, "[ ]{2,}", " "); // remove space [More than 2 space] and replace with one space

            return cleaned.Trim();
        }else { return stringToClean; }
    }

    public static bool ExistDirectory(PageOrDirectoryType directoryType)
    {
        switch (directoryType)
        {
            case PageOrDirectoryType.Anime:
                return Directory.Exists(Constants.AnimesDirectoryPath);
            case PageOrDirectoryType.Movie:
                return Directory.Exists(Constants.MoviesDirectoryPath);
            case PageOrDirectoryType.Series:
                return Directory.Exists(Constants.SeriesDirectoryPath);
            default:
                return false;
        }
    }

    public static void DeleteDirectory(PageOrDirectoryType directoryType)
    {
        switch (directoryType)
        {
            case PageOrDirectoryType.Anime:
                Directory.Delete(Constants.AnimesDirectoryPath, true);
                CreateDirectory();
                break;
            case PageOrDirectoryType.Movie:
                Directory.Delete(Constants.MoviesDirectoryPath, true);
                CreateDirectory();
                break;
            case PageOrDirectoryType.Series:
                Directory.Delete(Constants.SeriesDirectoryPath, true);
                CreateDirectory();
                break;
        }
    }

    public static void CreateDirectory()
    {
        if (!Directory.Exists(Constants.SeriesDirectoryPath))
        {
            Directory.CreateDirectory(Constants.SeriesDirectoryPath);
        }
        if (!Directory.Exists(Constants.MoviesDirectoryPath))
        {
            Directory.CreateDirectory(Constants.MoviesDirectoryPath);
        }
        if (!Directory.Exists(Constants.AnimesDirectoryPath))
        {
            Directory.CreateDirectory(Constants.AnimesDirectoryPath);
        }
    }

    public static string GetFileSize(long size)
    {
        string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        const string formatTemplate = "{0}{1:0.#} {2}";

        if (size == 0)
        {
            return string.Format(formatTemplate, null, 0, sizeSuffixes[0]);
        }

        var absSize = Math.Abs((double) size);
        var fpPower = Math.Log(absSize, 1000);
        var intPower = (int) fpPower;
        var iUnit = intPower >= sizeSuffixes.Length
            ? sizeSuffixes.Length - 1
            : intPower;
        var normSize = absSize / Math.Pow(1000, iUnit);

        return string.Format(
            formatTemplate,
            size < 0 ? "-" : null, normSize, sizeSuffixes[iUnit]);
    }

    public static ObservableCollection<string> GenerateTextBlockStyles()
    {
        return new ObservableCollection<string>
        {
            "BaseTextBlockStyle",
            "BodyStrongTextBlockStyle",
            "BodyTextBlockStyle",
            "CaptionTextBlockStyle",
            "DisplayTextBlockStyle",
            "HeaderTextBlockStyle",
            "SubheaderTextBlockStyle",
            "SubtitleTextBlockStyle",
            "TitleLargeTextBlockStyle",
            "TitleTextBlockStyle"
        };
    }

    public static double GetFontSizeBasedOnTextBlockStyle(string styleName)
    {
        return styleName switch
        {
            "BaseTextBlockStyle" => (double) Application.Current.Resources["BodyTextBlockFontSize"],
            "BodyStrongTextBlockStyle" => (double) Application.Current.Resources["BodyTextBlockFontSize"],
            "BodyTextBlockStyle" => (double) Application.Current.Resources["BodyTextBlockFontSize"],
            "CaptionTextBlockStyle" => (double) Application.Current.Resources["CaptionTextBlockFontSize"],
            "DisplayTextBlockStyle" => (double) Application.Current.Resources["DisplayTextBlockFontSize"],
            "HeaderTextBlockStyle" => 46,
            "SubheaderTextBlockStyle" => 34,
            "SubtitleTextBlockStyle" => (double) Application.Current.Resources["SubtitleTextBlockFontSize"],
            "TitleLargeTextBlockStyle" => (double) Application.Current.Resources["TitleLargeTextBlockFontSize"],
            "TitleTextBlockStyle" => (double) Application.Current.Resources["TitleTextBlockFontSize"],
            _ => 14
        };
    }

    public static void CreateIMDBDetailsWindow(string query)
    {
        var window = new IMDBDetailsWindow(query);
        new ThemeManager(window, App.Current.ThemeManager.ThemeOptions);
        window.Activate();
    }
}
