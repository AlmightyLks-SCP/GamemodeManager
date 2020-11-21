using CustomGamemode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamemodeManager.EventHandler
{
    internal class GMMEventHandler
    {
        private GamemodeLoader gamemodeLoader;

        public GMMEventHandler(GamemodeLoader gamemodeLoader)
        {
            this.gamemodeLoader = gamemodeLoader;
        }

        public void Round_RoundRestartEvent()
        {
            if (GMM.Config.AutoGamemodeEnd)
                foreach (var gamemode in gamemodeLoader.LoadedGamemodes)
                    gamemode.End();
        }
        public void Round_RoundEndEvent()
        {
            if (GMM.Config.AutoGamemodeEnd)
                foreach (var gamemode in gamemodeLoader.LoadedGamemodes)
                    gamemode.End();
        }
        public void Round_RoundStartEvent()
        {
            foreach (var modeName in gamemodeLoader.NextRoundGamemodes)
            {
                var gamemode = gamemodeLoader.LoadedGamemodes.FirstOrDefault((_) => _.Name.ToLower() == modeName.ToLower());

                if (gamemode == default(IGamemode))
                    continue;

                gamemode.Start();
            }
        }
    }
}
