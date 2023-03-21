using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DBD_ResourcePackManager.Classes;

namespace DBD_ResourcePackManager.UserControls
{
    public partial class PerkUC : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private Perk _perk;
        public Perk Perk
        {
            get => _perk;
            set
            {
                _perk = value;
                Visibility = 0;
                NotifyPropertyChanged();
            }
        }

        public PerkUC()
        {
            InitializeComponent();
        }
    }
}
