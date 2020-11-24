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
        internal List<(object Gamemode, MemberInfo ConfigMember)> LoadedGamemodes { get; set; }
        internal List<string> NextRoundGamemodes { get; set; }
        internal Dictionary<string, object> JsonConfigs { get; set; }

        private string _gamemodeDirectory;
        private string _gamemodeConfigDirectory;

        public GamemodeLoader(string gamemodeDirectory, string gamemodeConfigDirectory)
        {
            _gamemodeDirectory = gamemodeDirectory;
            _gamemodeConfigDirectory = gamemodeConfigDirectory;
            LoadedGamemodes = new List<(object Gamemode, MemberInfo ConfigMember)>();
            NextRoundGamemodes = new List<string>();
            JsonConfigs = new Dictionary<string, object>();
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
                    var gamemodeTypes = gamemodeAssembly.GetTypes().ToList();

                    foreach (var type in gamemodeTypes)
                    {
                        if (!typeof(IGamemode).IsAssignableFrom(type))
                            continue;

                        gamemodeTypes.Remove(type);
                        gamemodeDict.Add(type, gamemodeTypes);
                        break;
                    }
                }
                catch (Exception e)
                {
                    SynapseController.Server.Logger.Error($"{Path.GetFileName(gamemodePath)} failed to load.\n{e.StackTrace}");
                }
            }

            foreach (var infoTypePair in gamemodeDict)
            {
                try
                {
                    SynapseController.Server.Logger.Info($"{infoTypePair.Key.Name} will now be activated!");

                    object gamemode = Activator.CreateInstance(infoTypePair.Key);

                    LoadedGamemodes.Add((gamemode, null));

                    List<Type> types = infoTypePair.Value;
                    types.Add(gamemode.GetType());

                    FetchDefaultConfigs(types, gamemode);
                }
                catch (Exception e)
                {
                    SynapseController.Server.Logger.Error($"Instantiating {infoTypePair.Key.Assembly.GetName().Name} failed!\n{e}");
                }
            }

            foreach (var gamemode in LoadedGamemodes)
                SynapseController.Server.Logger.Info(((IGamemode)gamemode.Gamemode).ToInfoString());

            LoadConfig();
            ApplyConfigs();

            SynapseController.Server.Logger.Error($"JsonConfigs: " + JsonConfigs.Count);
        }

        private void FetchDefaultConfigs(List<Type> types, object gamemode)
        {
            SynapseController.Server.Logger.Info($"Am Fetching...");
            try
            {
                //Every single Type within Assembly
                foreach (var type in types)
                {
                    SynapseController.Server.Logger.Error($"-------------------------------> 1");
                    //Every single field within each Type
                    foreach (var field in type.GetFields())
                    {
                        SynapseController.Server.Logger.Error($"-------------------------------> field 2 | {type.Name}");
                        //Only fields with custom attribute

                        if (!Attribute.IsDefined(field, typeof(GamemodeConfig)))
                            continue;

                        SynapseController.Server.Logger.Error($"-------------------------------> field 3");

                        //Don't question why it's not 'field.FieldType', supposedly Unity likes to call things MonoField then.
                        //The field's type
                        Type cfgType = FieldInfo.GetFieldFromHandle(field.FieldHandle).FieldType;

                        for (int i = 0; i < LoadedGamemodes.Count; i++)
                        {
                            if (LoadedGamemodes[i].Gamemode == gamemode)
                                LoadedGamemodes[i] = (gamemode, field);
                        }

                        //The field's value
                        object typeObj = Activator.CreateInstance(cfgType);
                        JsonConfigs.Add(((IGamemode)gamemode).Name, typeObj);
                        SynapseController.Server.Logger.Error($"Heh: {JsonConfigs.Count} <---------------------------");
                        return;
                    }

                    //Every single property within each Type
                    foreach (var property in type.GetProperties())
                    {
                        SynapseController.Server.Logger.Error($"-------------------------------> property 2 | {type.Name}");
                        //Only property with custom attribute

                        if (!Attribute.IsDefined(property, typeof(GamemodeConfig)))
                            continue;

                        SynapseController.Server.Logger.Error($"-------------------------------> property 3");

                        //The property's type
                        Type cfgType = property.PropertyType;

                        for (int i = 0; i < LoadedGamemodes.Count; i++)
                        {
                            if (LoadedGamemodes[i].Gamemode == gamemode)
                                LoadedGamemodes[i] = (gamemode, property);
                        }

                        //The property's value
                        object typeObj = Activator.CreateInstance(cfgType);
                        JsonConfigs.Add(((IGamemode)gamemode).Name, typeObj);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                SynapseController.Server.Logger.Error($"Fetching configs failed!\n{e}");
            }
        }
        private void LoadConfig()
        {
            SynapseController.Server.Logger.Info($"Am Loading...");
            try
            {
                if (!File.Exists(Path.Combine(_gamemodeConfigDirectory, "GamemodeConfig.json")))
                {
                    File.WriteAllText(Path.Combine(_gamemodeConfigDirectory, "GamemodeConfig.json"), JsonConvert.SerializeObject(JsonConfigs));
                    return;
                }

                Dictionary<string, object> jsonConfig = new Dictionary<string, object>();

                string cfgFile = File.ReadAllText(Path.Combine(_gamemodeConfigDirectory, "GamemodeConfig.json"));

                jsonConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(cfgFile);

                foreach (var cfgFileEntry in jsonConfig)
                {
                    if (JsonConfigs.ContainsKey(cfgFileEntry.Key))
                        JsonConfigs[cfgFileEntry.Key] = cfgFileEntry.Value;
                }

                foreach (var _ in JsonConfigs)
                    SynapseController.Server.Logger.Info($"{_.Key} Doneeeeeeeeeeeeeeeeee!");

                File.WriteAllText(Path.Combine(_gamemodeConfigDirectory, "GamemodeConfig.json"), JsonConvert.SerializeObject(jsonConfig));
            }
            catch (Exception e)
            {
                SynapseController.Server.Logger.Error($"Loading configs failed!\n{e}");
            }
        }
        private void ApplyConfigs()
        {
            SynapseController.Server.Logger.Info($"Am Applying...");
            foreach (var cfg in JsonConfigs)
            {
                try
                {
                    var smth = LoadedGamemodes.FirstOrDefault((_) => ((IGamemode)_.Gamemode).Name == cfg.Key);

                    if (smth == default((object, MemberInfo)))
                        continue;

                    if (smth.ConfigMember == null)
                        continue;

                    if (smth.ConfigMember is PropertyInfo prop)
                    {
                        SynapseController.Server.Logger.Error($"I am in Property");
                        prop.SetValue(smth.Gamemode, cfg.Value, null); //Can't convert type JObject to prop.PropertyType
                    }
                    else if (smth.ConfigMember is FieldInfo field)
                    {
                        SynapseController.Server.Logger.Error($"I am in Field");
                        field.SetValue(smth.Gamemode, cfg.Value); //Can't convert type JObject to field.FieldType
                    }
                }
                catch (Exception e)
                {
                    SynapseController.Server.Logger.Error($"Applying configs for {cfg.Key} failed!\n{e}");
                }
            }
        }
    }
}
