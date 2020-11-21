# GamemodeManager

---

## Installation:
Download the `CustomGamemode.dll` and place it within your `Synapse\dependencies` folder.  
Place the `GamemodeManager.dll` in your usual plugins folder.  
Either your configured path or the default Gamemode folder will provide a place for your plugins to be stored and loaded.  
Done!

---

## Config

```yaml
[GamemodeManager]
{
# Custom Gamemode Path, leave empty for default
customGamemodePath: ''
# End all Gamemodes on round end
autoGamemodeEnd: true
}
```

---

## For Developers:
In order to create gamemodes, please download and reference the `CustomGamemode.dll` within your gamemode's project.  
Do not refer to Synapse's way of plugin-creation, because GamemodeManager has its own way of determining and loading a gamemode.  
However, you may refer to Synapse's other functionalities. This includes subscribing events, working with Synapse-Types, etc...  
<br><br>
Your Gamemode's class needs to implement the `IGamemode` interface, from the referenced `CustomGamemode.dll`.  
That's it.  

Quick example with the well-known Gamemode [Peanut Infection](https://github.com/AlmightyLks/PeanutInfection).

**Important Note:**  
First off: Be sure to fill out the implemented properties from `IGamemode` properly, at the very least the `Name`, which refers to the Gamemode's name.  
GamemodeManager will refer to the Gamemode via that name.  
Also, please don't use Spaces in that name, use any common type of naming-convention such as snake_case, PascalCase or camelCase.  

Secondly, please be sure to clean up your event on End.  
GamemodeManager is not responsible for each gamemode's junk and can't and thus won't do it for you.  

Also worth mentioning, (yet) gamemodes do not have access to their own configs.  
This is planned to be realized in the future.
