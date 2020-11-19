using Synapse.Api.Plugin;
using CustomGamemode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

namespace GamemodeManager
{
    [PluginInformation(
        Author = "AlmightyLks",
        Description = "A Gamemode Manager for Synapse",
        Name = "GamemodeManager",
        SynapseMajor = 2,
        SynapseMinor = 0,
        SynapsePatch = 0,
        Version = "1.0.0"
        )]
    public class GMM : AbstractPlugin
    {
        [Config(section = "GamemodeManager")]
        public static PluginConfig Config;

        internal static GMM GamemodeManager { get; private set; }
        internal List<(IGamemode Gamemode, Gamemode Info)> LoadedGamemodes { get; set; }
        internal List<string> NextRoundGamemodes { get; set; }
        private string gamemodeDirectory;

        public override void Load()
        {
            var watch = new Stopwatch();
            watch.Start();

            SynapseController.Server.Logger.Info($"<{Information.Name}> loading...");

            LoadedGamemodes = new List<(IGamemode Gamemode, Gamemode Info)>();
            NextRoundGamemodes= new List<string>();
            GamemodeManager = this;

            if (Config.CustomGamemodePath == string.Empty)
                gamemodeDirectory = Path.Combine(PluginDirectory, "Gamemodes");
            else
                gamemodeDirectory = Config.CustomGamemodePath;

            LoadGamemodes();

            watch.Stop();

            SynapseController.Server.Logger.Info($"<{Information.Name}> loaded {LoadedGamemodes.Count} Gamemodes within {watch.Elapsed.TotalMilliseconds} ms!");

            Synapse.Api.Events.EventHandler.Get.Round.RoundStartEvent += Round_RoundStartEvent;
        
        }

        private void Round_RoundStartEvent()
        {
            foreach (var modeName in NextRoundGamemodes)
            {
                var gamemode = GMM.GamemodeManager.LoadedGamemodes.FirstOrDefault((_) => _.Info.Name.ToLower() == modeName.ToLower());

                if (gamemode == default((IGamemode Gamemode, Gamemode Info)))
                    continue;

                gamemode.Gamemode.Start();
            }
        }

        //[Credits] Inspired by Synapse.
        public void LoadGamemodes()
        {
            if (!Directory.Exists(gamemodeDirectory))
                Directory.CreateDirectory(gamemodeDirectory);

            var pluginPaths = Directory.GetFiles(gamemodeDirectory, "*.dll").ToList();

            var dictionary = new Dictionary<Gamemode, (Type Gamemode, List<Type> Types)>();

            foreach (var pluginPath in pluginPaths)
            {
                try
                {
                    var pluginAssembly = Assembly.Load(File.ReadAllBytes(pluginPath));

                    foreach (var type in pluginAssembly.GetTypes())
                    {
                        if (!typeof(IGamemode).IsAssignableFrom(type))
                            continue;

                        var gamemodeInfo = type.GetCustomAttribute<Gamemode>();

                        if (gamemodeInfo is null)
                        {
                            SynapseController.Server.Logger.Info($"The File {pluginAssembly.GetName().Name} has a class which implements IGamemode without the Gamemode Attribute ... Gamemode not loaded.");
                            continue;
                        }

                        if (gamemodeInfo == default(Gamemode))
                        {
                            SynapseController.Server.Logger.Info($"The File {pluginAssembly.GetName().Name} has a class which implements IGamemode but does not have custom Gamemode Attribute ... Gamemode not loaded.");
                            continue;
                        }

                        var allTypes = pluginAssembly.GetTypes().ToList();
                        allTypes.Remove(type);
                        dictionary.Add(gamemodeInfo, (type, allTypes));
                        break;
                    }
                }
                catch (Exception e)
                {
                    SynapseController.Server.Logger.Info($"{Path.GetFileName(pluginPath)} failed to load.\n{e.StackTrace}");
                }
            }

            foreach (var infoTypePair in dictionary)
            {
                try
                {
                    SynapseController.Server.Logger.Info($"{infoTypePair.Key.Name} will now be activated!");

                    IGamemode gamemode = (IGamemode)Activator.CreateInstance(infoTypePair.Value.Gamemode);

                    LoadedGamemodes.Add((gamemode, infoTypePair.Key));
                }
                catch (Exception e)
                {
                    SynapseController.Server.Logger.Error($"Instantiating of {infoTypePair.Value.Gamemode.Assembly.GetName().Name} failed!\n{e}");
                }
            }

            foreach (var gamemode in LoadedGamemodes)
                SynapseController.Server.Logger.Info(gamemode.Info.ToString());
        }
    }
}
