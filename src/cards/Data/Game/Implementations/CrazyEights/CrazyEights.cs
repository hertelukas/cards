using cards.Data.Enums.Card;
using cards.Data.Game.Decks;
using cards.Data.Game.Implementations.CrazyEights.GameFeatures;

namespace cards.Data.Game.Implementations.CrazyEights;

public class CrazyEights : Game
{
    private readonly ILogger<CrazyEights> _logger;

    private Queue<ICard> _deck;
    private Stack<ICard> _playedCards;

    public Action<List<ICard>> ShuffleService { get; set; }
    public List<ICard>[] PlayerCards;
    public Suit WishedColor = Suit.Hearts;
    public int CurrentPlayer;
    public bool HasPlayedEight;
    public bool HasTakenCard;

    public CrazyEights()
    {
        _logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger<CrazyEights>();
        PlayerCards = Array.Empty<List<ICard>>();
        _deck = new Queue<ICard>();
        _playedCards = new Stack<ICard>();

        ShuffleService = ShufflingStrategies.FisherYatesShuffle;
    }

    public override string Title => "Crazy Eights";

    public override string Description => "Five cards are dealt to each player (or seven in a two-player game). " +
            "The remaining cards of the deck are placed face down at the center of the table as the stock pile. The top card is then turned face up to start the game as the first card in the discard pile.<br>" +
            "<b>Players discard by matching rank or suit with the top card of the discard pile.</b> " +
            "They can also <b>play any 8 at any time, which allows them to declare the suit that the next player is to play;</b> that player must then follow the named suit or play another 8. " +
            "If a player is unable to play, that player draws cards from the stock pile. He can still play a card or skip his turn. " +
            "A player may draw from the stock pile at any time, even when holding one or more playable cards.<br>" +
            "<b>Points:</b> The game ends as soon as one player has emptied their hand. That player collects a payment from each opponent equal to the point score of the cards remaining in that opponent's hand. " +
            "8s score 50, court cards 10 and all other cards face value.<br>" +
            "<i>Source: Wikipedia</i>";

    public override bool PointsAreGood => true;

    public override void Initialize(int players)
    {
        _logger.LogInformation("Initializing Crazy Eights with {Players} players", players);
        PlayerCards = new List<ICard>[players];
        InitializeDeck();
        Shuffle();

        // Initialize player cards
        for (var i = 0; i < PlayerCards.Length; i++)
        {
            PlayerCards[i] = new List<ICard>();
        }

        var cardsPerPlayer = players <= 2 ? 7 : 5;
        // Give every player 5 cards
        foreach (var hand in PlayerCards)
        {
            for (var i = 0; i < cardsPerPlayer; i++)
            {
                hand.Add(TakeCard());
            }
        }

        // Take the top card
        _playedCards = new Stack<ICard>();
        _playedCards.Push(TakeCard());
    }

    public override bool IsOver() => GetWinner() >= 0;

    public override void Play(int id, int cardIndex)
    {
        if (!CanPlay(id, cardIndex))
        {
            return;
        }

        var card = PlayerCards[id][cardIndex];

        _logger.LogInformation("{PlayerId} is playing {Card}", id, card.ToString());

        PlayerCards[id].Remove(card);
        _playedCards.Push(card);

        HasTakenCard = false;

        // If no eight played, the game goes on
        if (((Poker)card).Value != Value.Eight)
        {
            NextPlayer();
        }
        else
        {
            HasPlayedEight = true;
        }
    }

    public override List<int> CalcPoints()
    {
        var winner = GetWinner();

        var sum = PlayerCards.Sum(h => h.Sum(c =>
            {
                return ((Poker)c).Value switch
                {
                    Value.Eight => 50,
                    Value.Jack => 10,
                    Value.Queen => 10,
                    Value.King => 10,
                    Value.Ace => 10,
                    _ => (int)((Poker)c).Value + 1
                };
            }
        ));

        var result = new List<int>();

        for (var i = 0; i < PlayerCards.Length; i++)
        {
            result.Add(i == winner ? sum : 0);
        }

        return result;
    }

    public ICollection<ICard> GetHand(int id)
    {
        return PlayerCards[id];
    }

    public override void ExecuteFeature(int id, int featureId)
    {
        _logger.LogDebug("Player {PlayerId} is trying to execute feature {FeatureIndex}", id, featureId);
        GetExtraOptions().ToList()[featureId].Execute(id);
    }

