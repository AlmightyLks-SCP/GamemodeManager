using CustomGamemode;
using GamemodeManager.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GamemodeManager
{
    internal class GamemodeLoader
    {
        internal List<object> LoadedGamemodes { get; set; }
        internal List<string> NextRoundGamemodes { get; set; }

        private string _gamemodeDirectory;

        public GamemodeLoader(string gamemodeDirectory)
        {
            _gamemodeDirectory = gamemodeDirectory;
            LoadedGamemodes = new List<object>();
            NextRoundGamemodes = new List<string>();
        }

        //[Credits] Assembly-Loading inspired by Synapse.
        public void LoadGamemodes()
        {
            if (!Directory.Exists(_gamemodeDirectory))
                Directory.CreateDirectory(_gamemodeDirectory);

            var gamemodePaths = Directory.GetFiles(_gamemodeDirectory, "*.dll").ToList();

            var gamemodeDict = new Dictionary<Type, List<Type>>();

            foreach (var gamemodePath in gamemodePaths)
            {
                try
                {
                    var gamemodeAssembly = Assembly.Load(File.ReadAllBytes(gamemodePath));

                    foreach (var type in gamemodeAssembly.GetTypes())
                    {
                        if (!typeof(IGamemode).IsAssignableFrom(type))
                            continue;

                        var allTypes = gamemodeAssembly.GetTypes().ToList();
                        allTypes.Remove(type);
                        gamemodeDict.Add(type, allTypes);
                        break;
                    }
                }
                catch (Exception e)
                {
                    SynapseController.Server.Logger.Info($"{Path.GetFileName(gamemodePath)} failed to load.\n{e.StackTrace}");
                }
            }

            foreach (var infoTypePair in gamemodeDict)
            {
                try
                {
                    SynapseController.Server.Logger.Info($"{infoTypePair.Key.Name} will now be activated!");

                    object gamemode = Activator.CreateInstance(infoTypePair.Key);

                    LoadedGamemodes.Add(gamemode);
                }
                catch (Exception e)
                {
                    SynapseController.Server.Logger.Error($"Instantiating {infoTypePair.Key.Assembly.GetName().Name} failed!\n{e}");
                }
            }

            foreach (var gamemode in LoadedGamemodes)
                SynapseController.Server.Logger.Info(((IGamemode)gamemode).ToInfoString());
        }
    }
}
