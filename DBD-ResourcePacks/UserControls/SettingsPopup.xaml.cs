using DBD_ResourcePacks.Properties;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DBD_ResourcePacks.UserControls
{
    public partial class SettingsPopup : Window
    {
        public SettingsPopup()
        {
            InitializeComponent();
            GamePath.Text = Settings.Default.GameInstallationPath;
            Theme.SelectedIndex = Settings.Default.ThemeSetting;

            packsVersionLabel.Content = "x.x.x";
            resourcesVersionLabel.Content = "x.x.x";
            programVersionLabel.Content = "x.x.x";
        }

        private void GamePath_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            e.Handled = true;
            string path = ((TextBox)sender).Text;

            if (!MainWindow.IsValidGamePath(path))
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
            MainWindow.UpdateTheme();
        }

        private void CheckUpdatePacks(object sender, RoutedEventArgs e) { }
        private void CheckUpdateResources(object sender, RoutedEventArgs e) { }
        private void CheckUpdateProgram(object sender, RoutedEventArgs e) { }

        private void ClearBrowseCache_Click(object sender, RoutedEventArgs e) { }
        private void ClearOldDownloadBanners_Click(object sender, RoutedEventArgs e) { }
        private void ClearUICache_Click(object sender, RoutedEventArgs e) { }
        private void DeletePacks_Click(object sender, RoutedEventArgs e) { }
    }
}
