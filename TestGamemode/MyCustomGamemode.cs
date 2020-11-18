using Synapse.Api.Events.SynapseEventArguments;
using SynapseGamemode;

namespace TestGamemode
{
    [Gamemode(Author = "Wholesome", GitHubRepo = "None", Name = "CustomGamemode", Version = "1.0.0.0")]
    public class MyCustomGamemode : IGamemode
    {
        public void Init()
        {
            SynapseController.Server.Logger.Info("I am initializing");
            Synapse.Api.Events.EventHandler.Get.Map.TriggerTeslaEvent += Map_TriggerTeslaEvent;
        }
        private void Map_TriggerTeslaEvent(TriggerTeslaEventArgs ev)
        {
            SynapseController.Server.Logger.Info("Tesla Triggered");
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
