﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace DBD_ResourcePackManager.Classes
{
    public class Character : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        private WriteableBitmap _portraitImage;
        [JsonIgnore] public WriteableBitmap PortraitImage { get => _portraitImage; set { _portraitImage = value; NotifyPropertyChanged(); } }
        private WriteableBitmap _additionalImage;
        [JsonIgnore] public WriteableBitmap AdditionalImage { get => _additionalImage; set { _additionalImage = value; NotifyPropertyChanged(); } }

        [JsonIgnore] public string key = "";
        public string portrait = "";
        public string defaultPortrait = "";

        [JsonIgnore] private Perk _perkA;
        public Perk PerkA { get => _perkA; set { _perkA = value; NotifyPropertyChanged(); } }

        [JsonIgnore] private Perk _perkB;
        public Perk PerkB { get => _perkB; set { _perkB = value; NotifyPropertyChanged(); } }

        [JsonIgnore] private Perk _perkC;
        public Perk PerkC { get => _perkC; set { _perkC = value; NotifyPropertyChanged(); } }
    }
}
