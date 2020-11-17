using Synapse.Api.Events.SynapseEventArguments;

namespace TestGamemode
{
    internal class CustomEventHandler
    {
        internal void OnRemoteAdminCommandEvent(RemoteAdminCommandEventArgs ev)
        {
            SynapseController.Server.Logger.Info("I did it");
        }
    }
}