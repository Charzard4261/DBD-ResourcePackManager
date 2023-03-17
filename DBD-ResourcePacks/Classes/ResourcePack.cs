﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DBD_ResourcePacks
{
    public class ResourcePack : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private string _state = "";
        private bool _actionable = false;
        [JsonIgnore]
        public string PackState { get => _state; set { _state = value; NotifyPropertyChanged(); } }
        [JsonIgnore]
        public bool PackActionable { get => _actionable; set { _actionable = value; NotifyPropertyChanged(); } }

        public string uniqueKey = "";
        public string name = "";
        public float chapter;
        public int packVersion;
        public string bannerLink = "";
        public string downloadLink = "";
        public List<Credit> credits;
        public List<string> tags;
    }
}