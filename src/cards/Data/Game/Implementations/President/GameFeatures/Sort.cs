namespace cards.Data.Game.Implementations.President.GameFeatures
{
    public class Sort : IGameFeature
    {
        private readonly President _game;

        public Sort(President game)
        {
            _game = game;
        }

        public string Name => "Sort";

        public bool IsExecutable(int player) => true;

        public bool Execute(int player)
        {
            _game._playerCards[player].Sort();

            return true;
        }
    }
}
