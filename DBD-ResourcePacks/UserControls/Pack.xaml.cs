using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DBD_ResourcePacks.UserControls
{
    public partial class Pack : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        public ResourcePack _packInfo;
        public ResourcePack PackInfo
        {
            get => _packInfo;
            set
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
                //action.IsEnabled = _packInfo.downloadLink != "";
                Visibility = 0;
                NotifyPropertyChanged();
            }
        }

        public Pack()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
