# GamemodeManager

---

## Installation:
Download the `CustomGamemode.dll` and place it within your `Synapse\dependencies` folder.  
Place the `GamemodeManager.dll` in your usual plugins folder.  
Done!

---

## Config

```yaml
[GamemodeManager]
{
# Custom Gamemode Path, leave empty for default
customGamemodePath: ''
}
```

---

## For Developers:
In order to create gamemodes, please download and reference the `CustomGamemode.dll` within your gamemode's project.  
Do not refer to Synapse's way of plugin-creation, because GamemodeManager has its own way of determining and loading a gamemode.  
However, you may refer to Synapse's other functionalities. This includes subscribing events, working with Synapse-Types, etc...  
<br><br>
Your Gamemode's class needs to have a `Gamemode` Attribute, with values other than the default values,  
and has to implement the `IGamemode` interface,  both from the referenced `CustomGamemode.dll`.  

Quick example with the well-known Peanut Infection:
```cs
using CustomGamemode;
using Synapse.Api.Events.SynapseEventArguments;

namespace PeanutInfection
{
    [Gamemode(Name = "PeanutInfection", Author = "AlmightyLks", GitHubRepo = "https://github.com/AlmightyLks/SomeRepo",  Version = "1.0.0.0")]
    public class PeanutInfection : IGamemode
    {
        public void Start()
        {
            Synapse.Api.Events.EventHandler.Get.Round.RoundStartEvent += Round_RoundStartEvent;
            Synapse.Api.Events.EventHandler.Get.Player.PlayerDeathEvent += Player_PlayerDeathEvent;

            SynapseController.Server.Logger.Info("PeanutInfection Started");
        }
        public void End()
        {
            Synapse.Api.Events.EventHandler.Get.Round.RoundStartEvent -= Round_RoundStartEvent;
            Synapse.Api.Events.EventHandler.Get.Player.PlayerDeathEvent -= Player_PlayerDeathEvent;

            SynapseController.Server.Logger.Info("PeanutInfection Ended");
        }
        private void Round_RoundStartEvent()
        {
            SynapseController.Server.Map.Round.RoundLock = true;

            Timing.CallDelayed(0.1f, () =>
            {
                foreach (var player in SynapseController.Server.Players)
                {
                    player.ChangeRoleAtPosition(RoleType.ClassD);
                }
                SynapseController.Server.Players[UnityEngine.Random.Range(0, (SynapseController.Server.Players.Count - 1))].ChangeRoleAtPosition(RoleType.Scp173);
                SynapseController.Server.Players[UnityEngine.Random.Range(0, (SynapseController.Server.Players.Count - 1))].ChangeRoleAtPosition(RoleType.Scp173);
                SynapseController.Server.Players[UnityEngine.Random.Range(0, (SynapseController.Server.Players.Count - 1))].ChangeRoleAtPosition(RoleType.Scp173);

                SynapseController.Server.Map.Round.RoundLock = false;
            });
        }
        private void Player_PlayerDeathEvent(PlayerDeathEventArgs ev)
        {
            if (ev.Victim.RoleID != 0)
                ev.Victim.ChangeRoleAtPosition(RoleType.Scp173);
        }
    }
}

```

**Important Note:**  
First off: Be sure to fill out the `Gamemode` attribute properly, at the very least the `Name`, which refers to the Gamemode's name.  
GamemodeManager will refer to the Gamemode via that name.  
Also, please don't use Spaces in that name, use any common type of naming-convention such as snake_case, PascalCase or camelCase.  

Secondly, please be sure to clean up your event on End.  
GamemodeManager is not responsible for each gamemode's junk and can't and thus won't do it for you.  

Also worth mentioning, (yet) gamemodes do not have access to their own configs.  
This is planned to be realized in the future.
