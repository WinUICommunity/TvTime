using System.Diagnostics;
using System.Reflection;
using System.Web;

using CommunityToolkit.Labs.WinUI;

using Nucs.JsonSettings;
using Nucs.JsonSettings.Autosave;
using Nucs.JsonSettings.Fluent;
using Nucs.JsonSettings.Modulation;
using Nucs.JsonSettings.Modulation.Recovery;

using TvTime.ViewModels;

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Pickers;
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
        }
        else { return stringToClean; }
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

    public static ObservableCollection<string> SubtitleLanguageCollection()
    {
        return new ObservableCollection<string>
        {
            "Persian",
            "English",
            "Albanian",
            "Arabic",
            "Bengali",
            "Brazillian",
            "Burmese",
            "Croatian",
            "Danish",
            "Dutch",
            "Finnish",
            "French",
            "German",
            "Hebrew",
            "Hindi",
            "Indonesian",
            "Italian",
            "Japanese",
            "Korean",
            "Malay",
            "Malayalam",
            "Morwegian",
            "Romanian",
            "Russian",
            "Serbian",
            "Spanish",
            "Swedish",
            "Tamil",
            "Thai",
            "Turkish",
            "Urdu",
            "Vietnamese",
            "Hungarian",
            "Portuguese"
        };
    }

    public async static Task<string> GetFilePath(string filePath, PathType pathType = PathType.Relative)
    {
        StorageFile file = null;
        if (ApplicationHelper.IsPackaged)
        {
            switch (pathType)
            {
                case PathType.Relative:
                    var sourceUri = new Uri("ms-appx:///" + filePath);
                    file = await StorageFile.GetFileFromApplicationUriAsync(sourceUri);
                    break;
                case PathType.Absolute:
                    file = await StorageFile.GetFileFromPathAsync(filePath);
                    break;
            }
        }
        else
        {
            switch (pathType)
            {
                case PathType.Relative:
                    var sourcePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), filePath));
                    file = await StorageFile.GetFileFromPathAsync(sourcePath);
                    break;
                case PathType.Absolute:
                    file = await StorageFile.GetFileFromPathAsync(filePath);
                    break;
            }
        }

        return file.Path;
    }

    public static string GetServerUrlWithoutLeftAndRightPart(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return url;
        }
        else
        {
            Uri uri = new Uri(url);
            string host = uri.Host;
            string[] parts = host.Split('.');
            return string.Join(".", parts.Take(parts.Length - 1));
        }
    }

    public static string GetServerUrlWithoutRightPart(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return url;
        }
        else
        {
            Uri uri = new Uri(url);
            return uri.Scheme + "://" + uri.Host;
        }
    }

    public static void TokenViewSelectionChanged(BaseViewModel viewModel, TokenView token, SelectionChangedEventArgs e, Action OnTokenFilter)
    {
        if (viewModel.DataListACV == null)
        {
            return;
        }

        if (token != null)
        {
            dynamic selectedItem = e.AddedItems.Count > 0 ? e.AddedItems[0] as TokenItem : null;

            if (selectedItem == null)
            {
                selectedItem = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as TokenItem : null;
            }

            if (selectedItem == null)
            {
                return;
            }

            if (token.SelectedItems.Count == 0)
            {
                var allItem = token.Items[0] as TokenItem;
                allItem.IsSelected = true;
                selectedItem = allItem;
            }

            if (selectedItem.Content.ToString().Equals(Constants.ALL_FILTER) && selectedItem.IsSelected)
            {
                foreach (TokenItem item in token.Items)
                {
                    if (item.Content.ToString().Equals(Constants.ALL_FILTER))
                    {
                        continue;
                    }
                    item.IsSelected = false;
                }

                viewModel.DataListACV.Filter = null;
            }
            else if (!selectedItem.Content.ToString().Equals(Constants.ALL_FILTER))
            {
                foreach (TokenItem item in token.Items)
                {
                    if (item.Content.ToString().Equals(Constants.ALL_FILTER) && item.IsSelected)
                    {
                        item.IsSelected = false;
                    }
                    break;
                }

                OnTokenFilter?.Invoke();
            }

            viewModel.DataListACV.RefreshFilter();
        }
    }

    public async static Task<string> OpenFolderPicker()
    {
        FolderPicker folderPicker = new();
        folderPicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, WindowHelper.GetWindowHandleForCurrentWindow(App.currentWindow));

        StorageFolder folder = await folderPicker.PickSingleFolderAsync();
        return folder is not null ? folder.Path : null;
    }

    public static void LaunchIDM(string idmPath, string link)
    {
        Process.Start(idmPath, $"/d \"{link}\"");
    }

    public async static void IDMNotFoundDialog()
    {
        ContentDialog contentDialog = new ContentDialog
        {
            XamlRoot = App.currentWindow.Content.XamlRoot,
            Title = "IDM not found",
            Content = new InfoBar
            {
                Margin = new Thickness(10),
                Severity = InfoBarSeverity.Error,
                Title = "IDM was not found on your system, please install it first",
                IsOpen = true,
                IsClosable = false
            },
            PrimaryButtonText = "Ok"
        };
        await contentDialog.ShowAsyncQueue();
    }

    public static bool IsUrlFile(string url)
    {
        Uri uri;
        if (Uri.TryCreate(url, UriKind.Absolute, out uri))
        {
            string extension = System.IO.Path.GetExtension(uri.AbsolutePath);
            if (!string.IsNullOrEmpty(extension))
            {
                return true;
            }
        }

        return false;
    }
}
