using cards.Data.Game.Decks;

namespace cards.Data.Game.Implementations.President.GameFeatures
{
    public class Cancel : IGameFeature
    {
        private readonly President _game;

        public Cancel(President game)
        {
            _game = game;
        }

        public string Name => "Start over";

        public bool IsExecutable(int player) => _game._currentPlayer == player && _game._currentlyPlayedCards.Count > 0;

        public bool Execute(int player)
        {
            if (!IsExecutable(player)) return false;

            // Add the cards back to the player
            _game._playerCards[player].AddRange(_game._currentlyPlayedCards);
            _game._currentlyPlayedCards = new List<Poker>();
            return true;
        }
    }
}
