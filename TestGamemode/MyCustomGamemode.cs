using SynapseGamemode;
using System;

namespace TestGamemode
{
    [Gamemode(Author = "Wholesome", GitHubRepo = "None", Name = "My Custom Gamemode", Version = "1.0.0.0")]
    public class MyCustomGamemode : IGamemode
    {
        private CustomEventHandler eventHandler;
        public void Init()
        {
            eventHandler = new CustomEventHandler();
            Synapse.Api.Events.EventHandler.Get.Server.RemoteAdminCommandEvent += eventHandler.OnRemoteAdminCommandEvent;

            SynapseController.Server.Logger.Info("I am initializing");
        }
        public void Start()
        {
            SynapseController.Server.Logger.Info("I am starting");
        }
        public void End()
        {
            Synapse.Api.Events.EventHandler.Get.Server.RemoteAdminCommandEvent -= eventHandler.OnRemoteAdminCommandEvent;

            SynapseController.Server.Logger.Info("I am ending");
        }
    }
}
