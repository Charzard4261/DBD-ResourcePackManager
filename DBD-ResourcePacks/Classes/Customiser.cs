using DBD_ResourcePacks.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace DBD_ResourcePacks.Classes
{
    public class Customiser
    {
        public static string FOLDER_ACTION = "Actions/";
        public static string FOLDER_PORTRAIT = "CharPortraits/";
        public static string FOLDER_DAILY_RITUAL = "DailyRituals/";
        public static string FOLDER_EMBLEM = "Emblems/";
        public static string FOLDER_OFFERING = "Favors/";
        public static string FOLDER_PERK = "Perks/";
        public static string FOLDER_HELP = "Help/";
        public static string FOLDER_HELPLOADING = "HelpLoading/";
        public static string FOLDER_ADDON = "ItemAddons/";
        public static string FOLDER_ITEM = "Items/";
        public static string FOLDER_PACK = "Packs/";
        public static string FOLDER_POWER = "Powers/";
        public static string FOLDER_STATUS_EFFECT = "StatusEffects/";

        MainWindow _mainWindow;
        Dictionary<string, Survivor> _survivors = new();
        List<Perk> _commonSurvivorPerks = new();
        Dictionary<string, Killer> _killers = new();
        List<Perk> _commonKillerPerks = new();

        public Customiser(MainWindow mainWindow,
            Dictionary<string, Survivor> survivors,
            List<Perk> commonSurvivorPerks,
            Dictionary<string, Killer> killers,
            List<Perk> commonKillerPerks
            )
        {
            _mainWindow = mainWindow;
            _survivors = survivors;
            _commonSurvivorPerks = commonSurvivorPerks;
            _killers = killers;
            _commonKillerPerks = commonKillerPerks;
        }

        public string everything = "";
        public string allPortraits = "";
        public string allPerks = "";
        public string allItems = "";
        public string allItemAddons = "";
        public string allKillerPowers = "";
        public string allKillerAddons = "";
        public string allOfferings = "";
        public string allMiscUI = "";

        public string GetImageForPerk(Perk perk)
        {
            string file = perk.filePath;

            if (everything != "")
                file = _mainWindow.GetDownloadedPackInfo(everything);

            if (allPerks != "")
                file = _mainWindow.GetDownloadedPackInfo(allPerks);

            if (perk.forSurvivor)
            {
                if (allSurvivor != "")
                    file = _mainWindow.GetDownloadedPackInfo(allSurvivor);
            } else
            {
                if (allKiller != "")
                    file = _mainWindow.GetDownloadedPackInfo(allKiller);
            }

            if (overrides.ContainsKey(perk.key))
                file = _mainWindow.GetDownloadedPackInfo(overrides[perk.key]);

            return $"{FOLDER_PERK}{file}";
        }

        public async void DownloadAndSetImages()
        {
            foreach (Survivor survivor in _survivors.Values)
            {
                // Try and download the potrait (exits early if already cached)
                await MainWindow.DownloadImage(survivor.defaultPortrait, MainWindow.DIR_RESOURCES_DEFAULT_ICONS);
                // Set the Image property to the downloaded (or cached) file
                survivor.PortraitImage = new WriteableBitmap(new BitmapImage(new Uri(
                    Path.Combine(Environment.CurrentDirectory, $"{MainWindow.DIR_RESOURCES_DEFAULT_ICONS}{MainWindow.GetUniqueFilename(survivor.defaultPortrait)}"))));

                // Iterating over a temp list to prevent code duplication
                foreach (Perk perk in new List<Perk>() { survivor.PerkA, survivor.PerkB, survivor.PerkC })
                {
                    // Try and download the perk (exits early if already cached)
                    await MainWindow.DownloadImage(perk.defaultImage, MainWindow.DIR_RESOURCES_DEFAULT_ICONS);
                    // Set the Image property to the downloaded (or cached) file
                    perk.Image = new WriteableBitmap(new BitmapImage(new Uri(
                        Path.Combine(Environment.CurrentDirectory, $"{MainWindow.DIR_RESOURCES_DEFAULT_ICONS}{MainWindow.GetUniqueFilename(perk.defaultImage)}"))));
                }
            }

            foreach (Perk perk in _commonSurvivorPerks)
            {
                // Try and download the perk (exits early if already cached)
                await MainWindow.DownloadImage(perk.defaultImage, MainWindow.DIR_RESOURCES_DEFAULT_ICONS);
                // Set the Image property to the downloaded (or cached) file
                perk.Image = new WriteableBitmap(new BitmapImage(new Uri(
                    Path.Combine(Environment.CurrentDirectory, $"{MainWindow.DIR_RESOURCES_DEFAULT_ICONS}{MainWindow.GetUniqueFilename(perk.defaultImage)}"))));
            }

            foreach (Killer killer in _killers.Values)
            {
                // Try and download the potrait (exits early if already cached)
                await MainWindow.DownloadImage(killer.defaultPortrait, MainWindow.DIR_RESOURCES_DEFAULT_ICONS);
                // Set the Image property to the downloaded (or cached) file
                killer.PortraitImage = new WriteableBitmap(new BitmapImage(new Uri(
                    Path.Combine(Environment.CurrentDirectory, $"{MainWindow.DIR_RESOURCES_DEFAULT_ICONS}{MainWindow.GetUniqueFilename(killer.defaultPortrait)}"))));

                // Iterating over a temp list to prevent code duplication
                foreach (Perk perk in new List<Perk>() { killer.PerkA, killer.PerkB, killer.PerkC })
                {
                    // Try and download the perk (exits early if already cached)
                    await MainWindow.DownloadImage(perk.defaultImage, MainWindow.DIR_RESOURCES_DEFAULT_ICONS);
                    // Set the Image property to the downloaded (or cached) file
                    perk.Image = new WriteableBitmap(new BitmapImage(new Uri(
                        Path.Combine(Environment.CurrentDirectory, $"{MainWindow.DIR_RESOURCES_DEFAULT_ICONS}{MainWindow.GetUniqueFilename(perk.defaultImage)}"))));
                }

                // TODO Addons
            }

            foreach (Perk perk in _commonKillerPerks)
            {
                // Try and download the perk (exits early if already cached)
                await MainWindow.DownloadImage(perk.defaultImage, MainWindow.DIR_RESOURCES_DEFAULT_ICONS);
                // Set the Image property to the downloaded (or cached) file
                perk.Image = new WriteableBitmap(new BitmapImage(new Uri(
                    Path.Combine(Environment.CurrentDirectory, $"{MainWindow.DIR_RESOURCES_DEFAULT_ICONS}{MainWindow.GetUniqueFilename(perk.defaultImage)}"))));
            }

            // TODO Set Overrides
        }
    }
}
