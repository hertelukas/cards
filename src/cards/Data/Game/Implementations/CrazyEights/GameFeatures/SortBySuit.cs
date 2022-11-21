using cards.Data.Game.Decks;

namespace cards.Data.Game.Implementations.CrazyEights.GameFeatures
{
    public class SortBySuit : IGameFeature
    {
        private readonly CrazyEights _game;

        public SortBySuit(CrazyEights game)
        {
            _game = game;
        }

        public string Name => "Sort by suit";

        public bool IsExecutable(int player) => true;

        public bool Execute(int player)
        {
            _game.PlayerCards[player].Sort((c1, c2) =>
            {
                var p1 = (Poker)c1;
                var p2 = (Poker)c2;

                var suitCompare = p1.Suit.CompareTo(p2.Suit);
                return suitCompare == 0 ? p1.Value.CompareTo(p2.Value) : suitCompare;
            });

            return true;
        }
    }
}
