using Synapse.Api.Plugin;
using CustomGamemode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using GamemodeManager.EventHandler;

namespace GamemodeManager
{
    [PluginInformation(
        Author = "AlmightyLks",
        Description = "A Gamemode Manager for Synapse",
        Name = "GamemodeManager",
        SynapseMajor = 2,
        SynapseMinor = 1,
        SynapsePatch = 0,
        Version = "1.0.0"
        )]
    public class GMM : AbstractPlugin
    {
        [Config(section = "GamemodeManager")]
        public static PluginConfig Config;

        internal static GamemodeLoader GamemodeLoader { get; private set; }
        private GMMEventHandler _gmmEventHandler;

        public override void Load()
        {
            var watch = new Stopwatch();

            SynapseController.Server.Logger.Info($"<{Information.Name}> loading...");

            GamemodeLoader.LoadedGamemodes = new List<IGamemode>();
            GamemodeLoader.NextRoundGamemodes = new List<string>();

            if (Config.CustomGamemodePath == string.Empty)
                GamemodeLoader = new GamemodeLoader(Path.Combine(PluginDirectory, "Gamemodes"));
            else
                GamemodeLoader = new GamemodeLoader(Config.CustomGamemodePath);

            _gmmEventHandler = new GMMEventHandler(GamemodeLoader);

            watch.Start();

            GamemodeLoader.LoadGamemodes();

            watch.Stop();

            SynapseController.Server.Logger.Info($"<{Information.Name}> loaded {GamemodeLoader.LoadedGamemodes.Count} Gamemodes within {watch.Elapsed.TotalMilliseconds} ms!");

            Synapse.Api.Events.EventHandler.Get.Round.RoundStartEvent += _gmmEventHandler.Round_RoundStartEvent;
            Synapse.Api.Events.EventHandler.Get.Round.RoundEndEvent += _gmmEventHandler.Round_RoundEndEvent;
            Synapse.Api.Events.EventHandler.Get.Round.RoundRestartEvent += _gmmEventHandler.Round_RoundRestartEvent;
        }

    }
}
