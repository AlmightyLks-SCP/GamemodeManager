using CustomGamemode;
using Synapse.Command;
using System;
using System.Diagnostics;
using System.Linq;

namespace GamemodeManager.Commands
{
    [CommandInformation(
        Name = "gmm",
        Aliases = new[] { "gamemode", "gamemodemanager" },
        Description = "Toggle gamemodes",
        Permission = "GamemodeManager.Manage",
        Platforms = new Platform[] { Platform.RemoteAdmin },
        Usage = "\"gmm reload\" / \"gmm list\" / \"gmm start [GamemodeName]\" / \"gmm end [GamemodeName]\" / \"gmm nextround [GamemodeName]\""
        )]
    public class ToggleGamemode : ISynapseCommand
    {
        public CommandResult Execute(CommandContext context)
        {
            (IGamemode Gamemode, Gamemode Info) gamemode;
            var result = new CommandResult();
            var args = context.Arguments.ToArray();

            if (args.Length < 1)
            {
                result.State = CommandResultState.Error;
                result.Message = "Invalid input.";
                return result;
            }

            try
            {
                switch (args[0])
                {
                    case "list":
                        {
                            if (GMM.GamemodeManager.LoadedGamemodes.Count != 0)
                            {
                                result.State = CommandResultState.Ok;
                                result.Message = String.Join(", ", GMM.GamemodeManager.LoadedGamemodes.Select((_) => _.Info.Name));
                            }
                            else
                            {
                                result.State = CommandResultState.Ok;
                                result.Message = "No gamemodes loaded.";
                            }
                            break;
                        }
                    case "reload":
                        {
                            Stopwatch watch = new Stopwatch();

                            GMM.GamemodeManager.LoadedGamemodes.Clear();

                            watch.Start();
                            GMM.GamemodeManager.LoadGamemodes();
                            watch.Stop();

                            result.State = CommandResultState.Ok;
                            result.Message = $"<{GMM.GamemodeManager.Information.Name}> loaded {GMM.GamemodeManager.LoadedGamemodes.Count} Gamemodes within {watch.Elapsed.TotalMilliseconds} ms!";

                            break;
                        }
                    case "start" when args.Length == 2:
                        {
                            gamemode = GMM.GamemodeManager.LoadedGamemodes.FirstOrDefault((_) => _.Info.Name.ToLower() == args[1].ToLower());

                            if (gamemode == default((IGamemode Gamemode, Gamemode Info)))
                            {
                                result.State = CommandResultState.Error;
                                result.Message = $"{args[1]} - Gamemode not found";
                                return result;
                            }

                            gamemode.Gamemode.Start();

                            result.State = CommandResultState.Ok;
                            result.Message = $"{gamemode.Info.Name} started";

                            break;
                        }
                    case "nextround" when args.Length == 2:
                        {
                            var gamemodeExists = GMM.GamemodeManager.LoadedGamemodes.Any((_) => _.Info.Name.ToLower() == args[1].ToLower());

                            if (!gamemodeExists)
                            {
                                result.State = CommandResultState.Error;
                                result.Message = $"{args[1]} - Gamemode not found";
                            }
                            else
                            {
                                GMM.GamemodeManager.NextRoundGamemodes.Add(args[1]);
                                result.State = CommandResultState.Ok;
                                result.Message = $"{args[1]} queued for next round";
                            }

                            break;
                        }
                    case "end" when args.Length == 2:
                        {
                            gamemode = GMM.GamemodeManager.LoadedGamemodes.FirstOrDefault((_) => _.Info.Name.ToLower() == args[1].ToLower());

                            if (gamemode == default((IGamemode Gamemode, Gamemode Info)))
                            {
                                result.State = CommandResultState.Error;
                                result.Message = $"{args[1]} - Gamemode not found";
                                return result;
                            }

                            gamemode.Gamemode.End();

                            result.State = CommandResultState.Ok;
                            result.Message = $"{gamemode.Info.Name} ended";
                            break;
                        }
                    default:
                        result.State = CommandResultState.Error;
                        result.Message = "Invalid input.";
                        break;
                }
            }
            catch (Exception e)
            {
                result.State = CommandResultState.Error;
                if (args.Length < 2)
                {
                    result.Message = $"Error {args[0]}ing gamemode {args[1]}.";
                    SynapseController.Server.Logger.Info($"Error {args[0]}ing gamemode {args[1]}.");
                    SynapseController.Server.Logger.Info(e.StackTrace);
                }
                else
                {
                    result.Message = $"Error {args[0]}ing.";
                    SynapseController.Server.Logger.Info($"Error {args[0]}ing.");
                    SynapseController.Server.Logger.Info(e.StackTrace);
                }
            }

            return result;
        }
    }
}
