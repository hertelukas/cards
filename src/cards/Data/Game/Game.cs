using cards.Data.Game.Implementations;

namespace cards.Data.Game;

public abstract class Game
{
    public abstract string Title { get; }

    public abstract string Description { get; }

    /// <summary>
    /// Meaning of points
    /// </summary>
    /// <returns>Whether having a lot of points is good</returns>
    public abstract bool PointsAreGood { get; }

    /// <summary>
    /// Setup a new game
    /// </summary>
    /// <param name="players">The amount of players in that game</param>
    public abstract void Initialize(int players);

    /// <summary>
    /// Determine the state of the game
    /// </summary>
    /// <returns>Whether the game is over</returns>
    public abstract bool IsOver();

    /// <summary>
    /// Calculate points
    /// </summary>
    /// <returns>The points for this round for every user</returns>
    public abstract List<int> CalcPoints();

    /// <summary>
    /// Gets passed to a next rounds game constructor
    /// </summary>
    /// <returns>Data that is needed in the next round</returns>
    public virtual IPersistentInformation GetPersistentInformation()
    {
        return new DummyInformation();
    }

    /// <summary>
    /// Play a card
    /// </summary>
    /// <param name="id">The id of the player who tries to play</param>
    /// <param name="cardIndex">The index of the card that will be played</param>
    public abstract void Play(int id, int cardIndex);

    /// <summary>
    /// Executes a feature, does nothing if fails
    /// </summary>
    /// <param name="id">The id of the player who tries to execute the feature</param>
    /// <param name="featureId">The id of the feature</param>
    public abstract void ExecuteFeature(int id, int featureId);

    /// <summary>
    /// Representation of the current game, for every user
    /// </summary>
    /// <returns>A list of game data with all needed information for a player</returns>
    public abstract List<GameData> GetGameData();
}