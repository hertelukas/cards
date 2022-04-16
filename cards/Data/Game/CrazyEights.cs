using cards.Data.Game.Decks;

namespace cards.Data.Game;

public class CrazyEights : IGameService
{
    private readonly ILogger<CrazyEights> _logger;
    protected List<ICard>[] PlayerCards;
    private Queue<ICard> _deck;
    private Stack<ICard> _playedCards;
    protected int CurrentPlayer;
    private Poker.Suit _wishedColor = Poker.Suit.Hearts;
    protected bool HasPlayedEight;
    protected bool HasTakenCard;

    public CrazyEights()
    {
        _logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger<CrazyEights>();
        PlayerCards = Array.Empty<List<ICard>>();
        _deck = new Queue<ICard>();
        _playedCards = new Stack<ICard>();
    }

    public static string GetTitle()
    {
        return "Crazy Eights";
    }

    public static string GetDescription()
    {
        return
            "Five cards are dealt to each player (or seven in a two-player game). " +
            "The remaining cards of the deck are placed face down at the center of the table as the stock pile. The top card is then turned face up to start the game as the first card in the discard pile.<br>" +
            "<b>Players discard by matching rank or suit with the top card of the discard pile.</b> " +
            "They can also <b>play any 8 at any time, which allows them to declare the suit that the next player is to play;</b> that player must then follow the named suit or play another 8. " +
            "If a player is unable to play, that player draws cards from the stock pile. He can still play a card or skip his turn. " +
            "A player may draw from the stock pile at any time, even when holding one or more playable cards.<br>" +
            "<i>Source: Wikipedia</i>";
    }

    public void Initialize(int players)
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

    private void InitializeDeck()
    {
        _deck = new Queue<ICard>();

        foreach (var value in Enum.GetValues<Poker.Value>())
        {
            foreach (var suit in Enum.GetValues<Poker.Suit>())
            {
                _deck.Enqueue(new Poker(value, suit));
            }
        }
    }

    public int GetWinner()
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

    public int GetCurrentPlayer()
    {
        return CurrentPlayer;
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

    public ICollection<ICard> GetHand(int id)
    {
        return PlayerCards[id];
    }

    public ICollection<ICard> GetPlayableCards(int id)
    {
        var hand = GetHand(id);

        return hand
            .Where(IsPlayable)
            .ToList();
    }

    public bool IsPlayable(ICard card)
    {
        var topCard = (Poker) GetLastPlayedCard();
        var playedCard = (Poker) card;

        _logger.LogDebug("Trying to play {PlayedCard} on top of {TopCard}", playedCard.ToString(), topCard.ToString());

        // If the last card is an 8, we have to look at the wish
        if (topCard.ValueProp == Poker.Value.Eight)
        {
            return playedCard.ValueProp == Poker.Value.Eight || playedCard.SuitProp == _wishedColor;
        }

        return topCard.ValueProp == playedCard.ValueProp || topCard.SuitProp == playedCard.SuitProp ||
               playedCard.ValueProp == Poker.Value.Eight;
    }

    public ICollection<ICard> Shuffle()
    {
        var rnd = new Random();
        var n = _deck.Count;

        _logger.LogInformation("Shuffling {Amount} cards", n);

        var deckAsList = _deck.ToList();
        while (n > 1)
        {
            n--;
            var k = rnd.Next(n + 1);
            (deckAsList[k], deckAsList[n]) = (deckAsList[n], deckAsList[k]);
        }

        _deck = new Queue<ICard>(deckAsList);
        return _deck.ToList();
    }

    public ICard GetLastPlayedCard()
    {
        return _playedCards.Peek();
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

    public virtual void Play(int id, int cardIndex)
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
        if (((Poker) card).ValueProp != Poker.Value.Eight)
        {
            NextPlayer();
        }
        else
        {
            HasPlayedEight = true;
        }
    }

    public IEnumerable<IGameFeature> GetExtraOptions()
    {
        var result = new List<IGameFeature>
        {
            new ChooseSuitFeature(this, Poker.Suit.Clovers),
            new ChooseSuitFeature(this, Poker.Suit.Hearts),
            new ChooseSuitFeature(this, Poker.Suit.Tiles),
            new ChooseSuitFeature(this, Poker.Suit.Pikes),
            new TakeCardFeature(this),
            new DoNothing(this)
        };

        return result;
    }

    public virtual void NextPlayer()
    {
        CurrentPlayer = (CurrentPlayer + 1) % PlayerCards.Length;
        HasPlayedEight = false;
        HasTakenCard = false;
    }

    public void ExecuteFeature(int id, int featureId)
    {
        _logger.LogDebug("Player {PlayerId} is trying to execute feature {FeatureIndex}", id, featureId);
        GetExtraOptions().ToList()[featureId].Execute(id);
    }

    public List<GameData> GetGameData()
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

            if (((Poker) GetLastPlayedCard()).ValueProp == Poker.Value.Eight)
            {
                topCard += $": {Poker.SpanFromSuit(_wishedColor)}";
            }

            var features = GetExtraOptions().Select(feature => feature.GetName()).ToList();
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

    private class TakeCardFeature : IGameFeature
    {
        private readonly CrazyEights _game;

        public TakeCardFeature(CrazyEights game)
        {
            _game = game;
        }

        public string GetName()
        {
            return "Take";
        }

        public bool IsExecutable(int player)
        {
            return _game.CurrentPlayer == player && !_game.HasTakenCard && !_game.HasPlayedEight;
        }

        public bool Execute(int player)
        {
            // Abort if not executable
            if (!IsExecutable(player)) return false;

            _game.PlayerCards[player].Add(_game.TakeCard());
            _game.HasTakenCard = true;
            return true;
        }
    }

    private class DoNothing : IGameFeature
    {
        private readonly CrazyEights _game;

        public DoNothing(CrazyEights game)
        {
            _game = game;
        }

        public string GetName()
        {
            return "Skip";
        }

        public bool IsExecutable(int player)
        {
            return _game.HasTakenCard && _game.CurrentPlayer == player;
        }

        public bool Execute(int player)
        {
            if (!IsExecutable(player)) return false;

            _game.NextPlayer();
            return true;
        }
    }

    private class ChooseSuitFeature : IGameFeature
    {
        private readonly CrazyEights _game;
        private readonly Poker.Suit _suit;

        public ChooseSuitFeature(CrazyEights game, Poker.Suit t)
        {
            _game = game;
            _suit = t;
        }

        public string GetName()
        {
            return $"Choose {Poker.SpanFromSuit(_suit)}";
        }

        public bool IsExecutable(int player)
        {
            // If the player isn't playing or did not play an eight, he can't choose a suit
            return _game.CurrentPlayer == player && _game.HasPlayedEight;
        }

        public bool Execute(int player)
        {
            // Abort if not executable
            if (!IsExecutable(player)) return false;

            // The next player has not played the eight
            _game.HasPlayedEight = false;
            _game._wishedColor = _suit;
            _game.NextPlayer();
            return true;
        }
    }
}