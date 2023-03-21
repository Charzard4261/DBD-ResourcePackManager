using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace DBD_ResourcePackManager.Classes
{
    public class Customiser : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        MainWindow _mainWindow;
        public Dictionary<string, Survivor> _survivors = new();
        public List<Perk> _commonSurvivorPerks = new();
        public Dictionary<string, Killer> _killers = new();
        public List<Perk> _commonKillerPerks = new();

        public CustomiserSave save;

        #region Pack Key Variables and Properties
        public string Everything           { get => save.everything;           set { save.everything = value;           NotifyPropertyChanged("EverythingPackName"); } }
        public string AllPortraits         { get => save.allPortraits;         set { save.allPortraits = value;         NotifyPropertyChanged("AllPortraitsPackName"); } }
        public string AllPerks             { get => save.allPerks;             set { save.allPerks = value;             NotifyPropertyChanged("AllPerksPackName"); } }
        public string AllItems             { get => save.allItems;             set { save.allItems = value;             NotifyPropertyChanged("AllItemsPackName"); } }
        public string AllKillerPowers      { get => save.allKillerPowers;      set { save.allKillerPowers = value;      NotifyPropertyChanged("AllKillerPowersPackName"); } }
        public string AllAddons            { get => save.allAddons;            set { save.allAddons = value;            NotifyPropertyChanged("AllAddonsPackName"); } }
        public string AllOfferings         { get => save.allOfferings;         set { save.allOfferings = value;         NotifyPropertyChanged("AllOfferingsPackName"); } }
        public string AllEmblems           { get => save.allEmblems;           set { save.allEmblems = value;           NotifyPropertyChanged("AllEmblemsPackName"); } }
        public string AllStatusEffects     { get => save.allStatusEffects;     set { save.allStatusEffects = value;     NotifyPropertyChanged("AllStatusEffectsPackName"); } }
        public string AllMiscUI            { get => save.allMiscUI;            set { save.allMiscUI = value;            NotifyPropertyChanged("AllMiscUIPackName"); } }

        public string AllSurvivors         { get => save.allSurvivors;         set { save.allSurvivors = value;         NotifyPropertyChanged("AllSurvivorsPackName"); } }
        public string AllSurvivorPortraits { get => save.allSurvivorPortraits; set { save.allSurvivorPortraits = value; NotifyPropertyChanged("AllSurvivorPortraitsPackName"); } }
        public string AllSurvivorPerks     { get => save.allSurvivorPerks;     set { save.allSurvivorPerks = value;     NotifyPropertyChanged("AllSurvivorPerksPackName"); } }
        public string AllItemAddons        { get => save.allItemAddons;        set { save.allItemAddons = value;        NotifyPropertyChanged("AllItemAddonsPackName"); } }
        public string AllSurvivorOfferings { get => save.allSurvivorOfferings; set { save.allSurvivorOfferings = value; NotifyPropertyChanged("AllSurvivorOfferingsPackName"); } }
        public string AllSurvivorEmblems   { get => save.allSurvivorEmblems;   set { save.allSurvivorEmblems = value;   NotifyPropertyChanged("AllSurvivorEmblemsPackName"); } }

        public string AllKillers           { get => save.allKillers;           set { save.allKillers = value;           NotifyPropertyChanged("AllKillersPackName"); } }
        public string AllKillerPortraits   { get => save.allKillerPortraits;   set { save.allKillerPortraits = value;   NotifyPropertyChanged("AllKillerPortraitsPackName"); } }
        public string AllKillerPerks       { get => save.allKillerPerks;       set { save.allKillerPerks = value;       NotifyPropertyChanged("AllKillerPerksPackName"); } }
        public string AllKillerAddons      { get => save.allKillerAddons;      set { save.allKillerAddons = value;      NotifyPropertyChanged("AllKillerAddonsPackName"); } }
        public string AllKillerOfferings   { get => save.allKillerOfferings;   set { save.allKillerOfferings = value;   NotifyPropertyChanged("AllKillerOfferingsPackName"); } }
        public string AllKillerEmblems     { get => save.allKillerEmblems;     set { save.allKillerEmblems = value;     NotifyPropertyChanged("AllKillerEmblemsPackName"); } }
        #endregion

        #region Pack Name for WPF Binding
        public string EverythingPackName           { get { return GetPackName(save.everything); } }
        public string AllPortraitsPackName         { get { return GetPackName(save.allPortraits); } }
        public string AllPerksPackName             { get { return GetPackName(save.allPerks); } }
        public string AllAddonsPackName            { get { return GetPackName(save.allAddons); } }
        public string AllOfferingsPackName         { get { return GetPackName(save.allOfferings); } }
        public string AllEmblemsPackName           { get { return GetPackName(save.allEmblems); } }
        public string AllStatusEffectsPackName     { get { return GetPackName(save.allStatusEffects); } }
        public string AllMiscUIPackName            { get { return GetPackName(save.allMiscUI); } }

        public string AllSurvivorsPackName         { get { return GetPackName(save.allSurvivors); } }
        public string AllSurvivorPortraitsPackName { get { return GetPackName(save.allSurvivorPortraits); } }
        public string AllSurvivorPerksPackName     { get { return GetPackName(save.allSurvivorPerks); } }
        public string AllItemsPackName             { get { return GetPackName(save.allItems); } }
        public string AllItemAddonsPackName        { get { return GetPackName(save.allItemAddons); } }
        public string AllSurvivorOfferingsPackName { get { return GetPackName(save.allSurvivorOfferings); } }
        public string AllSurvivorEmblemsPackName   { get { return GetPackName(save.allSurvivorEmblems); } }

        public string AllKillersPackName           { get { return GetPackName(save.allKillers); } }
        public string AllKillerPortraitsPackName   { get { return GetPackName(save.allKillerPortraits); } }
        public string AllKillerPerksPackName       { get { return GetPackName(save.allKillerPerks); } }
        public string AllKillerPowersPackName      { get { return GetPackName(save.allKillerPowers); } }
        public string AllKillerAddonsPackName      { get { return GetPackName(save.allKillerAddons); } }
        public string AllKillerOfferingsPackName   { get { return GetPackName(save.allKillerOfferings); } }
        public string AllKillerEmblemsPackName     { get { return GetPackName(save.allKillerEmblems); } }
        #endregion

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

        /// <summary>
        /// Get the image file for a Perk after user customising.
        /// Returns default file if:
        /// a) The pack cannot be found
        /// b) The pack does not contain an image for that Perk
        /// c) A "default" override is wanted
        /// </summary>
        /// <param name="perk">The Perk to get the image for</param>
        /// <returns>A copy of the image</returns>
        public BitmapImage GetImageForPerk(Perk perk)
        {
            string folder = "";
            string first = $"{_mainWindow.appFolder}\\{Constants.DIR_DOWNLOADED}\\";
            string second = $"\\Pack\\{Constants.FOLDER_PERKS}\\{perk.filePath}";

            if (save.everything != "" && File.Exists($"{first}{save.everything}{second}"))
                folder = save.everything;

            if (save.allPerks != "" && File.Exists($"{first}{save.allPerks}{second}"))
                folder = save.allPerks;

            if (perk.forSurvivor)
            {
                if (save.allSurvivors != "" && File.Exists($"{first}{save.allSurvivors}{second}"))
                    folder = save.allSurvivors;

                if (save.allSurvivorPerks != "" && File.Exists($"{first}{save.allSurvivorPerks}{second}"))
                    folder = save.allSurvivorPerks;
            }
            else
            {
                if (save.allKillers != "" && File.Exists($"{first}{save.allKillers}{second}"))
                    folder = save.allKillers;

                if (save.allKillerPerks != "" && File.Exists($"{first}{save.allKillerPerks}{second}"))
                    folder = save.allKillerPerks;
            }

            if (save.overrides.ContainsKey(perk.key))
                folder = save.overrides[perk.key];

            string file;
            if (folder == "" || folder == "default")
                file = $"{_mainWindow.appFolder}\\{Constants.DIR_DEFAULT_ICONS}\\{Constants.GetUniqueFilename(perk.defaultImage)}";
            else
                file = $"{first}{folder}{second}";

            // A final check in case the default image is corrupted
            if (File.Exists(file))
                return Constants.LoadImage(file);
            return null;
        }
        /// <summary>
        /// Get the image file for a Character after user customising.
        /// Returns default file if:
        /// a) The pack cannot be found
        /// b) The pack does not contain an image for that Character
        /// c) A "default" override is wanted
        /// </summary>
        /// <param name="character">The Character to get the image for</param>
        /// <returns>A copy of the image</returns>
        public BitmapImage GetImageForCharacter(Character character)
        {
            string folder = "";
            string first = $"{_mainWindow.appFolder}\\{Constants.DIR_DOWNLOADED}\\";
            string second = $"\\Pack\\{Constants.FOLDER_PORTRAITS}\\{character.portrait}";

            if (save.everything != "" && File.Exists($"{first}{save.everything}{second}"))
                folder = save.everything;

            if (save.allPortraits != "" && File.Exists($"{first}{save.allPortraits}{second}"))
                folder = save.allPortraits;

            if (character is Survivor)
            {
                if (save.allSurvivors != "" && File.Exists($"{first}{save.allSurvivors}{second}"))
                    folder = save.allSurvivors;

                if (save.allSurvivorPortraits != "" && File.Exists($"{first}{save.allSurvivorPortraits}{second}"))
                    folder = save.allSurvivorPortraits;
            }
            else if (character is Killer)
            {
                if (save.allKillers != "" && File.Exists($"{first}{save.allKillers}{second}"))
                    folder = save.allKillers;

                if (save.allKillerPortraits != "" && File.Exists($"{first}{save.allKillerPortraits}{second}"))
                    folder = save.allKillerPortraits;
            }

            if (save.overrides.ContainsKey(character.key))
                folder = save.overrides[character.key];

            string file;
            if (folder == "" || folder == "default")
                file = $"{_mainWindow.appFolder}\\{Constants.DIR_DEFAULT_ICONS}\\{Constants.GetUniqueFilename(character.defaultPortrait)}";
            else
                file = $"{first}{folder}{second}";

            // A final check in case the default image is corrupted
            if (File.Exists(file))
                return Constants.LoadImage(file);
            return null;
        }
        /// <summary>
        /// Get the image file for a Killer's Power after user customising.
        /// Returns default file if:
        /// a) The pack cannot be found
        /// b) The pack does not contain an image for that Power
        /// c) A "default" override is wanted
        /// </summary>
        /// <param name="character">The Killer to get the Power image for</param>
        /// <returns>A copy of the image</returns>
        public BitmapImage GetImageForPower(Killer character)
        {
            string folder = "";
            string first = $"{_mainWindow.appFolder}\\{Constants.DIR_DOWNLOADED}\\";
            string second = $"\\Pack\\{Constants.FOLDER_POWERS}\\{character.powers[0]}";

            if (save.everything != "" && File.Exists($"{first}{save.everything}{second}"))
                folder = save.everything;

            if (save.allKillers != "" && File.Exists($"{first}{save.allKillers}{second}"))
                    folder = save.allKillers;

            if (save.allKillerPortraits != "" && File.Exists($"{first}{save.allKillerPortraits}{second}"))
                    folder = save.allKillerPortraits;

            if (save.overrides.ContainsKey(character.powers[0]))
                folder = save.overrides[character.powers[0]];

            string file;
            if (folder == "" || folder == "default")
                file = $"{_mainWindow.appFolder}\\{Constants.DIR_DEFAULT_ICONS}\\{Constants.GetUniqueFilename(character.defaultPower)}";
            else
                file = $"{first}{folder}{second}";

            // A final check in case the default image is corrupted
            if (File.Exists(file))
                return Constants.LoadImage(file);
            return null;
        }

        public string GetPackName(string key)
        {
            if (key == "default")
                return "Default";
            else if (_mainWindow.Register.downloadedRegistry.ContainsKey(key))
                return _mainWindow.Register.downloadedRegistry[key].name;
            else
                return key;
        }

        public async void DownloadImages()
        {
            foreach (Survivor survivor in _survivors.Values)
            {
                // Try and download the potrait (exits early if already cached)
                await Constants.DownloadImage(survivor.defaultPortrait, $"{_mainWindow.appFolder}\\{Constants.DIR_DEFAULT_ICONS}");
                // Set the Image property to the downloaded (or cached) file
                survivor.PortraitImage = GetImageForCharacter(survivor);

                // Iterating over a temp list to prevent code duplication
                foreach (Perk perk in new List<Perk>() { survivor.PerkA, survivor.PerkB, survivor.PerkC })
                {
                    // Try and download the perk (exits early if already cached)
                    await Constants.DownloadImage(perk.defaultImage, $"{_mainWindow.appFolder}\\{Constants.DIR_DEFAULT_ICONS}");
                    // Set the Image property to the downloaded (or cached) file
                    perk.Image = GetImageForPerk(perk);
                }
            }

            foreach (Perk perk in _commonSurvivorPerks)
            {
                // Try and download the perk (exits early if already cached)
                await Constants.DownloadImage(perk.defaultImage, $"{_mainWindow.appFolder}\\{Constants.DIR_DEFAULT_ICONS}");
                // Set the Image property to the downloaded (or cached) file
                perk.Image = GetImageForPerk(perk);
            }

            foreach (Killer killer in _killers.Values)
            {
                // Try and download the potrait (exits early if already cached)
                await Constants.DownloadImage(killer.defaultPortrait, $"{_mainWindow.appFolder}\\{Constants.DIR_DEFAULT_ICONS}");
                // Set the Image property to the downloaded (or cached) file
                killer.PortraitImage = GetImageForCharacter(killer);

                // Iterating over a temp list to prevent code duplication
                foreach (Perk perk in new List<Perk>() { killer.PerkA, killer.PerkB, killer.PerkC })
                {
                    // Try and download the perk (exits early if already cached)
                    await Constants.DownloadImage(perk.defaultImage, $"{_mainWindow.appFolder}\\{Constants.DIR_DEFAULT_ICONS}");
                    // Set the Image property to the downloaded (or cached) file
                    perk.Image = GetImageForPerk(perk);
                }

                await Constants.DownloadImage(killer.defaultPower, $"{_mainWindow.appFolder}\\{Constants.DIR_DEFAULT_ICONS}");
                // Set the Image property to the downloaded (or cached) file
                killer.AdditionalImage = GetImageForPower(killer);
                // TODO Addons
            }

            foreach (Perk perk in _commonKillerPerks)
            {
                // Try and download the perk (exits early if already cached)
                await Constants.DownloadImage(perk.defaultImage, $"{_mainWindow.appFolder}\\{Constants.DIR_DEFAULT_ICONS}");
                // Set the Image property to the downloaded (or cached) file
                perk.Image = GetImageForPerk(perk);
            }

            // TODO Set Overrides
        }

        public void SetImages()
        {
            foreach (Survivor survivor in _survivors.Values)
            {
                if (survivor == null)
                    continue;
                // Set the Image property to the downloaded (or cached) file
                survivor.PortraitImage = GetImageForCharacter(survivor);

                // Iterating over a temp list to prevent code duplication
                foreach (Perk perk in new List<Perk>() { survivor.PerkA, survivor.PerkB, survivor.PerkC })
                {
                    // Set the Image property to the downloaded (or cached) file
                    perk.Image = GetImageForPerk(perk);
                }
            }

            foreach (Perk perk in _commonSurvivorPerks)
            {
                // Set the Image property to the downloaded (or cached) file
                perk.Image = GetImageForPerk(perk);
            }

            foreach (Killer killer in _killers.Values)
            {
                // Set the Image property to the downloaded (or cached) file
                killer.PortraitImage = GetImageForCharacter(killer);

                // Iterating over a temp list to prevent code duplication
                foreach (Perk perk in new List<Perk>() { killer.PerkA, killer.PerkB, killer.PerkC })
                {
                    // Set the Image property to the downloaded (or cached) file
                    perk.Image = GetImageForPerk(perk);
                }

                // Set the Image property to the downloaded (or cached) file
                killer.AdditionalImage = GetImageForPower(killer);
                // TODO Addons
            }

            foreach (Perk perk in _commonKillerPerks)
            {
                // Set the Image property to the downloaded (or cached) file
                perk.Image = GetImageForPerk(perk);
            }

            // TODO Set Overrides
        }
    }
}
