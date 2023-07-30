using System.Diagnostics;

using CommunityToolkit.Labs.WinUI;

using Nucs.JsonSettings;
using Nucs.JsonSettings.Autosave;
using Nucs.JsonSettings.Fluent;
using Nucs.JsonSettings.Modulation;
using Nucs.JsonSettings.Modulation.Recovery;

using TvTime.ViewModels;

using Windows.Storage.Pickers;

namespace TvTime.Common;
public static class TvTimeHelper
{
    public static TvTimeConfig Settings = JsonSettings.Configure<TvTimeConfig>()
                               .WithRecovery(RecoveryAction.RenameAndLoadDefault)
                               .WithVersioning(VersioningResultAction.RenameAndLoadDefault)
                               .LoadNow()
                               .EnableAutosave();

    public static TvTimeServerConfig ServerSettings = JsonSettings.Configure<TvTimeServerConfig>()
                               .WithRecovery(RecoveryAction.RenameAndLoadDefault)
                               .WithVersioning(VersioningResultAction.RenameAndLoadDefault)
                               .LoadNow()
                               .EnableAutosave();


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
        IThemeService ThemeService = new ThemeService();
        ThemeService.Initialize(window);
        ThemeService.ConfigBackdrop(BackdropType.Mica);
        ThemeService.ConfigElementTheme(ElementTheme.Default);
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

            if (selectedItem.Content.ToString().Equals(App.Current.Localizer.GetLocalizedString("Constants_AllFilter")) && selectedItem.IsSelected)
            {
                foreach (TokenItem item in token.Items)
                {
                    if (item.Content.ToString().Equals(App.Current.Localizer.GetLocalizedString("Constants_AllFilter")))
                    {
                        continue;
                    }
                    item.IsSelected = false;
                }

                viewModel.DataListACV.Filter = null;
            }
            else if (!selectedItem.Content.ToString().Equals(App.Current.Localizer.GetLocalizedString("Constants_AllFilter")))
            {
                foreach (TokenItem item in token.Items)
                {
                    if (item.Content.ToString().Equals(App.Current.Localizer.GetLocalizedString("Constants_AllFilter")) && item.IsSelected)
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
        StorageFolder folder = await ApplicationHelper.PickSingleFolderAsync(App.currentWindow);
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

    public static ObservableCollection<TvTimeLanguage> TvTimeLanguagesCollection()
    {
        return new ObservableCollection<TvTimeLanguage>
        {
            new TvTimeLanguage("English", "en-US", FlowDirection.LeftToRight),
            new TvTimeLanguage("Persian", "fa-IR", FlowDirection.RightToLeft)
        };
    }

    public static string[] GetAvailableLanguages()
    {
        return TvTimeLanguagesCollection().Select(x => x.LanguageCode).ToArray();
    }  
}
