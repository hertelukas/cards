using cards.Data.Game.Decks;

namespace cards.Data.Game.Implementations;

public class President : IGameService
{
    private readonly ILogger<President> _logger;

    private Action<List<Poker>> ShuffleService { get; init; }

    private List<Poker>[] _playerCards;
    private List<Poker> _deck;
    private Stack<List<Poker>> _playedCards;
    private List<Poker> _currentlyPlayedCards; // A list of cards the current player tries to play
    private readonly List<int> _finishedPlayers; // A list of all players that already finished
    private int _currentPlayer;
    private int _lastPlayerPlayed = -1; // The last player who played a card
    private int _presidentIndex = -1;
    private int _vicePresidentIndex = -1;
    private int _highScumIndex = -1;
    private int _scumIndex = -1;

    private List<Poker> _cardsForPresident;
    private List<Poker> _cardsForVicePresident;
    private List<Poker> _cardsForHighScum;
    private List<Poker> _cardsForScum;

    private bool _exchangesFinished = true;

    public President(IPersistentInformation gameInformation)
    {
        if (gameInformation.GetType() == typeof(PersistentInformation))
        {
            LoadPersistentInformation((PersistentInformation) gameInformation);
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

    public static string GetTitle()
    {
        return "President (Beta)";
    }

    public static string GetDescription()
    {
        //TODO
        return "TODO";
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

    private void CheckExchangesFinished()
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

    public void Initialize(int players)
    {
        _logger.LogInformation("Initializing {Title} with {Players} players", GetTitle(), players);
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

    public bool IsOver()
    {
        Console.WriteLine($"Checking if over: {_finishedPlayers.Count} >= {_playerCards.Length - 1}");
        return _finishedPlayers.Count >= _playerCards.Length - 1;
    }

    public List<int> CalcPoints()
    {
        var result = new int[_playerCards.Length];

        var points = _playerCards.Length;
        foreach (var playerIndex in _finishedPlayers)
        {
            result[playerIndex] = points--;
        }

        return new List<int>(result);
    }

    public IPersistentInformation GetPersistentInformation()
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

    public bool PointsAreGood()
    {
        return true;
    }

    private bool IsPlayable(Poker card)
    {
        // If the player already has played some cards, that don't match, return false
        if (_currentlyPlayedCards.Count > 0 && _currentlyPlayedCards[0].ValueProp != card.ValueProp)
        {
            return false;
        }

        if (_playedCards.Count == 0)
        {
            return true;
        }

        var lastPlayedCards = _playedCards.Peek();
        return lastPlayedCards[0].ValueProp < card.ValueProp;
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

    private void NextPlayer()
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

    private void NewRound()
    {
        _lastPlayerPlayed = -1;
        _playedCards = new Stack<List<Poker>>();
    }

    public void Play(int id, int cardIndex)
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

    public void ExecuteFeature(int id, int featureId)
    {
        _logger.LogDebug("Player {PlayerId} is trying to execute feature {FeatureIndex}", id, featureId);
        GetFeatures(id).ToList()[featureId].Execute(id);
    }

    public List<GameData> GetGameData()
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

            var features = GetFeatures(i).Select(feature => feature.GetName()).ToList();
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

    #region Features

    private class PlayFirstCards : IGameFeature
    {
        private readonly President _game;

        public PlayFirstCards(President game)
        {
            _game = game;
        }

        public string GetName()
        {
            return "Confirm";
        }

        public bool IsExecutable(int player)
        {
            return _game._currentPlayer == player && _game._playedCards.Count == 0 &&
                   _game._currentlyPlayedCards.Count > 0 && _game._exchangesFinished;
        }

        public bool Execute(int player)
        {
            if (!IsExecutable(player)) return false;

            _game._lastPlayerPlayed = player;
            _game._playedCards.Push(_game._currentlyPlayedCards);
            _game.NextPlayer();
            return true;
        }
    }

    private class Cancel : IGameFeature
    {
        private readonly President _game;

        public Cancel(President game)
        {
            _game = game;
        }

        public string GetName()
        {
            return "Start over";
        }

        public bool IsExecutable(int player)
        {
            return _game._currentPlayer == player && _game._currentlyPlayedCards.Count > 0;
        }

        public bool Execute(int player)
        {
            if (!IsExecutable(player)) return false;

            // Add the cards back to the player
            _game._playerCards[player].AddRange(_game._currentlyPlayedCards);
            _game._currentlyPlayedCards = new List<Poker>();
            return true;
        }
    }

    private class Sort : IGameFeature
    {
        private readonly President _game;

        public Sort(President game)
        {
            _game = game;
        }

        public string GetName()
        {
            return "Sort";
        }

        public bool IsExecutable(int player)
        {
            return true;
        }

        public bool Execute(int player)
        {
            _game._playerCards[player].Sort();

            return true;
        }
    }

    private class Pass : IGameFeature
    {
        private readonly President _game;

        public Pass(President game)
        {
            _game = game;
        }

        public string GetName()
        {
            return "Pass";
        }

        public bool IsExecutable(int player)
        {
            return _game._currentPlayer == player && _game._exchangesFinished;
        }

        public bool Execute(int player)
        {
            if (!IsExecutable(player)) return false;

            // Add played cards back to the player
            _game._playerCards[player].AddRange(_game._currentlyPlayedCards);
            _game.NextPlayer();

            return true;
        }
    }

    private class ExchangeBestCard : IGameFeature
    {
        private readonly President _game;
        private readonly Poker? _bestCard;

        public ExchangeBestCard(President game, int playerIndex)
        {
            _game = game;
            _bestCard = _game._playerCards[playerIndex].Max();
        }

        public string GetName()
        {
            return _bestCard == null ? "" : $"Give {_bestCard.ToHtmlString()}";
        }

        public bool IsExecutable(int player)
        {
            if (_game._exchangesFinished)
            {
                return false;
            }

            // Scum exchange
            if (_game._scumIndex == player)
            {
                if (_game._cardsForPresident.Count < 1)
                {
                    return true;
                }

                // Exchange two cards
                if (_game._highScumIndex != -1 && _game._cardsForPresident.Count < 2)
                {
                    return true;
                }
            }

            // High scum exchange
            if (_game._highScumIndex == player)
            {
                if (_game._cardsForVicePresident.Count < 1)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Execute(int player)
        {
            if (!IsExecutable(player)) return false;

            if (_game._scumIndex == player && _bestCard != null)
            {
                _game._cardsForPresident.Add(_bestCard);
                _game._playerCards[player].Remove(_bestCard);
            }

            if (_game._highScumIndex == player && _bestCard != null)
            {
                _game._cardsForVicePresident.Add(_bestCard);
                _game._playerCards[player].Remove(_bestCard);
            }

            _game.CheckExchangesFinished();

            return true;
        }
    }

    private class ExchangeAnyCard : IGameFeature
    {
        private readonly President _game;
        private readonly bool _canExchange;
        private readonly bool _isPresident;
        private readonly Poker _card;

        public ExchangeAnyCard(President game, int playerIndex, int cardIndex)
        {
            _game = game;

            var presidentCanExchange = game._cardsForScum.Count < 2 &&
                                       (game._cardsForScum.Count < 1 || game._vicePresidentIndex == -1);

            var vicePresidentCanExchange = game._cardsForHighScum.Count < 1;

            _isPresident = playerIndex == game._presidentIndex;

            _canExchange = _isPresident && presidentCanExchange ||
                           playerIndex == game._vicePresidentIndex && vicePresidentCanExchange;

            _card = game._playerCards[playerIndex][cardIndex];
        }

        public string GetName()
        {
            return $"Give {_card.ToHtmlString()}";
        }

        public bool IsExecutable(int player)
        {
            return !_game._exchangesFinished && _canExchange;
        }

        public bool Execute(int player)
        {
            if (!IsExecutable(player)) return false;

            if (_isPresident)
            {
                _game._cardsForScum.Add(_card);
            }
            else
            {
                _game._cardsForHighScum.Add(_card);
            }

            _game._playerCards[player].Remove(_card);

            _game.CheckExchangesFinished();
            return true;
        }
    }

    #endregion

    private class PersistentInformation : IPersistentInformation
    {
        internal readonly int PresidentIndex;
        internal readonly int VicePresidentIndex;
        internal readonly int HighScumIndex;
        internal readonly int ScumIndex;

        public PersistentInformation(int presidentIndex, int vicePresidentIndex, int highScumIndex, int scumIndex)
        {
            PresidentIndex = presidentIndex;
            VicePresidentIndex = vicePresidentIndex;
            HighScumIndex = highScumIndex;
            ScumIndex = scumIndex;
        }
    }
}