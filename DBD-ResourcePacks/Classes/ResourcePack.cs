using System.Collections.Generic;

namespace DBD_ResourcePacks
{
    public class ResourcePack
    {
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
