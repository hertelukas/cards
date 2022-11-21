using cards.Data.Game.Decks;
using cards.Data.Game.Implementations.President.GameFeatures;

namespace cards.Data.Game.Implementations.President;

public class President : Game
{
    private readonly ILogger<President> _logger;

    private Action<List<Poker>> ShuffleService { get; init; }

    public List<Poker>[] _playerCards;
    private List<Poker> _deck;
    public Stack<List<Poker>> _playedCards;
    public List<Poker> _currentlyPlayedCards; // A list of cards the current player tries to play
    private readonly List<int> _finishedPlayers; // A list of all players that already finished
    public int _currentPlayer;
    public int _lastPlayerPlayed = -1; // The last player who played a card
    public int _presidentIndex = -1;
    public int _vicePresidentIndex = -1;
    public int _highScumIndex = -1;
    public int _scumIndex = -1;

    public List<Poker> _cardsForPresident;
    public List<Poker> _cardsForVicePresident;
    public List<Poker> _cardsForHighScum;
    public List<Poker> _cardsForScum;

    public bool _exchangesFinished = true;

    public President(IPersistentInformation gameInformation)
    {
        if (gameInformation.GetType() == typeof(PersistentInformation))
        {
            LoadPersistentInformation((PersistentInformation)gameInformation);
        }

        _logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger<President>();
        _playerCards = Array.Empty<List<Poker>>();
        _deck = new List<Poker>();
        _playedCards = new Stack<List<Poker>>();
        _currentlyPlayedCards = new List<Poker>();
        _finishedPlayers = new List<int>();

        _cardsForPresident = new List<Poker>();
        _cardsForScum = new List<Poker>();
        _cardsForVicePresident = new List<Poker>();
        _cardsForHighScum = new List<Poker>();

        ShuffleService = ShufflingStrategies.FisherYatesShuffle;
    }

    public override string Title => "President (Beta)";

    public override string Description => "<b>Dealing: </b>  All the cards are dealt as evenly as possible in clockwise rotation. " +
            "After cards are dealt, the scum must hand over the best card in their hand to the president, " +
            "and the president passes back any card they do not want. When playing with four or more players, " +
            "the president exchanges two cards with the scum and the vice president one with the high scum. <br>" +
            "<b>Playing: </b> Play in President is organized into tricks, much like in spades or bridge. " +
            "However, unlike those games, each trick can involve more than one card played by each player, " +
            "and players do not have to play a card in a trick. Suits are irrelevant in the game of president. <br>" +
            "The player on the dealer's left begins by leading any number of cards of the same rank. " +
            "The player on the left may then play an equal number of matching cards with a higher face value, or may pass. " +
            "Note that the same number of cards as the lead must be played. If the leader starts with a pair, only pairs may be played on top of it. " +
            "If three-of-a-kind is led, only three-of-a-kinds can be played on top of it. " +
            "The next player may do the same, and so on. <br>" +
            "This continues until all players have had a turn (which may or may not be because the highest-value card has already been played), or opted to pass. <br>" +
            "<b>End of a round: </b> When one player runs out of cards, they are out of play for the rest of the round, " +
            "but the other players can continue to play to figure out the titles. <br>" +
            "This version does not rearrange the seating of the players, so everyone plays in the same order each hand (though the scum leads the first trick).<br>" +
            "<i>Source: Wikipedia</i>";

    public override bool PointsAreGood => true;

    public override void Initialize(int players)
    {
        _logger.LogInformation("Initializing {Title} with {Players} players", Title, players);
        _playerCards = new List<Poker>[players];

        _deck = Poker.GetDeck().ToList();
        ShuffleService.Invoke(_deck);

        // Initialize player cards
        for (var i = 0; i < players; i++)
        {
            _playerCards[i] = new List<Poker>();
        }

        // Deal cards
        var current = 0;
        while (_deck.Count != 0)
        {
            _playerCards[current].Add(_deck[0]);
            _deck.RemoveAt(0);

            current = (current + 1) % players;
        }
    }

    public override void Play(int id, int cardIndex)
    {
        if (!CanPlay(id, cardIndex))
        {
            return;
        }

        var card = _playerCards[id][cardIndex];

        _logger.LogInformation("{PlayerId} is playing {Card}", id, card.ToString());

        _playerCards[id].Remove(card);
        _currentlyPlayedCards.Add(card);

        if (_playedCards.Count != 0 && _currentlyPlayedCards.Count == _playedCards.Peek().Count)
        {
            _playedCards.Push(_currentlyPlayedCards);
            _lastPlayerPlayed = id;

            NextPlayer();
        }
    }

    public override bool IsOver()
    {
        Console.WriteLine($"Checking if over: {_finishedPlayers.Count} >= {_playerCards.Length - 1}");
        return _finishedPlayers.Count >= _playerCards.Length - 1;
    }

    public override List<int> CalcPoints()
    {
        var result = new int[_playerCards.Length];

        var points = _playerCards.Length;
        foreach (var playerIndex in _finishedPlayers)
        {
            result[playerIndex] = points--;
        }

        return new List<int>(result);
    }

    public override IPersistentInformation GetPersistentInformation()
    {
        // If only one player, no king
        if (_finishedPlayers.Count == 0)
        {
            return new DummyInformation();
        }

        var scumIndex = 0;

        while (_finishedPlayers.Contains(scumIndex))
        {
            scumIndex++;
        }


        // If two or three players, king and scum
        if (_finishedPlayers.Count < 3)
        {
            return new PersistentInformation(
                _finishedPlayers[0],
                -1,
                -1,
                scumIndex);
        }

        return new PersistentInformation(
            _finishedPlayers[0],
            _finishedPlayers[1],
            _finishedPlayers[^1],
            scumIndex);
    }

