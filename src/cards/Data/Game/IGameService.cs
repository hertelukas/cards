using cards.Data.Game.Implementations;

namespace cards.Data.Game;

public interface IGameService
{
    public static string GetTitle(GameEnum gameEnum)
    {
        return gameEnum switch
        {
            GameEnum.CrazyEights => CrazyEights.GetTitle(),
            GameEnum.CrazyEightsVariation => CrazyEightsVariation.GetTitle(),
            _ => throw new ArgumentOutOfRangeException(nameof(gameEnum), gameEnum, null)
        };
    }

    public static string GetDescription(GameEnum gameEnum)
    {
        return gameEnum switch
        {
            GameEnum.CrazyEights => CrazyEights.GetDescription(),
            GameEnum.CrazyEightsVariation => CrazyEightsVariation.GetDescription(),
            _ => throw new ArgumentOutOfRangeException(nameof(gameEnum), gameEnum, null)
        };
    }

    /// <summary>
    /// Setup a new game
    /// </summary>
    /// <param name="players">The amount of players in that game</param>
    public void Initialize(int players);

    /// <summary>
    /// Get the winner of the current game
    /// </summary>
    /// <returns>The id of the winner, -1 if no winner exists</returns>
    public int GetWinner();

    /// <summary>
    /// Calculate points
    /// </summary>
    /// <returns>The points for this round for every user</returns>
    public List<int> CalcPoints();

    /// <summary>
    /// Meaning of points
    /// </summary>
    /// <returns>Whether having a lot of points is good</returns>
    public bool PointsAreGood();

    /// <summary>
    /// Play a card
    /// </summary>
    /// <param name="id">The id of the player who tries to play</param>
    /// <param name="cardIndex">The index of the card that will be played</param>
    public void Play(int id, int cardIndex);

    /// <summary>
    /// Executes a feature, does nothing if fails
    /// </summary>
    /// <param name="id">The id of the player who tries to execute the feature</param>
    /// <param name="featureId">The id of the feature</param>
    public void ExecuteFeature(int id, int featureId);

    /// <summary>
    /// Representation of the current game, for every user
    /// </summary>
    /// <returns>A list of game data with all needed information for a player</returns>
    public List<GameData> GetGameData();
}