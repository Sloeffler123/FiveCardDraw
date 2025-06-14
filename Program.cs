using PlayerSetUp;
using DeckFile;
using TransactionSetUp;
namespace SetUp
{
    public class MainSetUp
    {
        //make players and get their cards
        public static Player playerOne = Player.MakePlayer(Deck.WholeDeck, "Player1", 0);
        public static Player playerTwo = Player.MakePlayer(Deck.WholeDeck, "Player2", 1);
        public static Player playerThree = Player.MakePlayer(Deck.WholeDeck, "Player3", 2);
        public static Player playerFour = Player.MakePlayer(Deck.WholeDeck, "Player4", 3);
        // player list 
        public static List<Player> players = new List<Player>
        {
            playerOne,
            playerTwo,
            playerThree,
            playerFour
        };
        // used to check if the players have swapped cards yet
        public static bool notSwapped = true;
        public static void Rules()
        {
            Console.WriteLine("Welcome to Five Card Draw. Each player gets five cards. This is then followed by the first wave of betting. After everyone makes their bets players can swap up to three cards for new ones from the deck. This is then follwed by the final round of betting.");
        }

        public static void MainPlayerLoop(List<Player> players)
        {
            // Main Loop
            // displays players hands
            if (Transactions.round == 0)
            {
                Transactions.DisplayPotAndBlinds();
                Transactions.amountToCall = Transactions.bigBlind;
            }
            
            while (true)
            {
                if (Transactions.round == 0)
                {
                    Transactions.AddBlindsToPot(players);
                }
                Transactions.round += 1;
                foreach (var player in players)
                {
                    player.Hand.Sort((a, b) => a.Item1.CompareTo(b.Item1));
                    Console.WriteLine();
                    Console.WriteLine($"{player.Name}'s hand");
                    Player.DisplayPlayersCards(player.Hand);
                    if (player.Fold)
                    {
                        continue;
                    }
                    if (player.PlayerAmountBetted < Transactions.amountToCall)
                    {
                        Player.CallRaiseFold(player);
                    }
                    else if (!notSwapped)
                    {
                        Player.PlayerBet(player);
                        notSwapped = true;
                    }
                    else if (player.BlindOrder == 0 && Transactions.round == 1)
                    {
                        Player.PlayerBet(player);
                    }
                }
                if (Player.CheckIfAllPlayersCall(players))
                {
                    break;
                }
            }
        }

        public static void Main()
        {
            // display rules
            Console.WriteLine();
            Rules();
            Console.WriteLine();
            
            bool on = true;
            int count = 0;
            while (on)
            {
                MainPlayerLoop(players);
                if (notSwapped && count == 0)
                {
                    PlayerSwapAndPlayerReset(players);
                    notSwapped = false;
                    count += 1;
                }
                else
                {
                    CheckWhoWonAndReset(players);
                    notSwapped = true;
                    count = 0;
                }
            }
        }

        public static void PlayerSwapAndPlayerReset(List<Player> players)
        {
            Transactions.amountToCall = 0;
            foreach (var player in players)
            {
                if (!player.Fold)
                {
                    Player.SwapCards(player);
                    player.PlayerAmountBetted = 0;
                }
            }

        }

        public static void CheckWhoWonAndReset(List<Player> playersList)
        {
            // check who won
            List<((int, int, int, int), Player)> playerWinningHands = new();
            foreach (var player in playersList)
            {
                if (!player.Fold)
                {
                    var hands = Player.WinningHands(player.Hand);
                    playerWinningHands.Add((hands, player));
                }
            }
            // find the index of the highest int
            playerWinningHands.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            var winner = Player.CheckForTie(playerWinningHands);
            Console.WriteLine($"{winner.Name} wins!!! with a {string.Join(" ", winner.Hand)}");
            winner.TotalMoney += Transactions.pot;
            foreach (var player in playersList)
            {
                Player.DisplayPlayersCards(player.Hand);
                Player.ResetPlayerCardsAndIncreaseBlindOrder(player);
                player.PlayerAmountBetted = 0;
            }
            Deck.Reset();
            Transactions.amountToCall = 0;
            Transactions.round = 0;
            players = Player.FixTurnOrder(playersList);
            Transactions.pot = 0;
            Console.WriteLine();
            Console.WriteLine("New Game");
            Transactions.IncreaseBlinds();
            Player.ShowBalances(players);
            foreach (var player in players)
            {
                Player.GetNewCards(player);
            }
        }
    }
}
