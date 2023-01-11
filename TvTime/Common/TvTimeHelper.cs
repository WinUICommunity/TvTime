using Nucs.JsonSettings.Modulation.Recovery;
using Nucs.JsonSettings.Modulation;
using Nucs.JsonSettings;
using Nucs.JsonSettings.Fluent;
using Nucs.JsonSettings.Autosave;
using System.Text;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using System.Text.RegularExpressions;
using System.Web;
using TvTime.Models;

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
        Regex wordFilter = new Regex(Constants.FileNameRegex, RegexOptions.IgnoreCase);
        var cleaned = wordFilter.Replace(stringToClean, " ").Trim();
        cleaned = Regex.Replace(cleaned, "[ ]{2,}", " "); // remove space [More than 2 space] and replace with one space

        return cleaned.Trim();
    }

    public static void CreateDirectory()
    {
        if (Directory.Exists(Constants.SeriesDirectoryPath))
        {
            Directory.Delete(Constants.SeriesDirectoryPath, true);
        }
        if (Directory.Exists(Constants.MoviesDirectoryPath))
        {
            Directory.Delete(Constants.MoviesDirectoryPath, true);
        }
        if (Directory.Exists(Constants.AnimesDirectoryPath))
        {
            Directory.Delete(Constants.AnimesDirectoryPath, true);
        }

        Directory.CreateDirectory(Constants.SeriesDirectoryPath);
        Directory.CreateDirectory(Constants.MoviesDirectoryPath);
        Directory.CreateDirectory(Constants.AnimesDirectoryPath);
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
}
