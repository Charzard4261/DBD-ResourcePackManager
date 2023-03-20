using DBD_ResourcePackManager.Classes;
using DBD_ResourcePackManager.Properties;
using Microsoft.Win32;
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

        private void CheckUpdatePacks(object sender, RoutedEventArgs e) { _mainWindow.CheckForPacksUpdate(); }
        private void CheckUpdateResources(object sender, RoutedEventArgs e) { _mainWindow.CheckForResourcesUpdate(); }
        private void CheckUpdateProgram(object sender, RoutedEventArgs e) { _mainWindow.CheckForProgramUpdate(); }

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
            foreach (string name in Directory.GetFiles($"{_mainWindow.appFolder}\\{Constants.DIR_RESOURCES_DEFAULT_ICONS}"))
                File.Delete(name);
            Close();
            _mainWindow.Close();
        }
        private void OpenCache_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", $"{_mainWindow.appFolder}\\{Constants.DIR_CACHE}");
        }
    }
}
