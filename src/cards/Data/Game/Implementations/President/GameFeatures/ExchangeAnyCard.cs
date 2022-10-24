using cards.Data.Game.Decks;

namespace cards.Data.Game.Implementations.President.GameFeatures
{
    public class ExchangeAnyCard : IGameFeature
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

        public string Name => $"Give {_card.ToHtmlString()}";

        public bool IsExecutable(int player) => !_game._exchangesFinished && _canExchange;

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
}
