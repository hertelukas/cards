namespace cards.Data.Game.Implementations.CrazyEights.GameFeatures
{
    public class TakeCardFeature : IGameFeature
    {
        private readonly CrazyEights _game;

        public TakeCardFeature(CrazyEights game)
        {
            _game = game;
        }

        public string Name => "Take";

        public bool IsExecutable(int player) => _game.CurrentPlayer == player && !_game.HasTakenCard && !_game.HasPlayedEight;

        public bool Execute(int player)
        {
            // Abort if not executable
            if (!IsExecutable(player)) return false;

            _game.PlayerCards[player].Add(_game.TakeCard());
            _game.HasTakenCard = true;
            return true;
        }
    }
}
