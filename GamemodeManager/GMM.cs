using Synapse.Api.Plugin;
using System.IO;
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
        Version = "1.0.1"
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
            string gamemodePath = string.Empty;
            string gamemodeConfigPath = string.Empty;

            if (Config.CustomGamemodePath == string.Empty)
                gamemodePath = Path.Combine(PluginDirectory, "Gamemodes");
            else
                gamemodePath = Config.CustomGamemodePath;


            GamemodeLoader = new GamemodeLoader(gamemodePath, PluginDirectory);

            _gmmEventHandler = new GMMEventHandler(GamemodeLoader);

            watch.Start();

            GamemodeLoader.LoadGamemodes();

            watch.Stop();

            SynapseController.Server.Logger.Info($"<{Information.Name}> loaded {GamemodeLoader.LoadedGamemodes.Count} Gamemodes within {watch.Elapsed.TotalMilliseconds} ms!");

            Synapse.Api.Events.EventHandler.Get.Round.WaitingForPlayersEvent += _gmmEventHandler.Round_WaitingForPlayersEvent;
            Synapse.Api.Events.EventHandler.Get.Round.RoundEndEvent += _gmmEventHandler.Round_RoundEndEvent;
            Synapse.Api.Events.EventHandler.Get.Round.RoundRestartEvent += _gmmEventHandler.Round_RoundRestartEvent;
        }
    }
}
