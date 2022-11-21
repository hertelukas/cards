namespace cards.Data.Game.Implementations.CrazyEights.GameFeatures
{
    public class DoNothing : IGameFeature
    {
        private readonly CrazyEights _game;

        public DoNothing(CrazyEights game)
        {
            _game = game;
        }

        public string Name => "Skip";

        public bool IsExecutable(int player) => _game.HasTakenCard && _game.CurrentPlayer == player;

        public bool Execute(int player)
        {
            if (!IsExecutable(player)) return false;

            _game.NextPlayer();
            return true;
        }
    }
}
