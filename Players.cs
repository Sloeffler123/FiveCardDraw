using TransactionSetUp;
using DeckFile;
using System.Runtime.CompilerServices;
namespace PlayerSetUp
{
    public class Player
    {
        public List<(int, string)> Hand;
        public int TotalMoney;
        public string Name;
        public int BlindOrder;
        public bool Fold;
        public int PlayerAmountBetted;


        public Player(List<(int, string)> hand, int totalMoney, string name, int blindOrder, bool fold, int playerAmountBetted)
        {
            Hand = hand;
            TotalMoney = totalMoney;
            Name = name;
            BlindOrder = blindOrder;
            Fold = fold;
            PlayerAmountBetted = 0;


        }

        public static (int, int) WinningHands(List<(int, string)> playerHand)
        {
            int first = 0;
            int second = 0;

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

            playerHand.Sort((a, b) => a.Item1.CompareTo(b.Item1));
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
            var groups = numbers.GroupBy(n => n).Select(g => g.Count()).OrderByDescending(c => c).ToList();

            if (groups[0] == 4)
            {
                fourOfAKind = true;
            }
            else if (groups[0] == 3 && groups.Count > 1 && groups[1] == 2)
            {
                fullHouse = true;
            }
            else if (groups[0] == 3)
            {
                threeOfAKind = true;
            }
            else if (groups.Count(g => g == 2) == 2)
            {
                twoPair = true;
            }
            else if (groups[0] == 2)
            {
                onePair = true;
            }
            // check straights
            if (!royalFlush)
            {
                // check straight
                if (cardTwo == cardOne + 1 && cardThree == cardTwo + 1 && cardFour == cardThree + 1 && cardFive == cardFour + 1)
                {
                    straight = true;
                }
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
            }
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
            foreach (var hand in checkWinningHands)
            {
                if (hand)
                {
                    first = checkWinningHands.IndexOf(hand);
                    foreach (var totalHand in playerHand)
                    {
                        second += totalHand.Item1;
                    }

                }
                return (first, second);
            }
            first = checkWinningHands.IndexOf(highCard);
            foreach (var totalHand in playerHand)
            {
                double infinity = double.NegativeInfinity;
                if (totalHand.Item1 > infinity)
                {
                    infinity = totalHand.Item1;
                }
                second = (int)infinity;
            }
            return (first, second);
        }

        public static void ResetPlayerCardsAndIncreaseBlindOrder(Player player)
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
            Player newPlayer = new(Player.GetPlayersCards(deck), 1000, name, turnOrder, false, 0);
            return newPlayer;
        }

        // call this if its first player
        public static void PlayerBet(Player player)
        {
            bool holder = false;
            Console.WriteLine();
            if (TransactionSetUp.Transactions.round == 1)
            {
                Console.WriteLine($"Pot - {TransactionSetUp.Transactions.bigBlind}");
                Console.WriteLine();
                Console.WriteLine($"{player.Name} how much would you like to bet? ");
            }
            else
            {
                Console.WriteLine($"Pot - {TransactionSetUp.Transactions.pot}");
                Console.WriteLine();
                Console.WriteLine($"{player.Name} how much would you like to bet? ");
            }
            if (player.PlayerAmountBetted == 0)
            {
                holder = true;
            }
            string bet = Console.ReadLine();
            int playerBet = int.Parse(bet);
            player.TotalMoney -= playerBet;
            player.PlayerAmountBetted += playerBet;
            TransactionSetUp.Transactions.amountToCall = player.PlayerAmountBetted;
            if (holder)
            {
                Transactions.pot += player.PlayerAmountBetted;
                holder = false;
            }
        }

