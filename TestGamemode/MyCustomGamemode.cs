using SynapseGamemode;
using System;

namespace TestGamemode
{
    [Gamemode(Author = "Wholesome", GitHubRepo = "None", Name = "My Custom Gamemode", Version = "1.0.0.0")]
    public class MyCustomGamemode : IGamemode
    {
        public void Init()
        {
            SynapseController.Server.Logger.Info("I am initializing");
        }
        public void Start()
        {
            SynapseController.Server.Logger.Info("I am starting");
        }
        public void ForceStop()
        {
            SynapseController.Server.Logger.Info("I am force stopping");
        }
    }
}
