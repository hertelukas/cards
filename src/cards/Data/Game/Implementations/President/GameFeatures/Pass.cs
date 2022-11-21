namespace cards.Data.Game.Implementations.President.GameFeatures
{
    public class Pass : IGameFeature
    {
        private readonly President _game;

        public Pass(President game)
        {
            _game = game;
        }

        public string Name => "Pass";

        public bool IsExecutable(int player) => _game._currentPlayer == player && _game._exchangesFinished;

        public bool Execute(int player)
        {
            if (!IsExecutable(player)) return false;

            // Add played cards back to the player
            _game._playerCards[player].AddRange(_game._currentlyPlayedCards);
            _game.NextPlayer();

            return true;
        }
    }
}
