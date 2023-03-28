using DBD_ResourcePackManager.Classes;
using Newtonsoft.Json;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Windows;

namespace DBD_ResourcePackManager.UserControls
{
    public partial class PackImporter : Window
    {
        MainWindow _mainWindow;

        string _fileActions       = "";
        string _fileCharPortraits = "";
        string _fileCustomization = "";
        string _fileDailyRituals  = "";
        string _fileEmblems       = "";
        string _fileFavors        = "";
        string _filePerks         = "";
        string _fileHelp          = "";
        string _fileHelpLoading   = "";
        string _fileItemAddons    = "";
        string _fileItems         = "";
        string _filePacks         = "";
        string _filePowers        = "";
        string _fileStatusEffects = "";

        public PackImporter(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            InitializeComponent();
        }

        void Pack_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string fileName = Path.GetFileNameWithoutExtension(files[0]);

                _fileActions              = "";
                _fileCharPortraits        = "";
                _fileCustomization        = "";
                _fileDailyRituals         = "";
                _fileEmblems              = "";
                _fileFavors               = "";
                _filePerks                = "";
                _fileHelp                 = "";
                _fileHelpLoading          = "";
                _fileItemAddons           = "";
                _fileItems                = "";
                _filePacks                = "";
                _filePowers               = "";
                _fileStatusEffects        = "";

                TextBoxActions.Text       = "";
                TextBoxCharPortraits.Text = "";
                TextBoxCustomization.Text = "";
                TextBoxDailyRituals.Text  = "";
                TextBoxEmblems.Text       = "";
                TextBoxFavors.Text        = "";
                TextBoxPerks.Text         = "";
                TextBoxHelp.Text          = "";
                TextBoxHelpLoading.Text   = "";
                TextBoxItemAddons.Text    = "";
                TextBoxItems.Text         = "";
                TextBoxPacks.Text         = "";
                TextBoxPowers.Text        = "";
                TextBoxStatusEffects.Text = "";

