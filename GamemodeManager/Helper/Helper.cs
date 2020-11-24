using CustomGamemode;
using System.Linq;
using System.Reflection;

namespace GamemodeManager.Helper
{
    public static class Helper
    {
        public static string ToInfoString(this IGamemode gamemode)
            => $"Name: {gamemode.Name}, Author: {gamemode.Author}, Version: {gamemode.Version}, GitHub: {gamemode.GitHubRepo}";
    }
}
