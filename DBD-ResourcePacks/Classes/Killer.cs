using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace DBD_ResourcePacks.Classes
{
    public class Killer : Character
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private WriteableBitmap _powerImage;
        [JsonIgnore]
        public WriteableBitmap PowerImage { get => _powerImage; set { _powerImage = value; NotifyPropertyChanged(); } }

        public List<string> powers;
        public string defaultPower = "";

        public List<Addon> addons;
    }
}
