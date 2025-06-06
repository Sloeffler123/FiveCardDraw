namespace DeckFile
{
    public class Deck
    {
        public static List<(int, string)> WholeDeck = DeckSetUp();

        public static void Reset()
        {
            WholeDeck.Clear();
            WholeDeck = DeckFile.Deck.DeckSetUp();
        }

        // makes deck of 52 cards
            public static List<(int, string)> DeckSetUp()
            {
                List<(int, string)> suitsAndCards = new();
                string[] suits = { "♠", "♦", "♣", "♥" };
                foreach (var suit in suits)
                {
                    int count = 2;
                    for (int j = 0; j < 13; j++)
                    {
                        suitsAndCards.Add((count, suit));
                        count += 1;
                    }
                }
                return suitsAndCards;
            }
    }    
}
