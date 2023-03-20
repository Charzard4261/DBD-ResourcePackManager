﻿using DBD_ResourcePacks.Classes;
using DBD_ResourcePacks.Properties;
using DBD_ResourcePacks.UserControls;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        #region Packs
        public PackRegister Register { get; set; }

        List<PackUC> _downloadedPackUCs = new();
        int _currentDownloadPage = 0;

        List<PackUC> _browsePackUCs = new();
        int _currentBrowsePage = 0;
        #endregion

        #region Customise
        // Data
        private Customiser _customiser;
        public Customiser Customiser { get => _customiser; private set { _customiser = value; } }

        // UI
        Dictionary<string, CharacterUC> _survivorUCs = new();
        Dictionary<string, CharacterUC> _killerUCs = new();
        #endregion

        public MainWindow()
        {
            Constants.UpdateTheme();

            Directory.CreateDirectory(Constants.DIR_CACHE);
            Directory.CreateDirectory(Constants.DIR_CACHE_BROWSE);
            Directory.CreateDirectory(Constants.DIR_DOWNLOADED);
            Directory.CreateDirectory(Constants.DIR_RESOURCES);
            Directory.CreateDirectory(Constants.DIR_RESOURCES_DEFAULT_ICONS);

            #region Default Resources
            if (Directory.Exists(Constants.DIR_RESOURCES))
            {
                // TODO Download Resources
            }
            else
            {
                // TODO Check Resources Version for Update
            }

            List<Perk> commonSurvivorPerks = new();
            Dictionary<string, Survivor> survivors = new();
            using (StreamReader r = new StreamReader(Constants.FILE_SURVIVORS))
            {
                JObject file = JsonConvert.DeserializeObject<JObject>(r.ReadToEnd());
                foreach (KeyValuePair<string, JToken> entry in file)
                {
                    // Common Perks
                    if (entry.Key == "common_perks")
                    {
                        List<Perk> perks = entry.Value.ToObject<List<Perk>>();
                        foreach (Perk perk in perks)
                        {
                            perk.forSurvivor = true;
                            commonSurvivorPerks.Add(perk);
                        }
                        continue;
                    }
                    Survivor survivor = entry.Value.ToObject<Survivor>();
                    survivor.key = entry.Key;
                    survivor.PerkA.forSurvivor = true;
                    survivor.PerkA.fromCharacter = survivor;
                    survivor.PerkB.forSurvivor = true;
                    survivor.PerkB.fromCharacter = survivor;
                    survivor.PerkC.forSurvivor = true;
                    survivor.PerkC.fromCharacter = survivor;
                    survivors.Add(entry.Key, survivor);

                    CharacterUC characterUC = new CharacterUC();
                    characterUC.CharacterInfo = survivor;

                    Grid.SetColumn(characterUC, _survivorUCs.Count % 4);
                    Grid.SetRow(characterUC, _survivorUCs.Count / 4);
                    _survivorUCs.Add(entry.Key, characterUC);
                }
            }

            List<Perk> commonKillerPerks = new();
            Dictionary<string, Killer> killers = new();
            using (StreamReader r = new StreamReader(Constants.FILE_KILLERS))
            {
                JObject file = JsonConvert.DeserializeObject<JObject>(r.ReadToEnd());
                foreach (KeyValuePair<string, JToken> entry in file)
                {
                    // Common Perks
                    if (entry.Key == "common_perks")
                    {
                        List<Perk> perks = entry.Value.ToObject<List<Perk>>();
                        foreach (Perk perk in perks)
                        {
                            perk.forSurvivor = false;
                            commonKillerPerks.Add(perk);
                        }
                        continue;
                    }
                    Killer killer = entry.Value.ToObject<Killer>();
                    killer.key = entry.Key;
                    killer.PerkA.forSurvivor = false;
                    killer.PerkA.fromCharacter = killer;
                    killer.PerkB.forSurvivor = false;
                    killer.PerkB.fromCharacter = killer;
                    killer.PerkC.forSurvivor = false;
                    killer.PerkC.fromCharacter = killer;
                    killers.Add(entry.Key, killer);

                    CharacterUC characterUC = new CharacterUC();
                    characterUC.CharacterInfo = killer;

                    Grid.SetColumn(characterUC, _killerUCs.Count % 4 + 5);
                    Grid.SetRow(characterUC, _killerUCs.Count / 4);
                    _killerUCs.Add(entry.Key, characterUC);
                }
            }

            _customiser = new Customiser(this, survivors, commonSurvivorPerks, killers, commonKillerPerks);
            if (File.Exists(Constants.FILE_CUSTOMISER))
                using (StreamReader r = new StreamReader(Constants.FILE_CUSTOMISER))
                {
                    _customiser.save = JsonConvert.DeserializeObject<CustomiserSave>(r.ReadToEnd());
                }
            else
            {
                _customiser.save = new CustomiserSave();
                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
            _customiser.DownloadImages();
            #endregion

            #region Packs
            if (!File.Exists(Constants.FILE_PACKS))
            {
                // TODO Download JSON
            }
            else
            {
                // TODO Check latest version
            }

            Register = new(this);
            using (StreamReader r = new StreamReader(Constants.FILE_PACKS))
            {
                foreach (ResourcePack pack in JsonConvert.DeserializeObject<List<ResourcePack>>(r.ReadToEnd()))
                {
                    pack.PackState = "Download";
                    pack.PackActionable = true;
                    Register.packRegistry.Add(pack.uniqueKey, pack);
                }
                Register.browsePagePacks = Register.packRegistry.Keys.ToList();
            }

            foreach (DirectoryInfo potentialPack in new DirectoryInfo(Constants.DIR_DOWNLOADED).EnumerateDirectories())
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

                // If the download was interrupted, delete
                if (File.Exists($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/temp.txt"))
                {
                    Directory.Delete(potentialPack.FullName, true);
                    continue;
                }

                Register.downloadedRegistry.Add(pack.uniqueKey, pack);

                if (Register.packRegistry.ContainsKey(pack.uniqueKey))
                {
                    ResourcePack registryPack = Register.packRegistry[pack.uniqueKey];
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
            Register.downloadedPagePacks = Register.downloadedRegistry.Keys.ToList();
            #endregion

            InitializeComponent();

            // Fill out the downloaded & browse grids with columns
            for (int i = 0; i < Constants.PACKS_HEIGHT; i++)
            {
                ColumnDefinition dColumnStar = new ColumnDefinition();
                dColumnStar.Width = new GridLength(1, GridUnitType.Star);
                downloadedGrid.ColumnDefinitions.Add(dColumnStar);

                ColumnDefinition bColumnStar = new ColumnDefinition();
                bColumnStar.Width = new GridLength(1, GridUnitType.Star);
                browseGrid.ColumnDefinitions.Add(bColumnStar);
            }

            // Fill out the downloaded & browse grids with rows
            for (int i = 0; i < Constants.PACKS_WIDTH; i++)
            {
                RowDefinition dRowStar = new RowDefinition();
                dRowStar.Height = new GridLength(1, GridUnitType.Star);
                downloadedGrid.RowDefinitions.Add(dRowStar);

                RowDefinition bRowStar = new RowDefinition();
                bRowStar.Height = new GridLength(1, GridUnitType.Star);
                browseGrid.RowDefinitions.Add(bRowStar);
            }
            // Add the auto rows for the download page naviagation
            {
                RowDefinition dRowAuto = new RowDefinition();
                dRowAuto.Height = new GridLength(1, GridUnitType.Auto);
                downloadedGrid.RowDefinitions.Add(dRowAuto);

                RowDefinition bRowAuto = new RowDefinition();
                bRowAuto.Height = new GridLength(1, GridUnitType.Auto);
                browseGrid.RowDefinitions.Add(bRowAuto);
            }

            // Create a Pack User Control for each slot in the download grid
            for (int i = 0; i < Constants.PACKS_WIDTH * Constants.PACKS_HEIGHT; i++)
            {
                PackUC packUC = new PackUC();
                packUC.Visibility = Visibility.Hidden;
                packUC.action.Click += new RoutedEventHandler(UpdatePack);
                packUC.action.Tag = i;
                packUC.action2.Content = "x";
                packUC.action2.Click += new RoutedEventHandler(DeletePack);
                packUC.action2.Tag = i;
                packUC.action2.Visibility = Visibility.Visible;
                Grid.SetColumn(packUC, i % Constants.PACKS_WIDTH);
                Grid.SetRow(packUC, (i / Constants.PACKS_WIDTH) + 2);
                downloadedGrid.Children.Add(packUC);
                _downloadedPackUCs.Add(packUC);
            }

            // Create a Pack User Control for each slot in the browse grid
            for (int i = 0; i < Constants.PACKS_WIDTH * Constants.PACKS_HEIGHT; i++)
            {
                PackUC packUC = new PackUC();
                packUC.Visibility = Visibility.Hidden;
                packUC.action.Click += new RoutedEventHandler(DownloadPack);
                packUC.action.Tag = i;
                Grid.SetColumn(packUC, i % Constants.PACKS_WIDTH);
                Grid.SetRow(packUC, (i / Constants.PACKS_WIDTH) + 2);
                browseGrid.Children.Add(packUC);
                _browsePackUCs.Add(packUC);
            }

            // Fill out the customise grid with enough roughs for all characters to fit
            for (int i = 0; i < (Math.Max(_survivorUCs.Count, _killerUCs.Count) / 4) + 1; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Auto);
                characterGrid.RowDefinitions.Add(row);
            }
            // Add another row to the bottom for formatting purposes
            RowDefinition rowStar = new RowDefinition();
            rowStar.Height = new GridLength(1, GridUnitType.Star);
            characterGrid.RowDefinitions.Add(rowStar);

            // Add all the Survivors to the customise grid
            foreach (CharacterUC characterUC in _survivorUCs.Values)
                characterGrid.Children.Add(characterUC);
            // Add all the Killers to the customise grid
            foreach (CharacterUC characterUC in _killerUCs.Values)
                characterGrid.Children.Add(characterUC);
        }

        public void LoadBrowsePage(int page)
        {
            int pageCount = Register.browsePagePacks.Count / (Constants.PACKS_WIDTH * Constants.PACKS_HEIGHT);
            if (page < 0 || page > pageCount)
                return;
            _currentBrowsePage = page;

            browsePageLeft.IsEnabled = page > 0;
            browsePageSelect.Text = $"{page + 1}";
            browsePageTotal.Content = $"/{pageCount + 1}";
            browsePageRight.IsEnabled = page < pageCount;

            foreach (PackUC packUC in _browsePackUCs)
                packUC.Visibility = Visibility.Hidden;

            for (int packNum = 0; packNum < Constants.PACKS_WIDTH * Constants.PACKS_HEIGHT; packNum++)
            {
                PackUC packUI = _browsePackUCs[packNum];

                int index = page * Constants.PACKS_WIDTH * Constants.PACKS_HEIGHT + packNum;
                if (index >= Register.browsePagePacks.Count)
                    return;

                ResourcePack packInfo = Register.packRegistry[Register.browsePagePacks[index]];
                packUI.PackInfo = packInfo;

                if (packInfo.bannerLink != "")
                {
                    string uniqueFile = $"{Constants.DIR_CACHE_BROWSE}/{packInfo.uniqueKey}_{Constants.GetUniqueFilename(packInfo.bannerLink)}";
                    if (File.Exists(uniqueFile))
                    {
                        packUI.banner.Source = Constants.LoadImage(Path.Combine(Environment.CurrentDirectory, uniqueFile));
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
            Register.downloadedPagePacks.Add(resourcePack.uniqueKey);
        }

        public void LoadDownloadPage(int page)
        {
            int pageCount = Register.downloadedPagePacks.Count / (Constants.PACKS_WIDTH * Constants.PACKS_HEIGHT);
            if (page < 0 || page > pageCount)
                return;
            _currentDownloadPage = page;

            downloadPageLeft.IsEnabled = page > 0;
            downloadPageSelect.Text = $"{page + 1}";
            downloadPageTotal.Content = $"/{pageCount + 1}";
            downloadPageRight.IsEnabled = page < pageCount;

            foreach (PackUC packUC in _downloadedPackUCs)
                packUC.Visibility = Visibility.Hidden;

            for (int packNum = 0; packNum < Constants.PACKS_WIDTH * Constants.PACKS_HEIGHT; packNum++)
            {
                PackUC packUI = _downloadedPackUCs[packNum];

                int index = page * 9 + packNum;
                if (index >= Register.downloadedPagePacks.Count)
                    return;
                noPacksDownloaded.Visibility = Visibility.Hidden;

                if (!Register.downloadedRegistry.ContainsKey(Register.downloadedPagePacks[index]))
                    continue;

                ResourcePack packInfo = Register.downloadedRegistry[Register.downloadedPagePacks[index]];
                packUI.PackInfo = packInfo;

                /*if (register.packRegistry.ContainsKey(packInfo.uniqueKey))
                {
                    ResourcePack registryPack = register.packRegistry[packInfo.uniqueKey];
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

                string uniqueFile = $"{Constants.DIR_DOWNLOADED}/{packInfo.uniqueKey}/{Constants.GetUniqueFilename(packInfo.bannerLink)}";
                if (File.Exists(uniqueFile))
                {
                    packUI.banner.Source = Constants.LoadImage(Path.Combine(Environment.CurrentDirectory, uniqueFile));
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
            Register.downloadedRegistry.Remove(key);
            await DownloadPack(key);
            if (!Register.packRegistry.ContainsKey(key))
                return;
            Register.packRegistry[key].PackState = "Updated";
        }
        async void DeletePack(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            PackUC packUC = _downloadedPackUCs[id];
            packUC.Visibility = Visibility.Hidden;

            ResourcePack resourcePack = packUC.PackInfo;

            resourcePack.PackActionable = false;
            resourcePack.PackState = "Deleting...";

            Register.downloadedPagePacks.Remove(resourcePack.uniqueKey);
            Register.downloadedRegistry.Remove(resourcePack.uniqueKey);
            Directory.Delete($"{Constants.DIR_DOWNLOADED}/{resourcePack.uniqueKey}", true);

            if (Register.packRegistry.ContainsKey(resourcePack.uniqueKey))
            {
                ResourcePack registryPack = Register.packRegistry[resourcePack.uniqueKey];
                registryPack.PackActionable = true;
                registryPack.PackState = "Download";
            }
        }

        public async Task DownloadBanner(string url, string destinationFile, ResourcePack packInfo, PackUC pack)
        {
            using (WebClient client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(url), destinationFile);
                if (packInfo != pack.PackInfo)
                    return;
                pack.banner.Source = Constants.LoadImage(Path.Combine(Environment.CurrentDirectory, destinationFile));
                pack.banner.Visibility = 0;
            }
        }
        public async Task DownloadPack(string uniqueKey)
        {
            if (!Register.packRegistry.ContainsKey(uniqueKey))
                return;
            ResourcePack pack = Register.packRegistry[uniqueKey];

            if (pack.downloadLink == "")
                return;

            Directory.CreateDirectory($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}");
            File.WriteAllText($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/temp.txt", "Placeholder file whilst the pack is still downloading");
            File.WriteAllText($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/pack.json", JsonConvert.SerializeObject(pack, Formatting.Indented));

            using (WebClient client = new WebClient())
            {
                pack.PackState = "Downloading Banner";
                string bannerFile = $"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/{Constants.GetUniqueFilename(pack.bannerLink)}";
                if (pack.bannerLink != "" && !File.Exists(bannerFile))
                    await client.DownloadFileTaskAsync(new Uri(pack.bannerLink), bannerFile);
                pack.PackState = "Downloading Zip";
                await client.DownloadFileTaskAsync(new Uri(pack.downloadLink), $"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/pack.zip");

                if (Directory.Exists($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack"))
                {
                    pack.PackState = "Deleting Old Pack";
                    Directory.Delete($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack", true);
                }

                pack.PackState = "Unzipping";
                ZipFile.ExtractToDirectory($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/pack.zip", $"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack");
                pack.PackState = "Deleting Zip";
                File.Delete($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/pack.zip");
            }
            pack.PackState = "Finishing";
            File.Delete($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/temp.txt");
            ResourcePack copy = JsonConvert.DeserializeObject<ResourcePack>(JsonConvert.SerializeObject(pack));
            Register.downloadedRegistry.Add(pack.uniqueKey, copy);
            copy.PackActionable = false;
            copy.PackState = "Up To Date";
            pack.PackState = "Downloaded";
        }

        public ResourcePack GetDownloadedPackInfo(string uniqueKey)
        {
            if (!Register.downloadedRegistry.ContainsKey(uniqueKey))
                return null;
            return Register.downloadedRegistry[uniqueKey];
        }

        #region Element Events
        void OpenSettings(object sender, RoutedEventArgs e) { new SettingsPopup(this).ShowDialog(); }
        void OpenAbout(object sender, RoutedEventArgs e) { Process.Start(new ProcessStartInfo() { FileName = "https://github.com/Charzard4261/DBD-ResourcePacks", UseShellExecute = true }); }
        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is not TabControl tabControl)
                return;

            switch (tabControl.SelectedIndex)
            {
                case 0: // Installed
                    LoadDownloadPage(_currentDownloadPage);
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

        void DownloadSearch_KeyUp(object sender, RoutedEventArgs e) { Register.DownloadSearch = ((TextBox)sender).Text; }
        void DownloadPageLeft_Click(object sender, RoutedEventArgs e) { LoadDownloadPage(_currentDownloadPage - 1); }
        void DownloadPageRight_Click(object sender, RoutedEventArgs e) { LoadDownloadPage(_currentDownloadPage + 1); }
        void DownloadPageSelect_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            e.Handled = true;
            if (int.TryParse(((TextBox)sender).Text, out int page))
                LoadDownloadPage(page - 1);
        }

        void BrowseSearch_KeyUp(object sender, RoutedEventArgs e) { Register.BrowseSearch = ((TextBox)sender).Text; }
        void BrowsePageLeft_Click(object sender, RoutedEventArgs e) { LoadBrowsePage(_currentBrowsePage - 1); }
        void BrowsePageRight_Click(object sender, RoutedEventArgs e) { LoadBrowsePage(_currentBrowsePage + 1); }
        void BrowsePageSelect_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            e.Handled = true;
            if (int.TryParse(((TextBox)sender).Text, out int page))
                LoadBrowsePage(page - 1);
        }

        void SetAllEverything(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.Everything = "";
                else
                    _customiser.Everything = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllPortraits(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllPortraits = "";
                else
                    _customiser.AllPortraits = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllPerks(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllPerks = "";
                else
                    _customiser.AllPerks = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllItems(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllItems = "";
                else
                    _customiser.AllItems = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllPowers(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllKillerPowers = "";
                else
                    _customiser.AllKillerPowers = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllAddons(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllAddons = "";
                else
                    _customiser.AllAddons = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllOfferings(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllOfferings = "";
                else
                    _customiser.AllOfferings = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllEmblems(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllEmblems = "";
                else
                    _customiser.AllEmblems = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllStatusEffects(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllStatusEffects = "";
                else
                    _customiser.AllStatusEffects = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllMiscUI(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllMiscUI = "";
                else
                    _customiser.AllMiscUI = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }

        void SetAllSurvivorEverything(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllSurvivors = "";
                else
                    _customiser.AllSurvivors = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllSurvivorPortraits(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllSurvivorPortraits = "";
                else
                    _customiser.AllSurvivorPortraits = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllSurvivorPerks(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllSurvivorPerks = "";
                else
                    _customiser.AllSurvivorPerks = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllItemAddons(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllItemAddons = "";
                else
                    _customiser.AllItemAddons = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllSurvivorOfferings(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllSurvivorOfferings = "";
                else
                    _customiser.AllSurvivorOfferings = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }

        void SetAllKillerEverything(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllKillers = "";
                else
                    _customiser.AllKillers = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllKillerPortraits(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllKillerPortraits = "";
                else
                    _customiser.AllKillerPortraits = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllKillerPerks(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllKillerPerks = "";
                else
                    _customiser.AllKillerPerks = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllKillerAddons(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllKillerAddons = "";
                else
                    _customiser.AllKillerAddons = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }
        void SetAllKillerOfferings(object sender, RoutedEventArgs e)
        {
            PackSelect packSelect = new PackSelect(Register.GetDownloadedPacks());
            // == true because ShowDialog is a "bool?"
            if (packSelect.ShowDialog() == true)
            {
                string result = packSelect.unique_key;
                if (result == "none")
                    _customiser.AllKillerOfferings = "";
                else
                    _customiser.AllKillerOfferings = result;
                _customiser.SetImages();

                File.WriteAllText(Constants.FILE_CUSTOMISER, JsonConvert.SerializeObject(_customiser.save, Formatting.Indented));
            }
        }

        void Install(object sender, RoutedEventArgs e)
        {
            if (!Constants.IsValidGamePath(Settings.Default.GameInstallationPath))
                return;

            string gamePath = $"{Settings.Default.GameInstallationPath}/DeadByDaylight/Content/UI/Icons";
            if (Directory.Exists(gamePath))
                Directory.Delete(gamePath, true);

            Directory.CreateDirectory(gamePath);
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PORTRAITS}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_ITEMS}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_ADDONS}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_POWERS}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_ACTIONS}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_OFFERINGS}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_EMBLEMS}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_STATUS_EFFECTS}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_DAILY_RITUALS}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_HELP}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_HELPLOADING}");
            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PACKS}");

            if (_customiser.save.everything != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.everything))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.everything];
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack", $"{gamePath}");
                }
            }

            if (_customiser.save.allPortraits != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.allPortraits))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allPortraits];
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PORTRAITS}", $"{gamePath}/{Constants.FOLDER_PORTRAITS}");
                }
            }

            if (_customiser.save.allPerks != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.allPerks))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allPerks];
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}", $"{gamePath}/{Constants.FOLDER_PERKS}");
                }
            }

            if (_customiser.save.allItems != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.allItems))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allItems];
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_ITEMS}", $"{gamePath}/{Constants.FOLDER_ITEMS}");
                }
            }

            if (_customiser.save.allAddons != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.allAddons))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allAddons];
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_ADDONS}", $"{gamePath}/{Constants.FOLDER_ADDONS}");
                }
            }

            if (_customiser.save.allKillerPowers != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.allKillerPowers))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allKillerPowers];
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_POWERS}", $"{gamePath}/{Constants.FOLDER_POWERS}");
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_ACTIONS}", $"{gamePath}/{Constants.FOLDER_ACTIONS}");
                }
            }

            if (_customiser.save.allOfferings != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.allOfferings))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allOfferings];
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_OFFERINGS}", $"{gamePath}/{Constants.FOLDER_OFFERINGS}");
                }
            }

            if (_customiser.save.allEmblems != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.allEmblems))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allEmblems];
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_EMBLEMS}", $"{gamePath}/{Constants.FOLDER_EMBLEMS}");
                }
            }

            if (_customiser.save.allOfferings != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.allOfferings))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allOfferings];
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_STATUS_EFFECTS}", $"{gamePath}/{Constants.FOLDER_STATUS_EFFECTS}");
                }
            }

            if (_customiser.save.allMiscUI != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.allMiscUI))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allMiscUI];
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_DAILY_RITUALS}", $"{gamePath}/{Constants.FOLDER_DAILY_RITUALS}");
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_HELP}", $"{gamePath}/{Constants.FOLDER_HELP}");
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_HELPLOADING}", $"{gamePath}/{Constants.FOLDER_HELPLOADING}");
                    Constants.CopyFilesRecursively($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PACKS}", $"{gamePath}/{Constants.FOLDER_PACKS}");
                }
            }

            #region Survivor Overrides
            if (_customiser.save.allSurvivors != "")
            {
                string packKey = _customiser.save.allSurvivors;
                if (Register.downloadedRegistry.ContainsKey(packKey))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allSurvivors];
                    // Replace each Survivor's Portrait and Perk images
                    foreach (Survivor survivor in _customiser._survivors.Values)
                    {
                        // Overwrite Portrait
                        if (survivor.portrait.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = survivor.portrait.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PORTRAITS}{survivor.portrait.Substring(0, survivor.portrait.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PORTRAITS}/{survivor.portrait}", $"{gamePath}/{Constants.FOLDER_PORTRAITS}/{survivor.portrait}");

                        // Overwite Perks
                        if (survivor.PerkA.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = survivor.PerkA.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{survivor.PerkA.filePath.Substring(0, survivor.PerkA.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{survivor.PerkA.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{survivor.PerkA.filePath}");

                        if (survivor.PerkB.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = survivor.PerkB.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{survivor.PerkB.filePath.Substring(0, survivor.PerkB.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{survivor.PerkB.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{survivor.PerkB.filePath}");

                        if (survivor.PerkC.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = survivor.PerkC.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{survivor.PerkC.filePath.Substring(0, survivor.PerkC.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{survivor.PerkC.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{survivor.PerkC.filePath}");
                    }

                    // Replace the common Survivor Perk images
                    foreach (Perk perk in _customiser._commonSurvivorPerks)
                    {
                        if (perk.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = perk.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{perk.filePath.Substring(0, perk.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{perk.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{perk.filePath}");
                    }
                }
            }

            if (_customiser.save.allSurvivorPortraits != "")
            {
                string packKey = _customiser.save.allSurvivorPortraits;
                if (Register.downloadedRegistry.ContainsKey(packKey))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allSurvivorPortraits];
                    // Replace each Survivor's Portrait image
                    foreach (Survivor survivor in _customiser._survivors.Values)
                    {
                        // Overwrite Portrait
                        if (survivor.portrait.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = survivor.portrait.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PORTRAITS}{survivor.portrait.Substring(0, survivor.portrait.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PORTRAITS}/{survivor.portrait}", $"{gamePath}/{Constants.FOLDER_PORTRAITS}/{survivor.portrait}");
                    }
                }
            }

            if (_customiser.save.allSurvivorPerks != "")
            {
                string packKey = _customiser.save.allSurvivorPerks;
                if (Register.downloadedRegistry.ContainsKey(packKey))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allSurvivorPerks];
                    // Replace each Survivor's Perk images
                    foreach (Survivor survivor in _customiser._survivors.Values)
                    {
                        // Overwite Perks
                        if (survivor.PerkA.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = survivor.PerkA.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{survivor.PerkA.filePath.Substring(0, survivor.PerkA.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{survivor.PerkA.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{survivor.PerkA.filePath}");

                        if (survivor.PerkB.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = survivor.PerkB.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{survivor.PerkB.filePath.Substring(0, survivor.PerkB.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{survivor.PerkB.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{survivor.PerkB.filePath}");

                        if (survivor.PerkC.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = survivor.PerkC.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{survivor.PerkC.filePath.Substring(0, survivor.PerkC.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{survivor.PerkC.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{survivor.PerkC.filePath}");
                    }

                    // Replace the common Survivor Perk images
                    foreach (Perk perk in _customiser._commonSurvivorPerks)
                    {
                        if (perk.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = perk.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{perk.filePath.Substring(0, perk.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{perk.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{perk.filePath}");
                    }
                }
            }

            if (_customiser.save.allItemAddons != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.allItemAddons))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allItemAddons];
                    // TODO
                }
            }

            if (_customiser.save.allSurvivorOfferings != "")
            {
                string packKey = _customiser.save.allSurvivorOfferings;
                if (Register.downloadedRegistry.ContainsKey(packKey))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allSurvivorOfferings];
                    // TODO
                }
            }
            #endregion

            #region Killer Overrides
            if (_customiser.save.allKillers != "")
            {
                string packKey = _customiser.save.allKillers;
                if (Register.downloadedRegistry.ContainsKey(packKey))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allKillers];
                    // Replace each Killer's Portrait and Perk images
                    foreach (Killer killer in _customiser._killers.Values)
                    {
                        // Overwrite Portrait
                        if (killer.portrait.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = killer.portrait.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PORTRAITS}{killer.portrait.Substring(0, killer.portrait.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PORTRAITS}/{killer.portrait}", $"{gamePath}/{Constants.FOLDER_PORTRAITS}/{killer.portrait}");

                        // Overwite Perks
                        if (killer.PerkA.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = killer.PerkA.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{killer.PerkA.filePath.Substring(0, killer.PerkA.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{killer.PerkA.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{killer.PerkA.filePath}");

                        if (killer.PerkB.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = killer.PerkB.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{killer.PerkB.filePath.Substring(0, killer.PerkB.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{killer.PerkB.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{killer.PerkB.filePath}");

                        if (killer.PerkC.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = killer.PerkC.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{killer.PerkC.filePath.Substring(0, killer.PerkC.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{killer.PerkC.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{killer.PerkC.filePath}");
                    }

                    // Replace the common Killer Perk images
                    foreach (Perk perk in _customiser._commonKillerPerks)
                    {
                        if (perk.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = perk.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{perk.filePath.Substring(0, perk.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{perk.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{perk.filePath}");
                    }
                }
            }

            if (_customiser.save.allKillerPortraits != "")
            {
                string packKey = _customiser.save.allKillerPortraits;
                if (Register.downloadedRegistry.ContainsKey(packKey))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allKillerPortraits];
                    // Replace each Killer's Portrait image
                    foreach (Killer killer in _customiser._killers.Values)
                    {
                        // Overwrite Portrait
                        if (killer.portrait.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = killer.portrait.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PORTRAITS}{killer.portrait.Substring(0, killer.portrait.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PORTRAITS}/{killer.portrait}", $"{gamePath}/{Constants.FOLDER_PORTRAITS}/{killer.portrait}");
                    }
                }
            }

            if (_customiser.save.allKillerPerks != "")
            {
                string packKey = _customiser.save.allKillerPerks;
                if (Register.downloadedRegistry.ContainsKey(packKey))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allKillerPerks];
                    // Replace each Killer's Perk images
                    foreach (Killer killer in _customiser._killers.Values)
                    {
                        // Overwite Perks
                        if (killer.PerkA.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = killer.PerkA.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{killer.PerkA.filePath.Substring(0, killer.PerkA.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{killer.PerkA.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{killer.PerkA.filePath}");

                        if (killer.PerkB.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = killer.PerkB.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{killer.PerkB.filePath.Substring(0, killer.PerkB.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{killer.PerkB.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{killer.PerkB.filePath}");

                        if (killer.PerkC.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = killer.PerkC.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{killer.PerkC.filePath.Substring(0, killer.PerkC.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{killer.PerkC.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{killer.PerkC.filePath}");
                    }

                    // Replace the common Killer Perk images
                    foreach (Perk perk in _customiser._commonKillerPerks)
                    {
                        if (perk.filePath.Contains("/"))
                        {
                            // This file is in a sub folder, so create it
                            string[] folders = perk.filePath.Split("/");
                            Directory.CreateDirectory($"{gamePath}/{Constants.FOLDER_PERKS}{perk.filePath.Substring(0, perk.filePath.Length - folders[folders.Length - 1].Length)}");
                        }
                        Constants.CopyFile($"{Constants.DIR_DOWNLOADED}/{pack.uniqueKey}/Pack/{Constants.FOLDER_PERKS}/{perk.filePath}", $"{gamePath}/{Constants.FOLDER_PERKS}/{perk.filePath}");
                    }
                }
            }

            if (_customiser.save.allKillerAddons != "")
            {
                if (Register.downloadedRegistry.ContainsKey(_customiser.save.allKillerAddons))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allKillerAddons];
                    // TODO
                }
            }

            if (_customiser.save.allKillerOfferings != "")
            {
                string packKey = _customiser.save.allKillerOfferings;
                if (Register.downloadedRegistry.ContainsKey(packKey))
                {
                    ResourcePack pack = Register.downloadedRegistry[_customiser.save.allKillerOfferings];
                    // TODO
                }
            }
            #endregion

            install.Content = "Installed!";
        }
        #endregion

    }
}
