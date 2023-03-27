using DBD_ResourcePackManager.Classes;
using DBD_ResourcePackManager.Properties;
using GameFinder.StoreHandlers.EGS;
using GameFinder.StoreHandlers.Steam;
using Microsoft.Win32;
using Octokit;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DBD_ResourcePackManager.UserControls
{
    public partial class SettingsPopup : Window
    {
        MainWindow _mainWindow;

        public SettingsPopup(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            GamePath.Text = Settings.Default.GameInstallationPath;
            Theme.SelectedIndex = Settings.Default.ThemeSetting;

            packsVersionLabel.Content     = $"{Settings.Default.PacksVersionMajor}.{Settings.Default.PacksVersionMinor}";
            resourcesVersionLabel.Content = $"{Settings.Default.ResourcesVersionMajor}.{Settings.Default.ResourcesVersionMinor}";
            programVersionLabel.Content   = $"{Settings.Default.ProgramVersionMajor}.{Settings.Default.ProgramVersionMinor}.{Settings.Default.ProgramVersionPatch}";
            autoUpdate.IsChecked          = Settings.Default.AutoUpdate;
        }

        private void GamePath_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            e.Handled = true;
            string path = ((TextBox)sender).Text;

            if (!Constants.IsValidGamePath(path))
            {
                ((TextBox)sender).Text = Settings.Default.GameInstallationPath;
                return;
            }

            Settings.Default.GameInstallationPath = path;
            Settings.Default.Save();
        }

        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is not ComboBox comboBox)
                return;

            Settings.Default.ThemeSetting = comboBox.SelectedIndex;
            Settings.Default.Save();
            Constants.UpdateTheme();
        }

        private void CheckUpdatePacks(object sender, RoutedEventArgs e)
        {
            switch (_mainWindow.CheckForPacksUpdate())
            {
                case 0:
                    MessageBox.Show("No updates found.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 1:
                    MessageBox.Show("Updated!\nRestart to apply.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    packsVersionLabel.Content = $"{Settings.Default.PacksVersionMajor}.{Settings.Default.PacksVersionMinor}";
                    break;
                case -1:
                default:
                    break;
            }
        }
        private void CheckUpdateResources(object sender, RoutedEventArgs e)
        {
            switch (_mainWindow.CheckForResourcesUpdate())
            {
                case 0:
                    MessageBox.Show("No updates found.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 1:
                    MessageBox.Show("Updated!\nRestart to apply.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    resourcesVersionLabel.Content = $"{Settings.Default.ResourcesVersionMajor}.{Settings.Default.ResourcesVersionMinor}";
                    break;
                case -1:
                default:
                    break;
            }
        }
        private void CheckUpdateProgram(object sender, RoutedEventArgs e)
        {
            switch (_mainWindow.CheckForProgramUpdate())
            {
                case 0:
                    MessageBox.Show("No updates found.", "Information",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 1:
                case -1:
                default:
                    break;
            }
        }
        private void ToggleAutoUpdate(object sender, RoutedEventArgs e)
        {
            // IsChecked is a bool? so == true needed
            Settings.Default.AutoUpdate = ((CheckBox)sender).IsChecked == true;
            Settings.Default.Save();
        }

        private void ClearBrowseCache_Click(object sender, RoutedEventArgs e)
        {
            foreach (string name in Directory.GetFiles($"{_mainWindow.appFolder}\\{Constants.DIR_CACHE_BROWSE}"))
                File.Delete(name);
        }
        private void ClearOldDownloadBanners_Click(object sender, RoutedEventArgs e)
        {
            foreach (ResourcePack pack in _mainWindow.Register.downloadedRegistry.Values)
            {
                foreach (string name in Directory.GetFiles($"{_mainWindow.appFolder}\\{Constants.DIR_DOWNLOADED}\\{pack.uniqueKey}"))
                    if (!name.EndsWith(".json") && !name.EndsWith(Constants.GetUniqueFilename(pack.bannerLink)))
                        File.Delete(name);
            }
        }
        private void ClearDefaultImageCache_Click(object sender, RoutedEventArgs e)
        {
            foreach (string name in Directory.GetFiles($"{_mainWindow.appFolder}\\{Constants.DIR_DEFAULT_ICONS}"))
                File.Delete(name);
            Close();
            _mainWindow.Close();
        }
        private void OpenCache_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", $"{_mainWindow.appFolder}\\{Constants.DIR_CACHE}");
        }

        private void DetectGameInstallationFolder_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult firstResult = 
                MessageBox.Show("Is Dead by Daylight installed through Steam?",
                "Game Installation Finder", 
                MessageBoxButton.YesNo);
            if (firstResult == MessageBoxResult.Yes)
            {
                //Steam handler from GameFinder library.
                SteamHandler handler = new SteamHandler();
                //SteamDB said, DBD ID is 381210
                SteamGame? dbd = handler.FindOneGameById(381210, out _);
                if (dbd is not null)
                {
                    Settings.Default.GameInstallationPath = dbd.Path;
                    GamePath.Text = dbd.Path;
                    Settings.Default.Save();
                    MessageBox.Show("Steam installation found!",
                        "Game Installation Finder", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Steam installation not found.",
                        "Game Installation Finder", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
                return;
            }

            MessageBoxResult secondResult = 
                MessageBox.Show("Is Dead by Daylight installed through the Epic Games Store?",
                "Game Installation Finder", 
                MessageBoxButton.YesNo);
            if (secondResult == MessageBoxResult.Yes)
            {
                EGSHandler handler = new EGSHandler();
                //Search
                EGSGame? dbd = handler.FindOneGameById("611482b8586142cda48a0786eb8a127c:467a7bed47ec44d9b1c9da0c2dae58f7:Brill", out _);
                if (dbd is not null)
                {
                    Settings.Default.GameInstallationPath = dbd.InstallLocation;
                    GamePath.Text = dbd.InstallLocation;
                    Settings.Default.Save();
                    MessageBox.Show("Epic Games Store installation found!",
                        "Game Installation Finder", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Epic Games Store installation not found.",
                        "Game Installation Finder", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
                return;
            }
        }
    }
}
