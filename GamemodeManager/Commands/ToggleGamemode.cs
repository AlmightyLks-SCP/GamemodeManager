using Synapse.Command;
using SynapseGamemode;
using System;
using System.Linq;

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

            SynapseController.Server.Logger.Info(args.Length.ToString());
            SynapseController.Server.Logger.Info(String.Join(" ", args));

            if (args.Length < 2)
            {
                result.State = CommandResultState.Error;
                result.Message = "Invalid input.";
                return result;
            }
            
            (IGamemode Gamemode, Gamemode Info) gamemode;

            gamemode = GMM.GamemodeManager.LoadedGamemodes.FirstOrDefault((_) => _.Info.Name.ToLower() == args[1].ToLower());

            if(gamemode == default((IGamemode Gamemode, Gamemode Info)))
            {

                result.State = CommandResultState.Error;
                result.Message = $"{gamemode.Info.Name} - Gamemode not found";
                return result;
            }

            try
            {
                switch (args[0])
                {
                    case "start":
                        gamemode.Gamemode.Start();

                        result.State = CommandResultState.Ok;
                        result.Message = $"{gamemode.Info.Name} started";
                        break;
                    case "end":
                        gamemode.Gamemode.End();

                        result.State = CommandResultState.Ok;
                        result.Message = $"{gamemode.Info.Name} ended";
                        break;
                    default:
                        result.State = CommandResultState.Error;
                        result.Message = "Invalid input.";
                        break;
                }
            }
            catch (Exception e)
            {
                result.State = CommandResultState.Error;
                result.Message = $"Error {args[0]}ing gamemode {args[1]}.";
                SynapseController.Server.Logger.Info($"Error {args[0]}ing gamemode {args[1]}.");
                SynapseController.Server.Logger.Info(e.StackTrace);
            }

            return result;
        }
    }
}
