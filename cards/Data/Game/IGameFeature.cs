namespace cards.Data.Game;

public interface IGameFeature
{
    public string GetName();
    public bool IsExecutable(int player);
    public bool Execute(int player);
}