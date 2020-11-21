namespace CustomGamemode
{
    public interface IGamemode
    {
        void Start();
        void End();
        string Name { get; set; }
        string Author { get; set; }
        string GitHubRepo { get; set; }
        string Version { get; set; }
    }
}
