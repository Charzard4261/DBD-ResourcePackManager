using DBD_ResourcePacks.Classes;
using DBD_ResourcePacks.Properties;
using DBD_ResourcePacks.UserControls;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DBD_ResourcePacks
{
    public partial class MainWindow : Window
    {
        public static readonly string DIR_CACHE = "Cache/";
        public static readonly string DIR_CACHE_UI = "Cache/UI Images/";
        public static readonly string DIR_CACHE_BROWSE = "Cache/Browse Cache/";
        public static readonly string DIR_DOWNLOADED = "Downloaded/";
        public static readonly string DIR_RESOURCES = "Resources/";
        public static readonly string DIR_RESOURCES_DEFAULT_ICONS = "Resources/Default Images/";
        public static readonly string FILE_PACKS = "packs.json";
        public static readonly string FILE_SURVIVORS = $"{DIR_RESOURCES}survivors.json";
        public static readonly string FILE_KILLERS = $"{DIR_RESOURCES}killers.json";

        public static readonly int PACKS_WIDTH = 4;
        public static readonly int PACKS_HEIGHT = 4;

        #region Packs
        Dictionary<string, ResourcePack> _packRegistry = new();
        Dictionary<string, ResourcePack> _downloadedRegistry = new();

        List<PackUC> _downloadedPackUCs = new();
        List<string> _downloadedPacks;
        int _currentDownloadPage = 0;

        List<PackUC> _browsePackUCs = new();
        List<string> _browsePacks;
        int _currentBrowsePage = 0;
        #endregion

        #region Customise
        // Data
        Dictionary<string, Survivor> _survivors = new();
        List<Perk> _commonSurvivorPerks = new();
        Dictionary<string, Killer> _killers = new();
        List<Perk> _commonKillerPerks = new();

        // UI
        Dictionary<string, CharacterUC> _survivorUCs = new();
        Dictionary<string, CharacterUC> _killerUCs = new();
        #endregion

        public MainWindow()
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

            Directory.CreateDirectory(DIR_CACHE);
            Directory.CreateDirectory(DIR_CACHE_UI);
            Directory.CreateDirectory(DIR_CACHE_BROWSE);
            Directory.CreateDirectory(DIR_DOWNLOADED);
            Directory.CreateDirectory(DIR_RESOURCES);
            Directory.CreateDirectory(DIR_RESOURCES_DEFAULT_ICONS);

            #region Default Resources
            if (Directory.Exists(DIR_RESOURCES))
            {
                // TODO Download Resources
            }
            else
            {
                // TODO Check Resources Version for Update
            }

            using (StreamReader r = new StreamReader(FILE_SURVIVORS))
            {
                JObject file = JsonConvert.DeserializeObject<JObject>(r.ReadToEnd());
                foreach (KeyValuePair<string, JToken> entry in file)
                {
                    // Common Perks
                    if (entry.Key == "common_perks")
                    {
                        List<Perk> perks = entry.Value.ToObject<List<Perk>>();
                        foreach (Perk perk in perks)
                            _commonSurvivorPerks.Add(perk);
                        continue;
                    }
                    Survivor survivor = entry.Value.ToObject<Survivor>();
                    _survivors.Add(entry.Key, survivor);

                    CharacterUC characterUC = new CharacterUC();
                    characterUC.CharacterInfo = survivor;

                    Grid.SetColumn(characterUC, _survivorUCs.Count % 4);
                    Grid.SetRow(characterUC, _survivorUCs.Count / 4);
                    _survivorUCs.Add(entry.Key, characterUC);
                }
            }

            using (StreamReader r = new StreamReader(FILE_KILLERS))
            {
                JObject file = JsonConvert.DeserializeObject<JObject>(r.ReadToEnd());
                foreach (KeyValuePair<string, JToken> entry in file)
                {
                    // Common Perks
                    if (entry.Key == "common_perks")
                    {
                        List<Perk> perks = entry.Value.ToObject<List<Perk>>();
                        foreach (Perk perk in perks)
                            _commonSurvivorPerks.Add(perk);
                        continue;
                    }
                    Killer killer = entry.Value.ToObject<Killer>();
                    _killers.Add(entry.Key, killer);

                    CharacterUC characterUC = new CharacterUC();
                    characterUC.CharacterInfo = killer;

                    Grid.SetColumn(characterUC, _killerUCs.Count % 4 + 5);
                    Grid.SetRow(characterUC, _killerUCs.Count / 4);
                    _killerUCs.Add(entry.Key, characterUC);
                }
            }

            DownloadAndSetImages();
            #endregion

            #region Packs
            if (!File.Exists(FILE_PACKS))
            {
                // TODO Download JSON
            }
            else
            {
                // TODO Check latest version
            }

            using (StreamReader r = new StreamReader(FILE_PACKS))
            {
                foreach (ResourcePack pack in JsonConvert.DeserializeObject<List<ResourcePack>>(r.ReadToEnd()))
                {
                    pack.PackState = "Download";
                    pack.PackActionable = true;
                    _packRegistry.Add(pack.uniqueKey, pack);
                }
                _browsePacks = _packRegistry.Keys.ToList();
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
            _downloadedPacks = _downloadedRegistry.Keys.ToList();
            #endregion

            InitializeComponent();

            for (int i = 0; i < PACKS_WIDTH * PACKS_HEIGHT; i++)
            {
                PackUC packUC = new PackUC();
                packUC.Visibility = Visibility.Hidden;
                packUC.action.Click += new RoutedEventHandler(UpdatePack);
                packUC.action.Tag = i;
                packUC.action2.Content = "x";
                packUC.action2.Click += new RoutedEventHandler(DeletePack);
                packUC.action2.Tag = i;
                packUC.action2.Visibility = Visibility.Visible;
                Grid.SetColumn(packUC, i % PACKS_WIDTH);
                Grid.SetRow(packUC, i / PACKS_WIDTH);
                downloadedGrid.Children.Add(packUC);
                _downloadedPackUCs.Add(packUC);
            }

            for (int i = 0; i < PACKS_WIDTH * PACKS_HEIGHT; i++)
            {
                PackUC packUC = new PackUC();
                packUC.Visibility = Visibility.Hidden;
                packUC.action.Click += new RoutedEventHandler(DownloadPack);
                packUC.action.Tag = i;
                Grid.SetColumn(packUC, i % PACKS_WIDTH);
                Grid.SetRow(packUC, i / PACKS_WIDTH);
                browseGrid.Children.Add(packUC);
                _browsePackUCs.Add(packUC);
            }

            for (int i = 0; i < (Math.Max(_survivorUCs.Count, _killerUCs.Count) / 4) + 1; i++)
            {
                RowDefinition row = new RowDefinition();
                //row.Height = new GridLength(1, GridUnitType.Auto);
                characterGrid.RowDefinitions.Add(row);
            }
            characterGrid.RowDefinitions.Add(new RowDefinition());

            foreach (CharacterUC characterUC in _survivorUCs.Values)
                characterGrid.Children.Add(characterUC);
            foreach (CharacterUC characterUC in _killerUCs.Values)
                characterGrid.Children.Add(characterUC);
        }

        private void LoadBrowsePage(int page)
        {
            _currentBrowsePage = page;

            foreach (PackUC packUC in _browsePackUCs)
                packUC.Visibility = Visibility.Hidden;

            for (int packNum = 0; packNum < PACKS_WIDTH * PACKS_HEIGHT; packNum++)
            {
                PackUC packUI = _browsePackUCs[packNum];

                int index = page * 9 + packNum;
                if (index >= _browsePacks.Count)
                    return;

                ResourcePack packInfo = _packRegistry[_browsePacks[index]];
                packUI.PackInfo = packInfo;

                if (packInfo.bannerLink != "")
                {
                    string uniqueFile = $"{DIR_CACHE_BROWSE}{packInfo.uniqueKey}_{GetUniqueFilename(packInfo.bannerLink)}";
                    if (File.Exists(uniqueFile))
                    {
                        packUI.banner.Source = LoadImage(Path.Combine(Environment.CurrentDirectory, uniqueFile));
                        packUI.banner.Visibility = 0;
                    }
                    else
                        _ = DownloadBanner(packInfo.bannerLink, uniqueFile, packInfo, packUI);
                }
            }
        }
        async void DownloadPack(object sender, RoutedEventArgs e)
        {
            ResourcePack resourcePack = _browsePackUCs[(int)((Button)sender).Tag].PackInfo;
            resourcePack.PackActionable = false;
            resourcePack.PackState = "Downloading...";
            await DownloadPack(resourcePack.uniqueKey);
            _downloadedPacks.Add(resourcePack.uniqueKey);
        }

        private void LoadDownloadedPage(int page)
        {
            _currentDownloadPage = page;

            foreach (PackUC packUC in _downloadedPackUCs)
                packUC.Visibility = Visibility.Hidden;

            for (int packNum = 0; packNum < PACKS_WIDTH * PACKS_HEIGHT; packNum++)
            {
                PackUC packUI = _downloadedPackUCs[packNum];

                int index = page * 9 + packNum;
                if (index >= _downloadedPacks.Count)
                    return;
                noPacksDownloaded.Visibility = Visibility.Hidden;

                if (!_downloadedRegistry.ContainsKey(_downloadedPacks[index]))
                    continue;

                ResourcePack packInfo = _downloadedRegistry[_downloadedPacks[index]];
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

                string uniqueFile = $"{DIR_DOWNLOADED}{packInfo.uniqueKey}/{GetUniqueFilename(packInfo.bannerLink)}";
                if (File.Exists(uniqueFile))
                {
                    packUI.banner.Source = LoadImage(Path.Combine(Environment.CurrentDirectory, uniqueFile));
                    packUI.banner.Visibility = 0;
                }
            }
        }
        async void UpdatePack(object sender, RoutedEventArgs e)
        {
            ResourcePack resourcePack = _downloadedPackUCs[(int)((Button)sender).Tag].PackInfo;
            string key = resourcePack.uniqueKey;
            resourcePack.PackActionable = false;
            resourcePack.PackState = "Updating...";
            _downloadedRegistry.Remove(key);
            await DownloadPack(key);
            if (!_packRegistry.ContainsKey(key))
                return;
            _packRegistry[key].PackState = "Updated";
        }
        async void DeletePack(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            PackUC packUC = _downloadedPackUCs[id];
            packUC.Visibility = Visibility.Hidden;

            ResourcePack resourcePack = packUC.PackInfo;

            resourcePack.PackActionable = false;
            resourcePack.PackState = "Deleting...";

            _downloadedPacks.Remove(resourcePack.uniqueKey);
            _downloadedRegistry.Remove(resourcePack.uniqueKey);
            Directory.Delete($"{DIR_DOWNLOADED}{resourcePack.uniqueKey}", true);

            if (_packRegistry.ContainsKey(resourcePack.uniqueKey))
            {
                ResourcePack registryPack = _packRegistry[resourcePack.uniqueKey];
                registryPack.PackActionable = true;
                registryPack.PackState = "Download";
            }
        }

        public async Task DownloadImage(string url, string directory)
        {
            string uniqueFile = $"{directory}{GetUniqueFilename(url)}";

            if (File.Exists(uniqueFile))
                return;

            using (WebClient client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(url), uniqueFile);
            }
        }
        public async Task DownloadBanner(string url, string destinationFile, ResourcePack packInfo, PackUC pack)
        {
            using (WebClient client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(url), destinationFile);
                if (packInfo != pack.PackInfo)
                    return;
                pack.banner.Source = LoadImage(Path.Combine(Environment.CurrentDirectory, destinationFile));
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
                string bannerFile = $"{DIR_DOWNLOADED}{pack.uniqueKey}/{GetUniqueFilename(pack.bannerLink)}";
                if (pack.bannerLink != "" && !File.Exists(bannerFile))
                    await client.DownloadFileTaskAsync(new Uri(pack.bannerLink), bannerFile);
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
            ResourcePack copy = JsonConvert.DeserializeObject<ResourcePack>(JsonConvert.SerializeObject(pack));
            _downloadedRegistry.Add(pack.uniqueKey, copy);
            copy.PackActionable = false;
            copy.PackState = "Up To Date";
            pack.PackState = "Downloaded";
        }

        public async void DownloadAndSetImages()
        {
            foreach (Survivor survivor in _survivors.Values)
            {
                // Try and download the potrait (exits early if already cached)
                await DownloadImage(survivor.defaultPortrait, DIR_RESOURCES_DEFAULT_ICONS);
                // Set the Image property to the downloaded (or cached) file
                survivor.PortraitImage = new WriteableBitmap(new BitmapImage(new Uri(
                    Path.Combine(Environment.CurrentDirectory, $"{DIR_RESOURCES_DEFAULT_ICONS}{GetUniqueFilename(survivor.defaultPortrait)}"))));

                // Iterating over a temp list to prevent code duplication
                foreach (Perk perk in new List<Perk>() { survivor.PerkA, survivor.PerkB, survivor.PerkC })
                {
                    // Try and download the perk (exits early if already cached)
                    await DownloadImage(perk.defaultImage, DIR_RESOURCES_DEFAULT_ICONS);
                    // Set the Image property to the downloaded (or cached) file
                    perk.Image = new WriteableBitmap(new BitmapImage(new Uri(
                        Path.Combine(Environment.CurrentDirectory, $"{DIR_RESOURCES_DEFAULT_ICONS}{GetUniqueFilename(perk.defaultImage)}"))));
                }
            }

            foreach (Perk perk in _commonSurvivorPerks)
            {
                // Try and download the perk (exits early if already cached)
                await DownloadImage(perk.defaultImage, DIR_RESOURCES_DEFAULT_ICONS);
                // Set the Image property to the downloaded (or cached) file
                perk.Image = new WriteableBitmap(new BitmapImage(new Uri(
                    Path.Combine(Environment.CurrentDirectory, $"{DIR_RESOURCES_DEFAULT_ICONS}{GetUniqueFilename(perk.defaultImage)}"))));
            }

            foreach (Killer killer in _killers.Values)
            {
                // Try and download the potrait (exits early if already cached)
                await DownloadImage(killer.defaultPortrait, DIR_RESOURCES_DEFAULT_ICONS);
                // Set the Image property to the downloaded (or cached) file
                killer.PortraitImage = new WriteableBitmap(new BitmapImage(new Uri(
                    Path.Combine(Environment.CurrentDirectory, $"{DIR_RESOURCES_DEFAULT_ICONS}{GetUniqueFilename(killer.defaultPortrait)}"))));

                // Iterating over a temp list to prevent code duplication
                foreach (Perk perk in new List<Perk>() { killer.PerkA, killer.PerkB, killer.PerkC })
                {
                    // Try and download the perk (exits early if already cached)
                    await DownloadImage(perk.defaultImage, DIR_RESOURCES_DEFAULT_ICONS);
                    // Set the Image property to the downloaded (or cached) file
                    perk.Image = new WriteableBitmap(new BitmapImage(new Uri(
                        Path.Combine(Environment.CurrentDirectory, $"{DIR_RESOURCES_DEFAULT_ICONS}{GetUniqueFilename(perk.defaultImage)}"))));
                }

                // TODO Addons
            }

            foreach (Perk perk in _commonKillerPerks)
            {
                // Try and download the perk (exits early if already cached)
                await DownloadImage(perk.defaultImage, DIR_RESOURCES_DEFAULT_ICONS);
                // Set the Image property to the downloaded (or cached) file
                perk.Image = new WriteableBitmap(new BitmapImage(new Uri(
                    Path.Combine(Environment.CurrentDirectory, $"{DIR_RESOURCES_DEFAULT_ICONS}{GetUniqueFilename(perk.defaultImage)}"))));
            }

            // TODO Set Overrides
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

        public static string GetUniqueFilename(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            return $"{Regex.Replace(filePath.Substring(0, filePath.Length - extension.Length), "[^A-Za-z0-9-_]", "")}{extension}";
        }
        public static BitmapImage LoadImage(string filePath)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(filePath);
            image.EndInit();
            return image;
        }
    }
}
