using Synapse.Api.Events.SynapseEventArguments;
using System;

namespace PeanutMode
{
    internal class PluginHandler
    {
        internal void Map_TriggerTeslaEvent(TriggerTeslaEventArgs ev)
        {
            SynapseController.Server.Logger.Info("Tesla Triggered");
        }

        internal void OnDoorInteract(DoorInteractEventArgs ev)
        {
            SynapseController.Server.Logger.Info("Door triggered");
        }
    }
}