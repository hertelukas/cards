using cards.Data.Game.Decks;

namespace cards.Data.Game;

public class CrazyEights : IGameService
{
    private List<ICard>[] _playerCards;
    private Queue<ICard> _deck;
    private Stack<ICard> _playedCards;
    private int _currentPlayer = 0;

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

        return topCard.Value == playedCard.Value || topCard.Suit == playedCard.Suit || playedCard.Value == Poker.Value.Eight;
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
        // TODO let players wish a suit after playing 8
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

        _currentPlayer = (_currentPlayer + 1) % _playerCards.Length;
    }

    public List<IGameFeature> GetExtraOptions()
    {
        throw new NotImplementedException();
    }
}