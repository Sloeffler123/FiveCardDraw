using PlayerSetUp;
using DeckFile;
using TransactionSetUp;
namespace SetUp
{   
    public class MainSetUp
    {
        public static void Rules()
        {
            Console.WriteLine("Welcome to Five Card Draw. Each player gets five cards. This is then followed by the first wave of betting. After everyone makes their bets players can swap up to three cards for new ones from the deck. This is then follwed by the final round of betting.");

        }

        public static void Main()
        {
            // display rules
            Console.WriteLine();
            Rules();
            Console.WriteLine();
            var deck = DeckFile.Deck.WholeDeck;
            //make players and get their cards
            var playerOne = Player.MakePlayer(deck, "Player1", 0);
            var playerTwo = Player.MakePlayer(deck, "Player2", 1);
            var playerThree = Player.MakePlayer(deck, "Player3", 2);
            var playerFour = Player.MakePlayer(deck, "Player4", 3);
            // player list 
            List<Player> players = new List<Player>
            {
                playerOne,
                playerTwo,
                playerThree,
                playerFour,

            };
            // Main Loop
            bool on = true;
            bool blindsTaken = false;
            while (on)
            {
                // displays players hands
                int playerBet = 0;
                TransactionSetUp.Transactions.round += 1;
                TransactionSetUp.Transactions.DisplayPotAndBlinds();
                if (!blindsTaken)
                {
                    TransactionSetUp.Transactions.AddBlindsToPot();
                    blindsTaken = true;
                }
                foreach (var player in players)
                {
                    // sort cards from least to best
                    player.Hand.Sort((a, b) => a.Item1.CompareTo(b.Item1));
                    Console.WriteLine();
                    Console.WriteLine($"{player.Name}'s hand");
                    PlayerSetUp.Player.DisplayPlayersCards(player.Hand);
                    // ask for bet
                    if (player.Fold)
                    {
                        continue;
                    }
                    if (player.BlindOrder == 0)
                    {
                        player.TotalMoney -= TransactionSetUp.Transactions.bigBlind;
                        playerBet = PlayerSetUp.Player.PlayerBet(player);
                    }
                    else if (player.BlindOrder == 1)
                    {
                        player.TotalMoney -= TransactionSetUp.Transactions.smallBlind;
                        PlayerSetUp.Player.CallRaiseFold(player, playerBet);
                    }
                    else
                    {
                        PlayerSetUp.Player.CallRaiseFold(player, playerBet);
                    }
                }
                if (TransactionSetUp.Transactions.round != 2)
                {
                    foreach (var player in players)
                    {
                        if (!player.Fold)
                        {
                            PlayerSetUp.Player.SwapCards(player);
                        }
                    }
                }
                else
                {
                    // check who won
                    List<(int, Player)> playerWinningHands = new();
                    foreach (var player in players)
                    {
                        if (!player.Fold)
                        {
                            var hands = PlayerSetUp.Player.WinningHands(player.Hand);
                            playerWinningHands.Add((hands, player));
                        }
                    }
                    // find the index of the highest int
                    playerWinningHands.Sort((a, b) => b.Item1.CompareTo(a.Item1));
                    Console.WriteLine($"{playerWinningHands[0].Item2.Name} wins");
                    playerWinningHands[0].Item2.TotalMoney += Transactions.pot;
                    Transactions.pot = 0;
                    foreach (var player in players)
                    {
                        PlayerSetUp.Player.DisplayPlayersCards(player.Hand);
                        PlayerSetUp.Player.ResetPlayerCards(player);
                    }
                    Deck.Reset();
                    blindsTaken = false;
                    Console.WriteLine();
                    Console.WriteLine("New Game");
                    TransactionSetUp.Transactions.IncreaseBlinds();
                    PlayerSetUp.Player.ShowBalances(players);
                }
            }
        }
    } 
}