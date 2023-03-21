using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace DBD_ResourcePackManager.Classes
{
    public class Perk : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private BitmapImage _image;
        [JsonIgnore] public BitmapImage Image { get => _image; set { _image = value; NotifyPropertyChanged(); } }

        [JsonIgnore] public bool forSurvivor;
        [JsonIgnore] public Character fromCharacter;

        public string key = "";
        public string filePath = "";
        public string defaultImage = "";
    }
}
