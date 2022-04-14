using cards.Data.Game.Decks;

namespace cards.Data.Game;

public class CrazyEights : IGameService
{
    private List<ICard>[] _playerCards;
    private Queue<ICard> _deck;
    private Stack<ICard> _playedCards;
    private int _currentPlayer = 0;
    private Poker.Suit _wishedColor = Poker.Suit.Hearts;
    private bool _hasPlayedEight = false;

    public string GetTitle()
    {
        return "Crazy Eights";
    }

    public string GetDescription()
    {
        // TODO
        return "Shedding card game for two to seven players";
    }

    public void Initialize(int players)
    {
        _playerCards = new List<ICard>[players];
        InitializeDeck();
        Shuffle();

        // Initialize player cards
        for (var i = 0; i < _playerCards.Length; i++)
        {
            _playerCards[i] = new List<ICard>();
        }

        // Give every player 5 cards
        foreach (var hand in _playerCards)
        {
            for (var i = 0; i < 5; i++)
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
                _deck.Enqueue(new Poker.Card(value, suit));
            }
        }
    }

    public int GetWinner()
    {
        for (var i = 0; i < _playerCards.Length; i++)
        {
            if (_playerCards[i].Count == 0) return i;
        }

        return -1;
    }

    public int GetCurrentPlayer()
    {
        return _currentPlayer;
    }

    public ICard TakeCard()
    {
        var result = _deck.Dequeue();

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
        return _playerCards[id];
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
        var topCard = (Poker.Card) GetLastPlayedCard();
        var playedCard = (Poker.Card) card;

        // If the last card is an 8, we have to look at the wish
        if (topCard.Value == Poker.Value.Eight)
        {
            return playedCard.Value == Poker.Value.Eight || playedCard.Suit == _wishedColor;
        }

        return topCard.Value == playedCard.Value || topCard.Suit == playedCard.Suit ||
               playedCard.Value == Poker.Value.Eight;
    }

    public ICollection<ICard> Shuffle()
    {
        var rnd = new Random();

        var n = _deck.Count;
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

    public void Play(int id, ICard card)
    {
        // Check whether the player owns this card
        if (!_playerCards[id].Contains(card))
        {
            return;
        }

        // Check whether the card is playable
        if (!IsPlayable(card))
        {
            return;
        }

        _playerCards[id].Remove(card);
        _playedCards.Push(card);

        // If no eight played, the game goes on
        if (((Poker.Card) card).Value != Poker.Value.Eight)
        {
            _currentPlayer = (_currentPlayer + 1) % _playerCards.Length;
        }
        else
        {
            _hasPlayedEight = true;
        }
    }

    public List<IGameFeature> GetExtraOptions()
    {
        var result = new List<IGameFeature>
        {
            new ChooseSuit(this, Poker.Suit.Clovers),
            new ChooseSuit(this, Poker.Suit.Hearts),
            new ChooseSuit(this, Poker.Suit.Tiles),
            new ChooseSuit(this, Poker.Suit.Pikes)
        };

        return result;
    }

    public List<GameData> GetGameData()
    {
        var result = new List<GameData>();

        for (var i = 0; i < _playerCards.Length; i++)
        {
            var cards = _playerCards[i].Select(card => card.ToString());
            var otherPlayersAmountOfCards = new List<int>();
            
            for (var j = 1; j < _playerCards.Length; j++)
            {
                otherPlayersAmountOfCards.Add(_playerCards[(i + j) % _playerCards.Length].Count);
            }

            result.Add(new GameData(cards, otherPlayersAmountOfCards, _deck.Peek().ToString()));
        }

        return result;
    }

    private class ChooseSuit : IGameFeature
    {
        private readonly CrazyEights _game;
        private readonly Poker.Suit _suit;

        public ChooseSuit(CrazyEights game, Poker.Suit t)
        {
            _game = game;
            _suit = t;
        }

        public string GetName()
        {
            return "Choose " + _suit;
        }

        public bool IsExecutable(int player)
        {
            // If the player isn't playing or did not play an eight, he can't choose a suit
            return _game._currentPlayer == player && _game._hasPlayedEight;
        }

        public bool Execute(int player)
        {
            // Abort if not executable
            if (!IsExecutable(player)) return false;
            
            // The next player has not played the eight
            _game._hasPlayedEight = false;
            _game._wishedColor = _suit;
            _game._currentPlayer = (_game._currentPlayer + 1) % _game._playerCards.Length;
            return true;
        }
    }
}