    public override void ExecuteFeature(int id, int featureId)
    {
        _logger.LogDebug("Player {PlayerId} is trying to execute feature {FeatureIndex}", id, featureId);
        GetFeatures(id).ToList()[featureId].Execute(id);
    }

    public override List<GameData> GetGameData()
    {
        var result = new List<GameData>();

        for (var i = 0; i < _playerCards.Length; i++)
        {
            var cards = _playerCards[i].Select(card => card.ToHtmlString());
            var otherPlayersAmountOfCards = new List<int>();

            for (var j = 1; j < _playerCards.Length; j++)
            {
                otherPlayersAmountOfCards.Add(_playerCards[(i + j) % _playerCards.Length].Count);
            }

            var topCards = "No cards played";
            if (_playedCards.Count != 0)
            {
                topCards = string.Join(", ", _playedCards.Peek().Select(card => card.ToHtmlString()));
            }

            if (_currentPlayer == i && _currentlyPlayedCards.Count > 0)
            {
                topCards +=
                    $"<h5> Played: {string.Join(", ", _currentlyPlayedCards.Select(card => card.ToHtmlString()))}</h5>";
            }

            var features = GetFeatures(i).Select(feature => feature.Name).ToList();
            var featuresEnabled = GetFeatures(i).Select(feature => feature.IsExecutable(i)).ToList();

            result.Add(new GameData(
                cards,
                otherPlayersAmountOfCards,
                (_currentPlayer - i + _playerCards.Length) % _playerCards.Length,
                topCards,
                features,
                featuresEnabled
            ));
        }

        return result;
    }

    public void CheckExchangesFinished()
    {
        // If not all exchanges finished, return
        if (_presidentIndex != -1)
        {
            if (_cardsForPresident.Count == 0 || _cardsForScum.Count == 0)
            {
                return;
            }
        }

        if (_vicePresidentIndex != -1)
        {
            if (_cardsForVicePresident.Count != 1 || _cardsForHighScum.Count != 1 || _cardsForPresident.Count != 2 ||
                _cardsForScum.Count != 2)
            {
                return;
            }

            // All exchanges satisfied, exchange cards
            _playerCards[_vicePresidentIndex].AddRange(_cardsForVicePresident);
            _playerCards[_highScumIndex].AddRange(_cardsForHighScum);
        }

        _playerCards[_presidentIndex].AddRange(_cardsForPresident);
        _playerCards[_scumIndex].AddRange(_cardsForScum);


        _exchangesFinished = true;
    }

    public void NextPlayer()
    {
        // Add all finished players to the list
        for (var i = 0; i < _playerCards.Length; i++)
        {
            if (_playerCards[i].Count == 0 && !_finishedPlayers.Contains(i))
            {
                _finishedPlayers.Add(i);
            }
        }

        if (IsOver())
        {
            return;
        }

        _currentlyPlayedCards = new List<Poker>();

        _currentPlayer = (_currentPlayer + 1) % _playerCards.Length;

        var oneRound = false;
        while (_finishedPlayers.Contains(_currentPlayer))
        {
            if (_currentPlayer == _lastPlayerPlayed)
            {
                oneRound = true;
            }

            _currentPlayer = (_currentPlayer + 1) % _playerCards.Length;
            Console.WriteLine($"Current: {_currentPlayer}, Last: {_lastPlayerPlayed}");
        }

        if (_currentPlayer == _lastPlayerPlayed || oneRound)
        {
            NewRound();
        }
    }

    private void LoadPersistentInformation(PersistentInformation information)
    {
        _presidentIndex = information.PresidentIndex;
        _vicePresidentIndex = information.VicePresidentIndex;
        _highScumIndex = information.HighScumIndex;
        _scumIndex = information.ScumIndex;
        // Scum can start with the next round
        _currentPlayer = _scumIndex;
        _exchangesFinished = false;
    }

    private bool IsPlayable(Poker card)
    {
        // If the player already has played some cards, that don't match, return false
        if (_currentlyPlayedCards.Count > 0 && _currentlyPlayedCards[0].Value != card.Value)
        {
            return false;
        }

        if (_playedCards.Count == 0)
        {
            return true;
        }

        var lastPlayedCards = _playedCards.Peek();
        return lastPlayedCards[0].Value < card.Value;
    }

    private bool CanPlay(int playerId, int cardIndex)
    {
        // If there are still exchanges happening, nothing can be played
        if (!_exchangesFinished)
        {
            return false;
        }

        // Check whether the player is playing
        if (_currentPlayer != playerId)
        {
            return false;
        }

        // Check whether the player owns this card
        if (_playerCards[playerId].Count < cardIndex)
        {
            return false;
        }

        var card = _playerCards[playerId][cardIndex];

        // Check whether the card is playable
        return IsPlayable(card);
    }

    private void NewRound()
    {
        _lastPlayerPlayed = -1;
        _playedCards = new Stack<List<Poker>>();
    }

    private IEnumerable<IGameFeature> GetFeatures(int playerIndex)
    {
        var result = new List<IGameFeature>
        {
            new PlayFirstCards(this),
            new Cancel(this),
            new Sort(this),
            new Pass(this),
            new ExchangeBestCard(this, playerIndex)
        };

        // Add all give cards features
        if (!_exchangesFinished)
        {
            for (var i = 0; i < _playerCards[playerIndex].Count; i++)
            {
                result.Add(new ExchangeAnyCard(this, playerIndex, i));
            }
        }

        return result;
    }
}