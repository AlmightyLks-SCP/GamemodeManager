using Synapse.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamemodeManager.Commands
{
    [CommandInformation(
        Name = "gmm",
        Aliases = new[] { "gamemode", "gamemodemanager" },
        Description = "Toggle gamemodes",
        Permission = "GamemodeManager.Manage",
        Platforms = new Platform[] { Platform.RemoteAdmin },
        Usage = "\"gmm start [GamemodeName]\" or \"gmm end [GamemodeName]\""
        )]
    public class ToggleGamemode : ISynapseCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            var result = new CommandResult();
            var args = context.Arguments.ToArray();

            if (args.Length < 3)
            {
                result.State = CommandResultState.Error;
                result.Message = "Invalid input.";
                return result;
            }

            try
            {
                if (args[1] == "start")
                {
                    var gamemode = GMM.GamemodeManager.LoadedGamemodes.FirstOrDefault((_) => _.Info.Name.ToLower() == args[2].ToLower());
                    gamemode.Gamemode?.Start();

                    result.State = CommandResultState.Ok;
                    result.Message = $"{args[2]} started";
                }
                else if (args[1] == "end")
                {
                    var gamemode = GMM.GamemodeManager.LoadedGamemodes.FirstOrDefault((_) => _.Info.Name.ToLower() == args[2].ToLower());
                    gamemode.Gamemode?.End();

                    result.State = CommandResultState.Ok;
                    result.Message = $"{args[2]} ended";
                }
                else
                {
                    result.State = CommandResultState.Error;
                    result.Message = "Invalid input.";
                }
            }
            catch (Exception e)
            {
                result.State = CommandResultState.Error;
                result.Message = $"Error {args[1]}ing gamemode {args[2]}.";
                SynapseController.Server.Logger.Info($"Error {args[1]}ing gamemode {args[2]}.");
                SynapseController.Server.Logger.Info(e.StackTrace);
            }

            return result;
        }
    }
}
