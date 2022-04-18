namespace cards.Data.Game;

public static class ShufflingStrategies
{
    /// <summary>
    /// Shuffles a list of arbitrary type
    /// </summary>
    /// <param name="cards">The list of cards to be shuffled</param>
    /// <typeparam name="T">Element type of that list</typeparam>
    public static void FisherYatesShuffle<T>(List<T> cards)
    {
        var rnd = new Random();
        var n = cards.Count;

        while (n > 1)
        {
            n--;
            var k = rnd.Next(n + 1);
            (cards[k], cards[n]) = (cards[n], cards[k]);
        }
    }
}