                try
                {
                    LabelDropFile.Content = files[0];

                    string tempDirectory = $"{_mainWindow.appFolder}\\{Constants.DIR_CACHE_IMPORT}\\{fileName}";

                    Directory.CreateDirectory(tempDirectory);

                    using (Stream stream = File.OpenRead(files[0]))
                    using (var reader = ReaderFactory.Open(stream))
                    {
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {
                                Console.WriteLine(reader.Entry.Key);
                                reader.WriteEntryToDirectory(tempDirectory, new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }
                        }
                    }

                    RecurseSearch(new DirectoryInfo(tempDirectory));

                    TextBoxName.Text = fileName;

                    if (_fileActions != "")       TextBoxActions.Text       = _fileActions.Substring(tempDirectory.Length + 1);
                    if (_fileCharPortraits != "") TextBoxCharPortraits.Text = _fileCharPortraits.Substring(tempDirectory.Length + 1);
                    if (_fileCustomization != "") TextBoxCustomization.Text = _fileCustomization.Substring(tempDirectory.Length + 1);
                    if (_fileDailyRituals != "")  TextBoxDailyRituals.Text  = _fileDailyRituals.Substring(tempDirectory.Length + 1);
                    if (_fileEmblems != "")       TextBoxEmblems.Text       = _fileEmblems.Substring(tempDirectory.Length + 1);
                    if (_fileFavors != "")        TextBoxFavors.Text        = _fileFavors.Substring(tempDirectory.Length + 1);
                    if (_filePerks != "")         TextBoxPerks.Text         = _filePerks.Substring(tempDirectory.Length + 1);
                    if (_fileHelp != "")          TextBoxHelp.Text          = _fileHelp.Substring(tempDirectory.Length + 1);
                    if (_fileHelpLoading != "")   TextBoxHelpLoading.Text   = _fileHelpLoading.Substring(tempDirectory.Length + 1);
                    if (_fileItemAddons != "")    TextBoxItemAddons.Text    = _fileItemAddons.Substring(tempDirectory.Length + 1);
                    if (_fileItems != "")         TextBoxItems.Text         = _fileItems.Substring(tempDirectory.Length + 1);
                    if (_filePacks != "")         TextBoxPacks.Text         = _filePacks.Substring(tempDirectory.Length + 1);
                    if (_filePowers != "")        TextBoxPowers.Text        = _filePowers.Substring(tempDirectory.Length + 1);
                    if (_fileStatusEffects != "") TextBoxStatusEffects.Text = _fileStatusEffects.Substring(tempDirectory.Length + 1);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, $"Error Importing {fileName}", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void RecurseSearch(DirectoryInfo parentDirectory)
        {
            // Not Switch so that we can use Ordinal Ignore Case
            if (parentDirectory.Name.Equals(Constants.FOLDER_ACTIONS, StringComparison.OrdinalIgnoreCase))
                _fileActions = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_PORTRAITS, StringComparison.OrdinalIgnoreCase))
                _fileCharPortraits = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_CUSTOMIZATION, StringComparison.OrdinalIgnoreCase))
                _fileCustomization = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_DAILY_RITUALS, StringComparison.OrdinalIgnoreCase))
                _fileDailyRituals = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_EMBLEMS, StringComparison.OrdinalIgnoreCase))
                _fileEmblems = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_OFFERINGS, StringComparison.OrdinalIgnoreCase))
                _fileFavors = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_PERKS, StringComparison.OrdinalIgnoreCase))
                _filePerks = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_HELP, StringComparison.OrdinalIgnoreCase))
                _fileHelp = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_HELPLOADING, StringComparison.OrdinalIgnoreCase))
                _fileHelpLoading = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_ADDONS, StringComparison.OrdinalIgnoreCase))
                _fileItemAddons = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_ITEMS, StringComparison.OrdinalIgnoreCase))
                _fileItems = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_PACKS, StringComparison.OrdinalIgnoreCase))
                _filePacks = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_POWERS, StringComparison.OrdinalIgnoreCase))
                _filePowers = parentDirectory.FullName;
            else if (parentDirectory.Name.Equals(Constants.FOLDER_STATUS_EFFECTS, StringComparison.OrdinalIgnoreCase))
                _fileStatusEffects = parentDirectory.FullName;
            else
                foreach (DirectoryInfo directoryInfo in parentDirectory.GetDirectories())
                    RecurseSearch(directoryInfo);
        }

        void Import_Click(object sender, RoutedEventArgs e)
        {
            // Validate
            if (!float.TryParse(TextBoxChapter.Text, out float chapter))
            {
                MessageBox.Show($"Invalid Chapter Number: {TextBoxChapter.Text}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ResourcePack importedPack = new ResourcePack();
            importedPack.uniqueKey = $"Imported-{TextBoxName.Text.Replace(" ", "_")}-{Guid.NewGuid()}";

            string folder = $"{_mainWindow.appFolder}\\{Constants.DIR_IMPORTED}\\{importedPack.uniqueKey}";
            string sourceFolder = $"{_mainWindow.appFolder}\\{Constants.DIR_CACHE_IMPORT}\\{Path.GetFileNameWithoutExtension((string)LabelDropFile.Content)}";

            importedPack.folder = folder;
            importedPack.name = TextBoxName.Text;
            importedPack.chapter = chapter;
            importedPack.chapterName = _mainWindow.Customiser.GetChapter(chapter);
            importedPack.PackActionable = false;
            importedPack.PackState = "Imported";

            if (TextBoxBanner.Text != "") importedPack.bannerLink = Path.GetFileName(TextBoxBanner.Text);

            if (_fileActions != "")       importedPack.contains.Add(Constants.CONTAINS_POWERS);
            if (_fileCharPortraits != "") importedPack.contains.Add(Constants.CONTAINS_PORTRAITS);
            if (_fileCustomization != "") importedPack.contains.Add(Constants.CONTAINS_MISC_UI);
            if (_fileDailyRituals != "")  importedPack.contains.Add(Constants.CONTAINS_MISC_UI);
            if (_fileEmblems != "")       importedPack.contains.Add(Constants.CONTAINS_MISC_UI);
            if (_fileFavors != "")        importedPack.contains.Add(Constants.CONTAINS_OFFERINGS);
            if (_filePerks != "")         importedPack.contains.Add(Constants.CONTAINS_PERKS);
            if (_fileHelp != "")          importedPack.contains.Add(Constants.CONTAINS_MISC_UI);
            if (_fileHelpLoading != "")   importedPack.contains.Add(Constants.CONTAINS_MISC_UI);
            if (_fileItemAddons != "")    importedPack.contains.Add(Constants.CONTAINS_ADDONS);
            if (_fileItems != "")         importedPack.contains.Add(Constants.CONTAINS_ITEMS);
            if (_filePacks != "")         importedPack.contains.Add(Constants.CONTAINS_MISC_UI);
            if (_filePowers != "")        importedPack.contains.Add(Constants.CONTAINS_POWERS);
            if (_fileStatusEffects != "") importedPack.contains.Add(Constants.CONTAINS_MISC_UI);
            importedPack.contains = importedPack.contains.Distinct().ToList();

            Directory.CreateDirectory(folder);
            File.WriteAllText($"{folder}\\pack.json", JsonConvert.SerializeObject(importedPack, Formatting.Indented));

            if (TextBoxBanner.Text != "")        Constants.CopyFile($"{sourceFolder}\\{TextBoxBanner.Text}", $"{folder}\\{Path.GetFileName(TextBoxBanner.Text)}");

            if (TextBoxActions.Text != "")       Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxActions.Text}"      , $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxActions.Text)}");
            if (TextBoxCharPortraits.Text != "") Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxCharPortraits.Text}", $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxCharPortraits.Text)}");
            if (TextBoxCustomization.Text != "") Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxCustomization.Text}", $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxCustomization.Text)}");
            if (TextBoxDailyRituals.Text != "")  Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxDailyRituals.Text}" , $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxDailyRituals.Text)}");
            if (TextBoxEmblems.Text != "")       Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxEmblems.Text}"      , $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxEmblems.Text)}");
            if (TextBoxFavors.Text != "")        Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxFavors.Text}"       , $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxFavors.Text)}");
            if (TextBoxPerks.Text != "")         Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxPerks.Text}"        , $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxPerks.Text)}");
            if (TextBoxHelp.Text != "")          Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxHelp.Text}"         , $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxHelp.Text)}");
            if (TextBoxHelpLoading.Text != "")   Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxHelpLoading.Text}"  , $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxHelpLoading.Text)}");
            if (TextBoxItemAddons.Text != "")    Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxItemAddons.Text}"   , $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxItemAddons.Text)}");
            if (TextBoxItems.Text != "")         Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxItems.Text}"        , $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxItems.Text)}");
            if (TextBoxPacks.Text != "")         Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxPacks.Text}"        , $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxPacks.Text)}");
            if (TextBoxPowers.Text != "")        Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxPowers.Text}"       , $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxPowers.Text)}");
            if (TextBoxStatusEffects.Text != "") Constants.CopyFilesRecursively($"{sourceFolder}\\{TextBoxStatusEffects.Text}", $"{folder}\\Pack\\{Path.GetFileNameWithoutExtension(TextBoxStatusEffects.Text)}");

            Directory.Delete(sourceFolder, true);

            _mainWindow.Register.downloadedRegistry.Add(importedPack.uniqueKey, importedPack);
            _mainWindow.Register.RebuildDownloadList();
            Close();
        }
    }
}
