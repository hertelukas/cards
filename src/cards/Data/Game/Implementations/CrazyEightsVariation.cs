using cards.Data.Game.Decks;

namespace cards.Data.Game.Implementations;

public class CrazyEightsVariation : CrazyEights
{
    private readonly ILogger<CrazyEightsVariation> _logger;

    private bool _wrongDirection;
    private int _stackedTwos;

    public static string GetTitle()
    {
        return "Crazy Eights Variation";
    }

    public static string GetDescription()
    {
        return
            "Crazy Eights with extra rules:<br>" +
            "<b>Queens skip:</b> Playing a Queen causes the next player to miss their turn.<br>" +
            "<b>Aces reverse direction:</b> Playing an Ace reverses the direction of play.<br>" +
            "<b>Draw 2:</b> Playing a two forces the next player to draw two cards, unless they can play another two. Multiple twos \"stack\"; if a two is played in response to a two, the next player must draw four.<br>" +
            "<i>Source: Wikipedia</i>";
    }

    public CrazyEightsVariation()
    {
        _logger = LoggerFactory.Create(c => c.AddConsole()).CreateLogger<CrazyEightsVariation>();
    }

    public override void Play(int id, int cardIndex)
    {
        var skipPlayer = false;
        // Check whether the direction turns
        if (CanPlay(id, cardIndex))
        {
            var card = (Poker) PlayerCards[id][cardIndex];
            skipPlayer = card.ValueProp == Poker.Value.Queen;

            if (card.ValueProp == Poker.Value.Ace)
            {
                _wrongDirection = !_wrongDirection;
            }

            if (card.ValueProp == Poker.Value.Two)
            {
                _logger.LogInformation("Stack increased by two: {StackedTwos}", _stackedTwos);
                _stackedTwos += 2;
            }
            else
            {
                while (_stackedTwos > 0)
                {
                    _logger.LogInformation(
                        "Taking a card from the stack, " +
                        "{CurrentPlayer} didn't play a two",
                        CurrentPlayer);
                    PlayerCards[CurrentPlayer].Add(TakeCard());
                    _stackedTwos--;
                }
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

    protected override void NextPlayer()
    {
        // If he played, this is wrong
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
}