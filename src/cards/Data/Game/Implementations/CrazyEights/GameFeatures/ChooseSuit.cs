using cards.Data.Enums.Card;
using cards.Data.Game.Decks;

namespace cards.Data.Game.Implementations.CrazyEights.GameFeatures
{
    public class ChooseSuitFeature : IGameFeature
    {
        private readonly CrazyEights _game;
        private readonly Suit _suit;

        public ChooseSuitFeature(CrazyEights game, Suit suit)
        {
            _game = game;
            _suit = suit;
        }

        public string Name => $"Choose {Poker.SpanFromSuit(_suit)}";

        // If the player isn't playing or did not play an eight, he can't choose a suit
        public bool IsExecutable(int player) => _game.CurrentPlayer == player && _game.HasPlayedEight;

        public bool Execute(int player)
        {
            // Abort if not executable
            if (!IsExecutable(player)) return false;

            // The next player has not played the eight
            _game.HasPlayedEight = false;
            _game.WishedColor = _suit;
            _game.NextPlayer();
            return true;
        }
    }
}
