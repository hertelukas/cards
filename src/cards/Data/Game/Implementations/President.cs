namespace cards.Data.Game.Implementations;

public class President : IGameService
{
    private readonly ILogger<President> _logger;

    public President()
    {
        _logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger<President>();
    }

    public static string GetTitle()
    {
        return "President";
    }

    public static string GetDescription()
    {
        //TODO
        return "TODO";
    }

    public void Initialize(int players)
    {
        throw new NotImplementedException();
    }

    public int GetWinner()
    {
        throw new NotImplementedException();
    }

    public List<int> CalcPoints()
    {
        throw new NotImplementedException();
    }

    public bool PointsAreGood()
    {
        throw new NotImplementedException();
    }

    public void Play(int id, int cardIndex)
    {
        throw new NotImplementedException();
    }

    public void ExecuteFeature(int id, int featureId)
    {
        throw new NotImplementedException();
    }

    public List<GameData> GetGameData()
    {
        throw new NotImplementedException();
    }
}