using DBD_ResourcePackManager.Properties;
using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DBD_ResourcePackManager.Classes
{
    public class Constants
    {
        public const string GITHUB_OWNER = "Charzard4261";

        public const string REPO_PROGRAM   = "DBD-ResourcePackManager";
        public const string REPO_RESOURCES = "DBD-ResourcePackManager-Resources";
        public const string REPO_PACKS     = "DBD-ResourcePackManager-Packs";

        public const string LINK_SUBMISSION = "https://docs.google.com/forms/d/e/1FAIpQLSeoz3gwXeL4Ml_ziA1u8PWeClTN1xW4ejai6iWBiVq_I4kb0w/viewform";

        public const string DIR_CACHE         = "Cache";
        public const string DIR_CACHE_BROWSE  = "Cache/Browse Cache";
        public const string DIR_DEFAULT_ICONS = "Cache/Default Images";
        public const string DIR_PACKS         = "Packs";
        public const string DIR_DOWNLOADED    = "Downloaded";
        public const string DIR_RESOURCES     = "Resources";
        public const string FILE_SURVIVORS    = $"{DIR_RESOURCES}/survivors.json";
        public const string FILE_KILLERS      = $"{DIR_RESOURCES}/killers.json";
        public const string FILE_CHAPTERS     = $"{DIR_RESOURCES}/chapters.json";
        public const string FILE_CUSTOMISER   = $"custom.json";

        public const int PACKS_WIDTH  = 3;
        public const int PACKS_HEIGHT = 3;

        public const string FOLDER_ACTIONS        = "Actions";
        public const string FOLDER_PORTRAITS      = "CharPortraits";
        public const string FOLDER_CUSTOMIZATION  = "Customization";
        public const string FOLDER_DAILY_RITUALS  = "DailyRituals";
        public const string FOLDER_EMBLEMS        = "Emblems";
        public const string FOLDER_OFFERINGS      = "Favors";
        public const string FOLDER_PERKS          = "Perks";
        public const string FOLDER_HELP           = "Help";
        public const string FOLDER_HELPLOADING    = "HelpLoading";
        public const string FOLDER_ADDONS         = "ItemAddons";
        public const string FOLDER_ITEMS          = "Items";
        public const string FOLDER_PACKS          = "Packs";
        public const string FOLDER_POWERS         = "Powers";
        public const string FOLDER_STATUS_EFFECTS = "StatusEffects";

        public const string CONTAINS_PORTRAITS = "Portraits";
        public const string CONTAINS_PERKS     = "Perks";
        public const string CONTAINS_ITEMS     = "Items";
        public const string CONTAINS_ADDONS    = "Addons";
        public const string CONTAINS_POWERS    = "Powers";
        public const string CONTAINS_OFFERINGS = "Offerings";
        public const string CONTAINS_MISC_UI   = "Miscellaneous UI";


        public static async Task DownloadImage(string url, string directory)
        {
            string uniqueFile = $"{directory}/{GetUniqueFilename(url)}";

            if (File.Exists(uniqueFile))
                return;

            using (WebClient client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(url), uniqueFile);
            }
        }
        public static string GetUniqueFilename(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            return $"{Regex.Replace(filePath.Substring(0, filePath.Length - extension.Length), "[^A-Za-z0-9-_]", "")}{extension}";
        }
        public static BitmapImage LoadImage(string filePath)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(filePath);
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static bool IsValidGamePath(string folderPath)
        {
            if (!File.Exists($"{folderPath}{(folderPath.EndsWith("/") || folderPath.EndsWith("\\") ? "" : "/")}DeadByDaylight.exe"))
            {
                MessageBox.Show("Invalid Game Path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            Settings.Default.GameInstallationPath = folderPath;
            Settings.Default.Save();
            return true;
        }
        public static void UpdateTheme()
        {
            switch (Settings.Default.ThemeSetting)
            {
                case 0: // Auto
                    object? v = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", 1);
                    if ((v == null) || (int)v == 1)
                        Settings.Default.ThemeActual = "Light";
                    else
                        Settings.Default.ThemeActual = "Dark";
                    break;
                case 1: // Light
                    Settings.Default.ThemeActual = "Light";
                    break;
                case 2: // Dark
                    Settings.Default.ThemeActual = "Dark";
                    break;
                default:
                    break;
            }
        }
        public static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            DirectoryInfo source = new DirectoryInfo(sourcePath);

            foreach (DirectoryInfo info in source.GetDirectories())
            {
                Directory.CreateDirectory($"{targetPath}/{info.Name}");
                foreach (FileInfo file in info.GetFiles())
                    File.Copy(file.FullName, $"{targetPath}/{info.Name}/{file.Name}", true);
                CopyFilesRecursively(info.FullName, $"{targetPath}/{info.Name}");
            }
        }
        public static void CopyFile(string source, string target)
        {
            if (File.Exists(source))
                File.Copy(source, target, true);
        }
        public static void ExtractZipAndMoveUp(string zipPath, string destinationFolder)
        {
            ZipFile.ExtractToDirectory(zipPath, destinationFolder, true);
            foreach (DirectoryInfo dir in new DirectoryInfo(destinationFolder).GetDirectories())
            {
                foreach (FileInfo file in dir.GetFiles())
                    File.Move(file.FullName, $"{destinationFolder}\\{file.Name}", true);
                Directory.Delete(dir.FullName);
            }
        }
    }
}
