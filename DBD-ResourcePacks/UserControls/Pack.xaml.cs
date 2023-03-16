using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net;

namespace DBD_ResourcePacks.UserControls
{
    public partial class Pack : UserControl
    {
        public ResourcePack _packInfo;
        public ResourcePack PackInfo { get => _packInfo; set
            {
                _packInfo = value;
                name.Content = _packInfo.name;
                chapter.Content = _packInfo.chapter;
                version.Content = $"Version: {_packInfo.chapter.ToString("0.0")}.{_packInfo.packVersion}";
                StringBuilder creditsBuilder = new StringBuilder("Author");
                creditsBuilder.Append(_packInfo.credits.Count == 1 ? ":" : "s:");
                for (int creditIndex = 0; creditIndex < _packInfo.credits.Count; creditIndex++)
                {
                    if (creditIndex > 0)
                        creditsBuilder.Append(",");
                    creditsBuilder.Append(" ");
                    creditsBuilder.Append(_packInfo.credits[creditIndex].name);
                }
                credits.Content = creditsBuilder.ToString();
                banner.Visibility = (System.Windows.Visibility)1;
                action.IsEnabled = _packInfo.downloadLink != "";
                Visibility = 0;
            }
        }

        public Pack()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
