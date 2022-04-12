using cards.Data.Game.Decks;

namespace cards.Data.Game;

public interface IGameService
{
    /// <summary>
    /// The name of the game
    /// </summary>
    /// <returns>Title of the game</returns>
    public string GetTitle();
    
    /// <summary>
    /// A description about the game
    /// </summary>
    /// <returns>A detailed description about the rules of the game</returns>
    public string GetDescription();

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
    /// <param name="card">The card played</param>
    public void Play(int id, ICard card);

    /// <summary>
    /// A list of extra play options
    /// </summary>
    /// <returns></returns>
    public List<IGameFeature> GetExtraOptions();

}