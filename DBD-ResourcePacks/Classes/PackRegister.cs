using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DBD_ResourcePacks.Classes
{
    public class PackRegister : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        MainWindow _mainWindow;

        public Dictionary<string, ResourcePack> packRegistry = new();
        public Dictionary<string, ResourcePack> downloadedRegistry = new();
        public List<string> downloadedPagePacks;
        public List<string> browsePagePacks;

        #region Download Search / Filter
        string _downloadSearch = "";
        public string DownloadSearch { get => _downloadSearch; set { _downloadSearch = value; RebuildDownloadList(); NotifyPropertyChanged(); } }

        bool _downloadPortraits = true;
        bool _downloadPerks     = true;
        bool _downloadItems     = true;
        bool _downloadAddons    = true;
        bool _downloadPowers    = true;
        bool _downloadOfferings = true;
        bool _downloadMiscUI    = true;

        public bool DownloadPortraits { get => _downloadPortraits; set { _downloadPortraits = value; RebuildDownloadList(); NotifyPropertyChanged(); } }
        public bool DownloadPerks     { get => _downloadPerks;     set { _downloadPerks = value;     RebuildDownloadList(); NotifyPropertyChanged(); } }
        public bool DownloadItems     { get => _downloadItems;     set { _downloadItems = value;     RebuildDownloadList(); NotifyPropertyChanged(); } }
        public bool DownloadAddons    { get => _downloadAddons;    set { _downloadAddons = value;    RebuildDownloadList(); NotifyPropertyChanged(); } }
        public bool DownloadPowers    { get => _downloadPowers;    set { _downloadPowers = value;    RebuildDownloadList(); NotifyPropertyChanged(); } }
        public bool DownloadOfferings { get => _downloadOfferings; set { _downloadOfferings = value; RebuildDownloadList(); NotifyPropertyChanged(); } }
        public bool DownloadMiscUI    { get => _downloadMiscUI;    set { _downloadMiscUI = value;    RebuildDownloadList(); NotifyPropertyChanged(); } }
        #endregion

        #region Browse Search / Filter
        string _browseSearch = "";
        public string BrowseSearch { get => _browseSearch; set { _browseSearch = value; RebuildBrowseList(); NotifyPropertyChanged(); } }

        bool _browsePortraits = true;
        bool _browsePerks = true;
        bool _browseItems = true;
        bool _browseAddons = true;
        bool _browsePowers = true;
        bool _browseOfferings = true;
        bool _browseMiscUI = true;

        public bool BrowsePortraits { get => _browsePortraits; set { _browsePortraits = value; RebuildBrowseList(); NotifyPropertyChanged(); } }
        public bool BrowsePerks { get => _browsePerks; set { _browsePerks = value; RebuildBrowseList(); NotifyPropertyChanged(); } }
        public bool BrowseItems { get => _browseItems; set { _browseItems = value; RebuildBrowseList(); NotifyPropertyChanged(); } }
        public bool BrowseAddons { get => _browseAddons; set { _browseAddons = value; RebuildBrowseList(); NotifyPropertyChanged(); } }
        public bool BrowsePowers { get => _browsePowers; set { _browsePowers = value; RebuildBrowseList(); NotifyPropertyChanged(); } }
        public bool BrowseOfferings { get => _browseOfferings; set { _browseOfferings = value; RebuildBrowseList(); NotifyPropertyChanged(); } }
        public bool BrowseMiscUI { get => _browseMiscUI; set { _browseMiscUI = value; RebuildBrowseList(); NotifyPropertyChanged(); } }
        #endregion

        public PackRegister(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        void RebuildDownloadList()
        {
            // Filter the potential packs by search name
            List<ResourcePack> packs = downloadedRegistry.Values.Where(pack => pack.name.Contains(_downloadSearch) || pack.uniqueKey.Contains(_downloadSearch)).ToList();
            List<string> filtered = new();

            // Add a pack to list if it contains a tag that is selected
            if (_downloadPortraits)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_PORTRAITS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_downloadPerks)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_PERKS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_downloadItems)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_ITEMS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_downloadAddons)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_ADDONS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_downloadPowers)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_POWERS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_downloadOfferings)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_OFFERINGS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_downloadMiscUI)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_OFFERINGS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);

            downloadedPagePacks = filtered;
            _mainWindow.LoadDownloadPage(0);
        }

        public List<ResourcePack> GetDownloadedPacks()
        {
            List<ResourcePack> packs = new();

            foreach (string key in downloadedPagePacks)
                packs.Add(downloadedRegistry[key]);

            return packs;
        }

        void RebuildBrowseList()
        {
            // Filter the potential packs by search name
            List<ResourcePack> packs = packRegistry.Values.Where(pack => pack.name.Contains(_browseSearch) || pack.uniqueKey.Contains(_browseSearch)).ToList();
            List<string> filtered = new();

            // Add a pack to list if it contains a tag that is selected
            if (_browsePortraits)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_PORTRAITS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_browsePerks)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_PERKS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_browseItems)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_ITEMS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_browseAddons)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_ADDONS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_browsePowers)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_POWERS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_browseOfferings)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_OFFERINGS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);
            if (_browseMiscUI)
                foreach (ResourcePack pack in packs.Where(pack => pack.tags.Contains(Constants.TAG_OFFERINGS)))
                    if (!filtered.Contains(pack.uniqueKey))
                        filtered.Add(pack.uniqueKey);

            browsePagePacks = filtered;
            _mainWindow.LoadBrowsePage(0);
        }
    }
}
