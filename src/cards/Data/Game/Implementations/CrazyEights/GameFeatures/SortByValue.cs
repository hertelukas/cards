namespace cards.Data.Game.Implementations.CrazyEights.GameFeatures
{
    public class SortByValue : IGameFeature
    {
        private readonly CrazyEights _game;

        public SortByValue(CrazyEights game)
        {
            _game = game;
        }

        public string Name => "Sort by value";

        public bool IsExecutable(int player) => true;

        public bool Execute(int player)
        {
            _game.PlayerCards[player].Sort();
            return true;
        }
    }
}
