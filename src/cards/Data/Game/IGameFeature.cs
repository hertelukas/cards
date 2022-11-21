namespace cards.Data.Game;

public interface IGameFeature
{
    public string Name { get; }

    public bool IsExecutable(int player);
    public bool Execute(int player);
}