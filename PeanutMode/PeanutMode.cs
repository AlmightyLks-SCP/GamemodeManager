using Synapse.Api.Events.SynapseEventArguments;
using SynapseGamemode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeanutMode
{
    [Gamemode(Author = "Wholesome", GitHubRepo = "None", Name = "PeanutMode", Version = "1.0.0.0")]
    public class PeanutMode : IGamemode
    {
        private PluginHandler pluginHandler = new PluginHandler();
        public void Init()
        {
            Synapse.Api.Events.EventHandler.Get.Map.DoorInteractEvent += pluginHandler.OnDoorInteract;
        }
        public void Start()
        {
            SynapseController.Server.Logger.Info("Gamemode Activated");
        }
        public void End()
        {
            SynapseController.Server.Logger.Info("Gamemode Deactivated");
        }
    }
}
