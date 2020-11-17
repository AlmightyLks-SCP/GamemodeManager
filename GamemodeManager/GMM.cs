using Synapse.Api.Plugin;
using SynapseGamemode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GamemodeManager
{
    [PluginInformation(
        Author = "AlmightyLks",
        Description = "A gamemode manager",
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

        private List<IGamemode> loadedGamemodes;
        private string gamemodeDirectory;
        public override void Load()
        {
            SynapseController.Server.Logger.Info($"<{Information.Name}> loading...");
            if (Config.CustomGamemodePath == string.Empty)
                gamemodeDirectory = Path.Combine(PluginDirectory, "Gamemodes");
            else
                gamemodeDirectory = Config.CustomGamemodePath;

            loadedGamemodes = new List<IGamemode>();
            LoadGamemodes();
            SynapseController.Server.Logger.Info($"<{Information.Name}> loaded!");
        }
        private void LoadGamemodes()
        {
            if (!Directory.Exists(gamemodeDirectory))
                Directory.CreateDirectory(gamemodeDirectory);

            var pluginPaths = Directory.GetFiles(gamemodeDirectory, "*.dll").ToList();

            var dictionary = new Dictionary<Gamemode, (Type Gamemode, List<Type> Types)>();

            foreach (var pluginPath in pluginPaths)
            {
                var pluginAssembly = Assembly.Load(File.ReadAllBytes(pluginPath));

                foreach (var type in pluginAssembly.GetTypes())
                {
                    if (!typeof(IGamemode).IsAssignableFrom(type))
                        continue;

                    var gamemodeInfo = type.GetCustomAttribute<Gamemode>();

                    if (gamemodeInfo is null)
                    {
                        SynapseController.Server.Logger.Info($"The File {pluginAssembly.GetName().Name} has a class which implements IGamemode without Gamemode Attribute ... Default Values will be applied.");
                        gamemodeInfo = new Gamemode();
                    }

                    var allTypes = pluginAssembly.GetTypes().ToList();
                    allTypes.Remove(type);
                    dictionary.Add(gamemodeInfo, (type, allTypes));
                    break;
                }
            }

            SynapseController.Server.Logger.Info("---------------------------");

            foreach (var smth in dictionary)
            {
                SynapseController.Server.Logger.Info($"Name: {smth.Key.Name}, Author: {smth.Key.Author},Name: {smth.Key.Version}, Name: {smth.Key.GitHubRepo}");
            }

            foreach (var gamemode in loadedGamemodes)
            {
                gamemode.Init();
                gamemode.Start();
                gamemode.ForceStop();
            }
        }
    }
}
