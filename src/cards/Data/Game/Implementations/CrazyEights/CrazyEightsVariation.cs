using cards.Data.Enums.Card;
using cards.Data.Game.Decks;
using cards.Data.Game.Implementations.CrazyEights.GameFeatures;

namespace cards.Data.Game.Implementations.CrazyEights;

public class CrazyEightsVariation : CrazyEights
{
    private readonly ILogger<CrazyEightsVariation> _logger;

    private bool _wrongDirection;

    public int _stackedTwos;

    public override string Title => "Crazy Eights Variation";

    public override string Description => "Crazy Eights with extra rules:<br>" +
            "<b>Queens skip:</b> Playing a Queen causes the next player to miss their turn.<br>" +
            "<b>Aces reverse direction:</b> Playing an Ace reverses the direction of play.<br>" +
            "<b>Draw 2:</b> Playing a two forces the next player to draw two cards, unless they can play another two. Multiple twos \"stack\"; if a two is played in response to a two, the next player must draw four.<br>" +
            "<b>Points:</b> The game ends as soon as one player has emptied their hand. That player collects a payment from each opponent equal to the point score of the cards remaining in that opponent's hand. " +
            "8s score 50, court cards 10 and all other cards face value.<br>" +
            "<i>Source: Wikipedia</i>";

    public CrazyEightsVariation()
    {
        _logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger<CrazyEightsVariation>();
    }

    protected override bool IsPlayable(ICard card)
    {
        // If the player has to take twos and doesn't try to play a two, the move is invalid
        if (_stackedTwos > 0 && ((Poker)card).Value != Value.Two)
            return false;

        return base.IsPlayable(card);
    }

    public override void Play(int id, int cardIndex)
    {
        var skipPlayer = false;
        // Check whether the direction turns
        if (CanPlay(id, cardIndex))
        {
            var card = (Poker)PlayerCards[id][cardIndex];
            skipPlayer = card.Value == Value.Queen;

            if (card.Value == Value.Ace)
            {
                _wrongDirection = !_wrongDirection;
            }

            if (card.Value == Value.Two)
            {
                _logger.LogInformation("Stack increased by two: {StackedTwos}", _stackedTwos);
                _stackedTwos += 2;
            }
        }

        base.Play(id, cardIndex);

        if (skipPlayer)
        {
            if (_wrongDirection)
            {
                CurrentPlayer = (CurrentPlayer - 1 + PlayerCards.Length) % PlayerCards.Length;
            }
            else
            {
                CurrentPlayer = (CurrentPlayer + 1) % PlayerCards.Length;
            }
        }
    }

    protected override IEnumerable<IGameFeature> GetExtraOptions()
    {
        var result = base.GetExtraOptions().ToList();
        result.Add(new TakePlusTwoCardsFeature(this));
        return result;
    }

    public override void NextPlayer()
    {
        // If the player hasn't played, he has taken a card
        // The player has to take all stacked twos
        if (HasTakenCard)
        {
            while (_stackedTwos > 0)
            {
                _logger.LogInformation("Taking a card from the stack, " +
                                       "{CurrentPlayer} picked up a card",
                    CurrentPlayer);
                PlayerCards[CurrentPlayer].Add(TakeCard());
                _stackedTwos--;
            }
        }

        if (_wrongDirection)
        {
            CurrentPlayer = (CurrentPlayer - 1 + PlayerCards.Length) % PlayerCards.Length;
            HasPlayedEight = false;
            HasTakenCard = false;
        }
        else
        {
            base.NextPlayer();
        }
    }

    public override List<GameData> GetGameData()
    {
        var result = base.GetGameData();

        if (_stackedTwos > 0)
        {
            for (var i = 0; i < PlayerCards.Length; i++)
            {
                result[i].TopCard += $" (take {_stackedTwos} cards)";
            }
        }

        return result;
    }
}