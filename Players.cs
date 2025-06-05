using TransactionSetUp;
using DeckFile;
namespace PlayerSetUp
{
    public class Player
    {
        public List<(int, string)> Hand;
        public int TotalMoney;
        public string Name;
        public int BlindOrder;
        public bool Fold;

        public Player(List<(int, string)> hand, int totalMoney, string name, int blindOrder, bool fold)
        {
            Hand = hand;
            TotalMoney = totalMoney;
            Name = name;
            BlindOrder = blindOrder;
            Fold = fold;

        }

        public static int WinningHands(List<(int, string)> playerHand)
        {

            bool royalFlush = false;
            bool straightFlush = false;
            bool fourOfAKind = false;
            bool fullHouse = false;
            bool flush = false;
            bool straight = false;
            bool threeOfAKind = false;
            bool twoPair = false;
            bool onePair = false;
            bool highCard = false;

            List<bool> checkWinningHands = new()
            {
                royalFlush,
                straightFlush,
                fourOfAKind,
                fullHouse,
                flush,
                straight,
                threeOfAKind,
                twoPair,
                onePair,
                highCard
            };

            int cardOne = playerHand[0].Item1;
            int cardTwo = playerHand[1].Item1;
            int cardThree = playerHand[2].Item1;
            int cardFour = playerHand[3].Item1;
            int cardFive = playerHand[4].Item1;

            // split list into suit and integers
            List<int> numbers = new();
            foreach (var num in playerHand)
            {
                numbers.Add(num.Item1);
            }
            List<string> suits = new();
            foreach (var suit in playerHand)
            {
                suits.Add(suit.Item2);
            }

            // check flush
            if (suits.All(s => s == suits[0]))
            {
                flush = true;
            }
            // check all pairs
            if (!fourOfAKind)
            {// check pair
                if (numbers.Count(n => n == 2) == 2)
                {
                    onePair = true;
                }
                // check three of a kind
                if (numbers.Count(n => n == 3) == 3)
                {
                    threeOfAKind = true;
                }
                // check four of a kind
                if (numbers.Count(n => n == 4) == 4)
                {
                    fourOfAKind = true;
                }
            }
            // check two of kind and full house
            if (!fullHouse)
            {
                // check full house
                if (onePair && threeOfAKind)
                {
                    fullHouse = true;
                }
                // check two pair
                int pairCount = 0;
                foreach (var num in numbers)
                {
                    if (numbers.Count(n => n == 2) == 2)
                    {
                        pairCount += 1;
                    }
                }
                if (pairCount == 4)
                {
                    twoPair = true;

                }
            }
            // check straights
            if (!royalFlush)
            {
                // check royal flush
                if (straight && flush && cardOne == 11)
                {
                    royalFlush = true;
                }
                // check straight flush
                if (straight && flush)
                {
                    straightFlush = true;
                }
                // check straight
                if (cardTwo == cardOne - 1 && cardThree == cardTwo - 1 && cardFour == cardThree - 1 && cardFive == cardFour - 1)
                {
                    straight = true;
                }
            }

            foreach (var hand in checkWinningHands)
            {
                if (hand)
                {
                    return checkWinningHands.IndexOf(hand);
                }
            }
            return checkWinningHands.IndexOf(highCard);
        }

        public static void ResetPlayerCards(Player player)
        {
            player.Hand.Clear();
            player.Fold = false;
            if (player.BlindOrder == 3)
            {
                player.BlindOrder = 0;
            }
            else
            {
                player.BlindOrder += 1;
            }
        }
        public static List<(int, string)> GetPlayersCards(List<(int, string)> deck) // deck of cards from decksetup
        {
            Random random = new();
            List<(int, string)> playerCards = new();
            for (int i = 0; i < 5; i++)
            {
                int randomNum = random.Next(0, deck.Count);
                playerCards.Add(deck[randomNum]);
                deck.Remove(deck[randomNum]);
            }
            return playerCards;
        }

        // takes the deck setup as the param
        public static Player MakePlayer(List<(int, string)> deck, string name, int turnOrder)
        {
            Player newPlayer = new(Player.GetPlayersCards(deck), 1000, name, turnOrder, false);
            return newPlayer;
        }

        // call this if its first player or if player raises
        public static int PlayerBet(Player player)
        {
            Console.WriteLine($"{player.Name} how much would you like to bet? ");
            string bet = Console.ReadLine();
            int playerBet = int.Parse(bet);
            player.TotalMoney -= playerBet;
            return playerBet;
        }

        public static void DisplayPlayersCards(List<(int, string)> playersHand)
        {
            List<string> playersDeck = new();
            foreach (var card in playersHand)
            {
                if (card.Item1 == 11)
                {
                    playersDeck.Add($"(Jack, {card.Item2})");
                }
                else if (card.Item1 == 12)
                {
                    playersDeck.Add($"(Queen, {card.Item2})");
                }
                else if (card.Item1 == 13)
                {
                    playersDeck.Add($"(King, {card.Item2})");
                }
                else if (card.Item1 == 14)
                {
                    playersDeck.Add($"(Ace, {card.Item2})");
                }
                else
                {
                    playersDeck.Add(card.ToString());
                }
            }
            Console.WriteLine(string.Join(" ", playersDeck));
        }
        // call this on second player through forth
        public static void CallRaiseFold(Player player, int playerBet)
        {

            Console.WriteLine($"{player.Name} what would you like to do? (C)all, (F)old, (R)aise");
            Console.WriteLine($"Call - {Transactions.bigBlind + playerBet}");

            string userInput = Console.ReadLine().ToUpper();
            if (userInput == "C" | userInput == "CALL")
            {
                player.TotalMoney -= playerBet;
                Transactions.pot += playerBet;
            }
            else if (userInput == "F" | userInput == "FOLD")
            {
                player.Fold = true;
            }
            else if (userInput == "R" | userInput == "RAISE")
            {
                var playerRaise = PlayerBet(player);
                player.TotalMoney -= playerBet;
                player.TotalMoney -= playerRaise;
                Transactions.pot += playerBet + playerRaise;
            }
        }
        // check if player has enough money to bet
        public static bool CheckEnoughMoney(Player player, int playerBet)
        {
            if (player.TotalMoney < playerBet)
            {
                Console.WriteLine("Insuffecient funds");
                return false;
            }
            return true;
        }

        // change cards
        public static void SwapCards(Player player)
        {
            Console.WriteLine($"{player.Name}: How many cards would you like to swap? (0-3)");

            string input = Console.ReadLine().ToUpper();
            int userInput = int.Parse(input);
            Player.DisplayPlayersCards(player.Hand);
            Console.WriteLine("Please input the order of the cards you want to swap from left to right(0-5).");
            if (userInput == 0)
            {
                return;
            }
            for (int i = userInput; i < 0; i++)
                {
                    string swap = Console.ReadLine();
                    int swapPosition = int.Parse(swap);
                    player.Hand[swapPosition] = GetCard();

                }
            Console.WriteLine("New Deck");
            Player.DisplayPlayersCards(player.Hand);
        }

        public static (int, string) GetCard()
        {
            Random randomCard = new();
            (int, string) newCard = Deck.WholeDeck[randomCard.Next(0, Deck.WholeDeck.Count)];
            Deck.WholeDeck.Remove(newCard);
            return newCard;
        }

    }
}  