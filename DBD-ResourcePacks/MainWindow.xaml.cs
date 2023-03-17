using DBD_ResourcePacks.Properties;
using DBD_ResourcePacks.UserControls;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DBD_ResourcePacks
{
    public partial class MainWindow : Window
    {
        public static readonly string JSON_PACKS = "packs.json";
        public static readonly string DIR_CACHE = "Cache/";
        public static readonly string DIR_CACHE_UI = "Cache/UI Images/";
        public static readonly string DIR_CACHE_BROWSE = "Cache/Browse Cache/";
        public static readonly string DIR_DOWNLOADED = "Downloaded/";
        public static readonly string TEMP_WRITING_JSON = "Writing Pack json";
        public static readonly string TEMP_DOWNLOADING_BANNER = "Downloading Banner";
        public static readonly string TEMP_DOWNLOADING_PACK = "Downloading Pack";
        public static readonly string TEMP_EXTRACTING_PACK = "Extracting Pack";
        public static readonly string TEMP_DELETING_PACK = "Deleting Pack";

        Dictionary<string, ResourcePack> _packRegistry = new();
        Dictionary<string, ResourcePack> _downloadedRegistry = new();

        List<string> _packsDownloaded;
        int _currentDownloadPage = 0;

        List<string> _packsBrowse;
        int _currentBrowsePage = 0;
        Dictionary<string, string> _browseState = new();

        public MainWindow()
        {
            switch (Settings.Default.ThemeSetting)
            {
                case 0: // Auto
                    if ((int)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", 1) == 1)
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

            Directory.CreateDirectory(DIR_CACHE);
            Directory.CreateDirectory(DIR_CACHE_UI);
            Directory.CreateDirectory(DIR_CACHE_BROWSE);
            Directory.CreateDirectory(DIR_DOWNLOADED);

            if (!File.Exists(JSON_PACKS))
            {
                // TODO Download
            }
            else
            {
                // TODO Check latest version
            }

            using (StreamReader r = new StreamReader(JSON_PACKS))
            {
                foreach (ResourcePack pack in JsonConvert.DeserializeObject<List<ResourcePack>>(r.ReadToEnd()))
                {
                    pack.PackState = "Download";
                    pack.PackActionable = true;
                    _packRegistry.Add(pack.uniqueKey, pack);
                }
                _packsBrowse = _packRegistry.Keys.ToList();
            }

            foreach (DirectoryInfo potentialPack in new DirectoryInfo(DIR_DOWNLOADED).EnumerateDirectories())
            {
                if (!File.Exists($"{potentialPack.FullName}/pack.json"))
                    continue;

                string json = "";
                using (StreamReader r = new StreamReader($"{potentialPack.FullName}/pack.json"))
                {
                    json = r.ReadToEnd();
                }

                if (json == "")
                    continue;

                ResourcePack pack = JsonConvert.DeserializeObject<ResourcePack>(json);

                if (pack == null)
                    continue;

                if (File.Exists($"{DIR_DOWNLOADED}{pack.uniqueKey}/temp.txt"))
                    DownloadPack(pack.uniqueKey);
                else
                {
                    _downloadedRegistry.Add(pack.uniqueKey, pack);

                    if (_packRegistry.ContainsKey(pack.uniqueKey))
                    {
                        ResourcePack registryPack = _packRegistry[pack.uniqueKey];
                        registryPack.PackState = "Downloaded";
                        registryPack.PackActionable = false;

                        if (registryPack.chapter > pack.chapter ||
                            (registryPack.chapter == pack.chapter && registryPack.packVersion > pack.packVersion))
                        {
                            pack.PackState = "Update";
                            pack.PackActionable = true;
                        }
                        else
                        {
                            pack.PackState = "Up To Date";
                            pack.PackActionable = false;
                        }
                    }
                    else
                    {
                        pack.PackState = "No Longer Available";
                        pack.PackActionable = false;
                    }
                }
            }
            _packsDownloaded = _downloadedRegistry.Keys.ToList();

            InitializeComponent();

            browse0.action.Click += new RoutedEventHandler(DownloadPack0);
            browse1.action.Click += new RoutedEventHandler(DownloadPack1);
            browse2.action.Click += new RoutedEventHandler(DownloadPack2);
            browse3.action.Click += new RoutedEventHandler(DownloadPack3);
            browse4.action.Click += new RoutedEventHandler(DownloadPack4);
            browse5.action.Click += new RoutedEventHandler(DownloadPack5);
            browse6.action.Click += new RoutedEventHandler(DownloadPack6);
            browse7.action.Click += new RoutedEventHandler(DownloadPack7);
            browse8.action.Click += new RoutedEventHandler(DownloadPack8);

            downloaded0.action.Click += new RoutedEventHandler(UpdatePack0);
            downloaded1.action.Click += new RoutedEventHandler(UpdatePack1);
            downloaded2.action.Click += new RoutedEventHandler(UpdatePack2);
            downloaded3.action.Click += new RoutedEventHandler(UpdatePack3);
            downloaded4.action.Click += new RoutedEventHandler(UpdatePack4);
            downloaded5.action.Click += new RoutedEventHandler(UpdatePack5);
            downloaded6.action.Click += new RoutedEventHandler(UpdatePack6);
            downloaded7.action.Click += new RoutedEventHandler(UpdatePack7);
            downloaded8.action.Click += new RoutedEventHandler(UpdatePack8);
        }

        private void LoadBrowsePage(int page)
        {
            _currentBrowsePage = page;

            browse0.Visibility = Visibility.Hidden;
            browse1.Visibility = Visibility.Hidden;
            browse2.Visibility = Visibility.Hidden;
            browse3.Visibility = Visibility.Hidden;
            browse4.Visibility = Visibility.Hidden;
            browse5.Visibility = Visibility.Hidden;
            browse6.Visibility = Visibility.Hidden;
            browse7.Visibility = Visibility.Hidden;
            browse8.Visibility = Visibility.Hidden;

            Pack packUI;
            for (int packNum = 0; packNum < 9; packNum++)
            {
                switch (packNum)
                {
                    case 0:
                        packUI = browse0;
                        break;
                    case 1:
                        packUI = browse1;
                        break;
                    case 2:
                        packUI = browse2;
                        break;
                    case 3:
                        packUI = browse3;
                        break;
                    case 4:
                        packUI = browse4;
                        break;
                    case 5:
                        packUI = browse5;
                        break;
                    case 6:
                        packUI = browse6;
                        break;
                    case 7:
                        packUI = browse7;
                        break;
                    case 8:
                        packUI = browse8;
                        break;
                    default:
                        return;
                }

                int index = page * 9 + packNum;
                if (index >= _packsBrowse.Count)
                    return;

                ResourcePack packInfo = _packRegistry[_packsBrowse[index]];
                packUI.PackInfo = packInfo;

                if (packInfo.bannerLink != "")
                {
                    string extension = Path.GetExtension(packInfo.bannerLink);
                    string uniqueFile = $"{DIR_CACHE_BROWSE}{packInfo.uniqueKey}_{Regex.Replace(packInfo.bannerLink.Substring(0, packInfo.bannerLink.Length - extension.Length), "[^A-Za-z0-9-_]", "")}{extension}";
                    if (File.Exists(uniqueFile))
                    {
                        packUI.banner.Source = new WriteableBitmap(new BitmapImage(new Uri(Path.Combine(Environment.CurrentDirectory, uniqueFile))));
                        packUI.banner.Visibility = 0;
                    }
                    else
                        DownloadBanner(packInfo.bannerLink, uniqueFile, packInfo, packUI);
                }
            }
        }

        void DownloadPack0(object sender, RoutedEventArgs e) { DownloadPack(browse0); }
        void DownloadPack1(object sender, RoutedEventArgs e) { DownloadPack(browse1); }
        void DownloadPack2(object sender, RoutedEventArgs e) { DownloadPack(browse2); }
        void DownloadPack3(object sender, RoutedEventArgs e) { DownloadPack(browse3); }
        void DownloadPack4(object sender, RoutedEventArgs e) { DownloadPack(browse4); }
        void DownloadPack5(object sender, RoutedEventArgs e) { DownloadPack(browse5); }
        void DownloadPack6(object sender, RoutedEventArgs e) { DownloadPack(browse6); }
        void DownloadPack7(object sender, RoutedEventArgs e) { DownloadPack(browse7); }
        void DownloadPack8(object sender, RoutedEventArgs e) { DownloadPack(browse8); }

        async void DownloadPack(Pack packUI)
        {
            ResourcePack resourcePack = packUI.PackInfo;
            resourcePack.PackActionable = false;
            resourcePack.PackState = "Downloading...";
            await DownloadPack(resourcePack.uniqueKey);
            _packsDownloaded.Add(resourcePack.uniqueKey);
        }

        private void LoadDownloadedPage(int page)
        {
            _currentDownloadPage = page;

            downloaded0.Visibility = Visibility.Hidden;
            downloaded1.Visibility = Visibility.Hidden;
            downloaded2.Visibility = Visibility.Hidden;
            downloaded3.Visibility = Visibility.Hidden;
            downloaded4.Visibility = Visibility.Hidden;
            downloaded5.Visibility = Visibility.Hidden;
            downloaded6.Visibility = Visibility.Hidden;
            downloaded7.Visibility = Visibility.Hidden;
            downloaded8.Visibility = Visibility.Hidden;

            Pack packUI;
            for (int packNum = 0; packNum < 9; packNum++)
            {
                switch (packNum)
                {
                    case 0:
                        packUI = downloaded0;
                        break;
                    case 1:
                        packUI = downloaded1;
                        break;
                    case 2:
                        packUI = downloaded2;
                        break;
                    case 3:
                        packUI = downloaded3;
                        break;
                    case 4:
                        packUI = downloaded4;
                        break;
                    case 5:
                        packUI = downloaded5;
                        break;
                    case 6:
                        packUI = downloaded6;
                        break;
                    case 7:
                        packUI = downloaded7;
                        break;
                    case 8:
                        packUI = downloaded8;
                        break;
                    default:
                        return;
                }

                int index = page * 9 + packNum;
                if (index >= _packsDownloaded.Count)
                    return;
                noPacksDownloaded.Visibility = Visibility.Hidden;

                if (!_downloadedRegistry.ContainsKey(_packsDownloaded[index]))
                    continue;

                ResourcePack packInfo = _downloadedRegistry[_packsDownloaded[index]];
                packUI.PackInfo = packInfo;

                /*if (_packRegistry.ContainsKey(packInfo.uniqueKey))
                {
                    ResourcePack registryPack = _packRegistry[packInfo.uniqueKey];
                    if (registryPack.chapter > packInfo.chapter || (registryPack.chapter == packInfo.chapter && registryPack.packVersion > packInfo.packVersion))
                    {
                        packUI.action.Content = "Update";
                        packUI.action.IsEnabled = true;
                    }
                    else
                    {
                        packUI.action.Content = "Up To Date";
                        packUI.action.IsEnabled = false;
                    }
                }
                else
                {
                    packUI.action.Content = "Pack No Longer Available";
                    packUI.action.IsEnabled = false;
                }*/

                string extension = Path.GetExtension(packInfo.bannerLink);
                string uniqueFile = $"{Regex.Replace(packInfo.bannerLink.Substring(0, packInfo.bannerLink.Length - extension.Length), "[^A-Za-z0-9-_]", "")}{extension}";
                if (File.Exists($"{DIR_DOWNLOADED}{packInfo.uniqueKey}/{uniqueFile}"))
                {
                    packUI.banner.Source = new WriteableBitmap(new BitmapImage(new Uri(
                        Path.Combine(Environment.CurrentDirectory,
                        $"{DIR_DOWNLOADED}{packInfo.uniqueKey}/{uniqueFile}"
                        ))));
                    packUI.banner.Visibility = 0;
                }
            }
        }

        void UpdatePack0(object sender, RoutedEventArgs e) { UpdatePack(downloaded0); }
        void UpdatePack1(object sender, RoutedEventArgs e) { UpdatePack(downloaded1); }
        void UpdatePack2(object sender, RoutedEventArgs e) { UpdatePack(downloaded2); }
        void UpdatePack3(object sender, RoutedEventArgs e) { UpdatePack(downloaded3); }
        void UpdatePack4(object sender, RoutedEventArgs e) { UpdatePack(downloaded4); }
        void UpdatePack5(object sender, RoutedEventArgs e) { UpdatePack(downloaded5); }
        void UpdatePack6(object sender, RoutedEventArgs e) { UpdatePack(downloaded6); }
        void UpdatePack7(object sender, RoutedEventArgs e) { UpdatePack(downloaded7); }
        void UpdatePack8(object sender, RoutedEventArgs e) { UpdatePack(downloaded8); }

        async void UpdatePack(Pack packUI)
        {
            ResourcePack pack = packUI.PackInfo;
            string key = pack.uniqueKey;
            pack.PackActionable = false;
            pack.PackState = "Updating...";
            _downloadedRegistry.Remove(key);
            await DownloadPack(key);
            if (!_packRegistry.ContainsKey(key))
                return;
            _packRegistry[key].PackState = "Updated";
        }

        public async void DownloadBanner(string url, string destinationFile, ResourcePack packInfo, Pack pack)
        {
            using (WebClient client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(url), destinationFile);
                if (packInfo != pack.PackInfo)
                    return;
                pack.banner.Source = new WriteableBitmap(new BitmapImage(new Uri(Path.Combine(Environment.CurrentDirectory, destinationFile))));
                pack.banner.Visibility = 0;
            }
        }
        public async Task DownloadPack(string uniqueKey)
        {
            if (!_packRegistry.ContainsKey(uniqueKey))
                return;
            ResourcePack pack = _packRegistry[uniqueKey];

            if (pack.downloadLink == "")
                return;

            Directory.CreateDirectory($"{DIR_DOWNLOADED}{pack.uniqueKey}");
            File.WriteAllText($"{DIR_DOWNLOADED}{pack.uniqueKey}/temp.txt", "Placeholder file whilst the pack is still downloading");
            File.WriteAllText($"{DIR_DOWNLOADED}{pack.uniqueKey}/pack.json", JsonConvert.SerializeObject(pack, Formatting.Indented));

            using (WebClient client = new WebClient())
            {
                pack.PackState = "Downloading Banner";
                string extension = Path.GetExtension(pack.bannerLink);
                string uniqueFile = $"{Regex.Replace(pack.bannerLink.Substring(0, pack.bannerLink.Length - extension.Length), "[^A-Za-z0-9-_]", "")}{extension}";
                if (pack.bannerLink != "" && !File.Exists($"{DIR_DOWNLOADED}{pack.uniqueKey}/{uniqueFile}"))
                    await client.DownloadFileTaskAsync(new Uri(pack.bannerLink), $"{DIR_DOWNLOADED}{pack.uniqueKey}/{uniqueFile}");
                pack.PackState = "Downloading Zip";
                await client.DownloadFileTaskAsync(new Uri(pack.downloadLink), $"{DIR_DOWNLOADED}{pack.uniqueKey}/pack.zip");

                if (Directory.Exists($"{DIR_DOWNLOADED}{pack.uniqueKey}/Pack"))
                {
                    pack.PackState = "Deleting Old Pack";
                    Directory.Delete($"{DIR_DOWNLOADED}{pack.uniqueKey}/Pack", true);
                }

                pack.PackState = "Unzipping";
                ZipFile.ExtractToDirectory($"{DIR_DOWNLOADED}{pack.uniqueKey}/pack.zip", $"{DIR_DOWNLOADED}{pack.uniqueKey}/Pack");
                pack.PackState = "Deleting Zip";
                File.Delete($"{DIR_DOWNLOADED}{pack.uniqueKey}/pack.zip");
            }
            pack.PackState = "Finishing";
            File.Delete($"{DIR_DOWNLOADED}{pack.uniqueKey}/temp.txt");
            _downloadedRegistry.Add(pack.uniqueKey, pack);
            pack.PackState = "Downloaded";
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is not TabControl tabControl)
                return;

            switch (tabControl.SelectedIndex)
            {
                case 0: // Installed
                    LoadDownloadedPage(_currentDownloadPage);
                    break;
                case 1: // Browse
                    LoadBrowsePage(_currentBrowsePage);
                    break;
                case 2: // Customise
                    break;
                default:
                    break;
            }
        }
    }
}