        public static void PlayerRaise(Player player)
        {
            Console.WriteLine("How much would you like to raise?");
            Console.WriteLine();
            string userInput = Console.ReadLine();
            int playerRaise = int.Parse(userInput) + TransactionSetUp.Transactions.amountToCall - player.PlayerAmountBetted;
            TransactionSetUp.Transactions.amountToCall = playerRaise + player.PlayerAmountBetted;
            player.PlayerAmountBetted = TransactionSetUp.Transactions.amountToCall;
            TransactionSetUp.Transactions.pot += playerRaise;

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
            Console.WriteLine();
            Console.WriteLine(string.Join(" ", playersDeck));
        }
        // call this on second player through forth
        public static void CallRaiseFold(Player player)
        {
            // bug fix
            // need to make sure every play calls the raise
            // need to fix winning hands to actually pick the correct winner
            string userInput = "";
            Console.WriteLine();
            Console.WriteLine($"Pot - {TransactionSetUp.Transactions.pot}");
            Console.WriteLine();
            if (player.BlindOrder > 0)
            {
                Console.WriteLine($"{player.Name} what would you like to do? (C)all, (F)old, (R)aise");
                Console.WriteLine($"Call - {TransactionSetUp.Transactions.amountToCall - player.PlayerAmountBetted}");
                Console.WriteLine();
                userInput = Console.ReadLine().ToUpper();
            }
            else if (player.BlindOrder == 0)
            {
                Console.WriteLine("How much would you like to bet?");
                Console.WriteLine();
                userInput = Console.ReadLine().ToUpper();
            }

            if (userInput == "C" | userInput == "CALL")
            {
                player.TotalMoney -= TransactionSetUp.Transactions.amountToCall - player.PlayerAmountBetted;
                TransactionSetUp.Transactions.pot += TransactionSetUp.Transactions.amountToCall - player.PlayerAmountBetted;
                player.PlayerAmountBetted = TransactionSetUp.Transactions.amountToCall;
            }
            else if (userInput == "F" | userInput == "FOLD")
            {
                player.Fold = true;
            }
            else if (userInput == "R" | userInput == "RAISE")
            {
                PlayerRaise(player);
            }
            else
            {
                PlayerBet(player);
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
            Console.WriteLine();
            Console.WriteLine($"{player.Name}: How many cards would you like to swap? (0-3)");
            DisplayPlayersCards(player.Hand);

            string input = Console.ReadLine().ToUpper();
            int userInput = int.Parse(input);
            DisplayPlayersCards(player.Hand);
            Console.WriteLine("Please input the order of the cards you want to swap from left to right(1-5).");
            if (userInput == 0)
            {
                return;
            }
            for (int i = 0; i < userInput; i++)
            {
                string swap = Console.ReadLine();
                int swapPosition = int.Parse(swap) - 1;
                player.Hand[swapPosition] = GetCard();

            }
            Console.WriteLine("New Deck");
            player.Hand.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            DisplayPlayersCards(player.Hand);
        }

        public static (int, string) GetCard()
        {
            Random randomCard = new();
            (int, string) newCard = DeckFile.Deck.WholeDeck[randomCard.Next(0, DeckFile.Deck.WholeDeck.Count)];
            DeckFile.Deck.WholeDeck.Remove(newCard);
            return newCard;
        }

        public static void ShowBalances(List<Player> players)
        {
            foreach (var player in players)
            {
                Console.WriteLine($"{player.Name} - {player.TotalMoney}");
            }
        }

        public static List<Player> FixTurnOrder(List<Player> players)
        {
            return players.OrderBy(p => p.BlindOrder).ToList();
        }

        public static bool CheckIfAllPlayersCall(List<Player> players)
        {
            bool isTrue = false;
            foreach (var player in players)
            {

                if (player.PlayerAmountBetted == Transactions.amountToCall || player.Fold)
                {
                    isTrue = true;
                }
                else
                {
                    isTrue = false;
                }
            }
            return isTrue;
        }

        public static void CheckIfTie(List<(int, Player)> winningHands)
        {
            foreach (var hand in winningHands)
            {
                
            }
        }
    }
}  