namespace cards.Data.Game.Implementations.President.GameFeatures
{
    public class PlayFirstCards : IGameFeature
    {
        private readonly President _game;

        public PlayFirstCards(President game)
        {
            _game = game;
        }

        public string Name => "Confirm";

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
}
