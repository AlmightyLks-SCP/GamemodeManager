using CustomGamemode;
using GamemodeManager.Helper;
using Newtonsoft.Json;
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
        internal Dictionary<string, string> GamemodeConfigs { get; set; }

        private string _gamemodeDirectory;
        private string _gamemodeConfigDirectory;

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

        private void CreateDefaultConfigs(Assembly assembly)
        {
            //Every single Type within Assembly
            foreach (var type in assembly.GetTypes())
            {
                //Every single field within each Type
                foreach (var field in type.GetFields(BindingFlags.Public))
                {
                    //Only fields with custom attribute
                    var cfgAttr = field.GetCustomAttribute<GamemodeConfig>();
                    if (cfgAttr is null)
                        continue;

                    //Don't question why it's not 'field.FieldType', supposedly Unity likes to call things MonoField then.
                    //The field's type
                    Type cfgType = FieldInfo.GetFieldFromHandle(field.FieldHandle).FieldType;

                    //The field's value
                    object typeObj = Activator.CreateInstance(cfgType);
                }
            }
        }
        //Ignore as of yet.
        private void LoadConfig(Assembly gamemode)
        {
            if (!File.Exists(Path.Combine(_gamemodeConfigDirectory, "GamemodeConfig.json")))
                File.WriteAllText(Path.Combine(_gamemodeConfigDirectory, "GamemodeConfig.json"), JsonConvert.SerializeObject(GamemodeConfigs));
            Dictionary<string, string> jsonConfig = new Dictionary<string, string>();

            string cfgFile = File.ReadAllText(Path.Combine(_gamemodeConfigDirectory, "GamemodeConfig.json"));
            jsonConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(cfgFile);

            SynapseController.Server.Logger.Info(File.ReadAllText(Path.Combine(_gamemodeConfigDirectory, "GamemodeConfig.json")));
            SynapseController.Server.Logger.Info("------------------");
            SynapseController.Server.Logger.Info(JsonConvert.SerializeObject(jsonConfig));
        }
    }
}
