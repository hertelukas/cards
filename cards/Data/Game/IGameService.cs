using cards.Data.Game.Decks;
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
    /// The player that has to play now
    /// </summary>
    /// <returns>The id of that player</returns>
    public int GetCurrentPlayer();

    /// <summary>
    /// Current player takes one card from the top
    /// </summary>
    /// <returns>The new card</returns>
    public ICard TakeCard();

    /// <summary>
    /// Get the hand of a player
    /// </summary>
    /// <param name="id">The id of the player</param>
    /// <returns>A collection of all the cards</returns>
    public ICollection<ICard> GetHand(int id);

    /// <summary>
    /// Get all playable cards of a player
    /// </summary>
    /// <param name="id">The id of the player</param>
    /// <returns>A collection of all the cards that are playable</returns>
    public ICollection<ICard> GetPlayableCards(int id);

    /// <summary>
    /// Checks whether a card can be played
    /// </summary>
    /// <param name="card">The card that should be played</param>
    /// <returns>True, if it can be played</returns>
    public bool IsPlayable(ICard card);

    /// <summary>
    /// Shuffles all remaining cards in the deck
    /// </summary>
    /// <returns>A collection of all cards in the deck</returns>
    public ICollection<ICard> Shuffle();

    /// <summary>
    /// The last played card
    /// </summary>
    /// <returns>The last played card</returns>
    public ICard GetLastPlayedCard();

    /// <summary>
    /// Play a card
    /// </summary>
    /// <param name="id">The id of the player who tries to play</param>
    /// <param name="cardIndex">The index of the card that will be played</param>
    public void Play(int id, int cardIndex);

    /// <summary>
    /// A list of extra play options
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IGameFeature> GetExtraOptions();

    /// <summary>
    /// Next players turn
    /// </summary>
    public void NextPlayer();

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