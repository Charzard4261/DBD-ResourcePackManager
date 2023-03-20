using DBD_ResourcePacks.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DBD_ResourcePacks.UserControls
{
    /// <summary>
    /// Interaction logic for PackSelect.xaml
    /// </summary>
    public partial class PackSelect : Window
    {
        public string unique_key;
        List<ResourcePack> _resourcePacks;
        Dictionary<string, Button> _packToButton = new();

        public PackSelect(List<ResourcePack> resourcePacks)
        {
            _resourcePacks = resourcePacks;
            InitializeComponent();

            foreach (ResourcePack pack in resourcePacks)
            {
                Button button = new Button();
                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.Content = pack.name;
                button.Tag = pack.uniqueKey;
                button.Click += OnPackSelect;
                _packToButton.Add(pack.uniqueKey, button);

                packsStackPanel.Children.Add(button);
            }
        }

        public void OnPackSelect(object sender, RoutedEventArgs e)
        {
            unique_key = (string)((Button)sender).Tag;
            DialogResult = true;
            Close();
        }

        private void Search_KeyUp(object sender, KeyEventArgs e)
        {
            string text = ((TextBox)sender).Text;
            foreach (ResourcePack pack in _resourcePacks)
            {
                if (pack.name.StartsWith(text) || pack.uniqueKey.StartsWith(text))
                    _packToButton[pack.uniqueKey].Visibility = Visibility.Visible;
                else
                    _packToButton[pack.uniqueKey].Visibility = Visibility.Collapsed;
            }
        }
    }
}
