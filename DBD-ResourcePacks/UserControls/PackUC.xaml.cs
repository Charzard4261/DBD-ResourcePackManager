﻿using DBD_ResourcePackManager.Classes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DBD_ResourcePackManager.UserControls
{
    public partial class PackUC : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private ResourcePack _packInfo;
        public ResourcePack PackInfo
        {
            get => _packInfo;
            set
            {
                _packInfo = value;
                name.Content = _packInfo.name;
                chapter.Content = _packInfo.chapterName;
                version.Content = $"Version: {_packInfo.chapter.ToString("0.0")}.{_packInfo.packVersion}";
                credits.Inlines.Clear();
                credits.Inlines.Add(new Run(_packInfo.credits.Count == 1 ? "Author:" : "Authors:"));
                for (int creditIndex = 0; creditIndex < _packInfo.credits.Count; creditIndex++)
                {
                    credits.Inlines.Add(new Run(creditIndex > 0 ? ", " : " "));
                    Run run3 = new Run(_packInfo.credits[creditIndex].name);
                    if (_packInfo.credits[creditIndex].link != "")
                    {
                        Uri uri = new Uri(_packInfo.credits[creditIndex].link);
                        Hyperlink hyperlink = new Hyperlink(run3) { NavigateUri = uri };
                        hyperlink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(
                            (a, b) => Process.Start(new ProcessStartInfo() { FileName = b.Uri.OriginalString, UseShellExecute = true })
                            );
                        credits.Inlines.Add(hyperlink);
                    }
                    else
                        credits.Inlines.Add(run3);
                }
                if (_packInfo.credits.Count <= 0)
                    credits.Inlines.Clear();
                if (_packInfo.contains.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder("");
                    for (int i = 0; i < _packInfo.contains.Count; i++)
                    {
                        stringBuilder.Append(i > 0 ? ", " : "");
                        stringBuilder.Append(_packInfo.contains[i]);
                    }
                    tags.Text = stringBuilder.ToString();
                }
                else
                    tags.Text = "";
                banner.Visibility = (System.Windows.Visibility)1;
                Visibility = 0;
                NotifyPropertyChanged();
            }
        }

        public PackUC()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
