using cards.Data.Game.Decks;

namespace cards.Data.Game.Implementations.President.GameFeatures
{
    public class ExchangeBestCard : IGameFeature
    {
        private readonly President _game;
        private readonly Poker? _bestCard;

        public ExchangeBestCard(President game, int playerIndex)
        {
            _game = game;
            _bestCard = _game._playerCards[playerIndex].Max();
        }

        public string Name => _bestCard == null ? "" : $"Give {_bestCard.ToHtmlString()}";

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
}
