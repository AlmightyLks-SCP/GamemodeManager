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

        internal static GMM GamemodeManager { get; private set; }
        internal List<(IGamemode Gamemode, Gamemode Info)> LoadedGamemodes { get; set; }
        private string gamemodeDirectory;

        public override void Load()
        {
            SynapseController.Server.Logger.Info($"<{Information.Name}> loading...");

            GamemodeManager = this;

            if (Config.CustomGamemodePath == string.Empty)
                gamemodeDirectory = Path.Combine(PluginDirectory, "Gamemodes");
            else
                gamemodeDirectory = Config.CustomGamemodePath;

            LoadedGamemodes = new List<(IGamemode Gamemode, Gamemode Info)>();
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
                        SynapseController.Server.Logger.Info($"The File {pluginAssembly.GetName().Name} has a class which implements IGamemode without the Gamemode Attribute ... Gamemode not loaded.");
                        continue;
                    }

                    if(gamemodeInfo == default(Gamemode))
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
                    SynapseController.Server.Logger.Error($"Synapse-Controller: Activation of {infoTypePair.Value.Gamemode.Assembly.GetName().Name} failed!!\n{e}");
                }
            }

            SynapseController.Server.Logger.Info("---------------------------");

            foreach (var gamemode in LoadedGamemodes)
            {
                SynapseController.Server.Logger.Info(gamemode.Info.ToString());
                gamemode.Gamemode.Init();
            }
        }
    }
}
