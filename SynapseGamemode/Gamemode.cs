using System;

namespace SynapseGamemode
{
    public class Gamemode : Attribute
    {
        public string Name { get; set; } = "Unknown";
        public string Author { get; set; } = "Unknown";
        public string GitHubRepo { get; set; } = "Unknown";
        public string Version { get; set; } = "0.0.0.0";
    }
}
