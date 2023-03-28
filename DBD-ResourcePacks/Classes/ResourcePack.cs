using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DBD_ResourcePackManager.Classes
{
    public class ResourcePack : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private string _state = "";
        private bool _actionable = false;
        [JsonIgnore] public string PackState { get => _state; set { _state = value; NotifyPropertyChanged(); } }
        [JsonIgnore] public bool PackActionable { get => _actionable; set { _actionable = value; NotifyPropertyChanged(); } }

        [JsonIgnore] public string folder = "";
        public string uniqueKey = "";
        public string name = "";
        public float chapter;
        [JsonIgnore] public string chapterName = "";
        public float packVersion;
        public string bannerLink = "";
        public string downloadLink = "";
        public string fileType = "";
        public List<Credit> credits = new();
        public List<string> contains = new();
        public List<string> tags = new();

        public bool ShouldShowUpIn(string search)
        {
            if (name.ToLower().Contains(search.ToLower()) || uniqueKey.ToLower().Contains(search.ToLower()))
                return true;
            foreach (string tag in tags)
                if (tag.ToLower().Contains(search.ToLower()))
                    return true;
            return false;
        }
    }
}