    public override List<GameData> GetGameData()
    {
        var result = new List<GameData>();

        for (var i = 0; i < PlayerCards.Length; i++)
        {
            var cards = PlayerCards[i].Select(card => card.ToHtmlString());
            var otherPlayersAmountOfCards = new List<int>();

            for (var j = 1; j < PlayerCards.Length; j++)
            {
                otherPlayersAmountOfCards.Add(PlayerCards[(i + j) % PlayerCards.Length].Count);
            }

            var topCard = GetLastPlayedCard().ToHtmlString();

            if (GetLastPlayedCard().Value == Value.Eight)
            {
                topCard += $": {Poker.SpanFromSuit(WishedColor)}";
            }

            var features = GetExtraOptions().Select(feature => feature.Name).ToList();
            var featuresEnabled = GetExtraOptions().Select(feature => feature.IsExecutable(i)).ToList();

            result.Add(new GameData(
                cards,
                otherPlayersAmountOfCards,
                (CurrentPlayer - i + PlayerCards.Length) % PlayerCards.Length,
                topCard,
                features,
                featuresEnabled
            ));
        }

        return result;
    }

    public virtual void NextPlayer()
    {
        CurrentPlayer = (CurrentPlayer + 1) % PlayerCards.Length;
        HasPlayedEight = false;
        HasTakenCard = false;
    }

    public ICard TakeCard()
    {
        var result = _deck.Dequeue();

        _logger.LogInformation("Taking {Card}", result.ToString());

        // Shuffle if the deck is now empty
        if (_deck.Count == 0)
        {
            var topCard = _playedCards.Pop();

            _deck = new Queue<ICard>();

            while (_playedCards.Count > 0)
            {
                _deck.Enqueue(_playedCards.Pop());
            }

            Shuffle();
            _playedCards.Push(topCard);
        }

        return result;
    }

    protected virtual bool IsPlayable(ICard card)
    {
        var topCard = GetLastPlayedCard();
        var playedCard = (Poker)card;

        _logger.LogDebug("Trying to play {PlayedCard} on top of {TopCard}", playedCard.ToString(), topCard.ToString());

        // If the last card is an 8, we have to look at the wish
        if (topCard.Value == Value.Eight)
        {
            return playedCard.Value == Value.Eight || playedCard.Suit == WishedColor;
        }

        return topCard.Value == playedCard.Value || topCard.Suit == playedCard.Suit ||
               playedCard.Value == Value.Eight;
    }

    // Checks if the card can be played and if the user owns the card
    protected bool CanPlay(int id, int cardIndex)
    {
        _logger.LogDebug("{PlayerId} is trying to play his {CardIndex}. card", id, cardIndex);
        // Check whether the player is playing
        if (CurrentPlayer != id)
        {
            return false;
        }

        // Check whether the player owns this card
        if (PlayerCards[id].Count < cardIndex)
        {
            return false;
        }

        var card = PlayerCards[id][cardIndex];

        // Check whether the card is playable
        return IsPlayable(card);
    }

    protected virtual IEnumerable<IGameFeature> GetExtraOptions()
    {
        var result = new List<IGameFeature>
        {
            new SortByValue(this),
            new SortBySuit(this),
            new ChooseSuitFeature(this, Suit.Clovers),
            new ChooseSuitFeature(this, Suit.Hearts),
            new ChooseSuitFeature(this, Suit.Diamonds),
            new ChooseSuitFeature(this, Suit.Peaks),
            new TakeCardFeature(this),
            new DoNothing(this)
        };

        return result;
    }

    private void InitializeDeck()
    {
        _deck = new Queue<ICard>();

        foreach (var value in Enum.GetValues<Value>())
        {
            foreach (var suit in Enum.GetValues<Suit>())
            {
                _deck.Enqueue(new Poker(value, suit));
            }
        }
    }

    private int GetWinner()
    {
        for (var i = 0; i < PlayerCards.Length; i++)
        {
            if (PlayerCards[i].Count == 0)
            {
                _logger.LogInformation("Player {Winner} won", i);
                return i;
            }
        }

        _logger.LogInformation("No winner determined");
        return -1;
    }

    private void Shuffle()
    {
        var deckAsList = _deck.ToList();

        ShuffleService.Invoke(deckAsList);

        _deck = new Queue<ICard>(deckAsList);
    }

    private Poker GetLastPlayedCard()
    {
        return (Poker)_playedCards.Peek();
    }
}