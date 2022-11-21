namespace cards.Data.Game.Implementations.CrazyEights.GameFeatures
{
    public class TakePlusTwoCardsFeature : IGameFeature
    {
        private readonly CrazyEightsVariation _game;

        public TakePlusTwoCardsFeature(CrazyEightsVariation game)
        {
            _game = game;
        }

        public string Name => $"Take {_game._stackedTwos}";

        public bool IsExecutable(int player) => _game.CurrentPlayer == player && _game._stackedTwos > 0;

        public bool Execute(int player)
        {
            // Abort if not executable
            if (!IsExecutable(player)) return false;

            while (_game._stackedTwos > 0)
            {
                _game.PlayerCards[player].Add(_game.TakeCard());
                _game._stackedTwos--;
            }

            return true;
        }
    }
}
