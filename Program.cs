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
                    else if (player.PlayerAmountBetted == Transactions.amountToCall)
                    {
                        Player.PlayerBet(player);
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
                playerFour
            };
            bool on = true;
            bool notSwapped = true;
            while (on)
            {
                MainPlayerLoop(players);
                if (notSwapped)
                {
                    PlayerSwapAndPlayerReset(players);
                    notSwapped = false;
                }
                else
                {
                    CheckWhoWonAndReset(players);
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
        public static void CheckWhoWonAndReset(List<Player> players)
        {
            // check who won
            List<((int,int), Player)> playerWinningHands = new();
            foreach (var player in players)
            {
                if (!player.Fold)
                {
                    var hands = Player.WinningHands(player.Hand);
                    playerWinningHands.Add((hands, player));
                }
            }
            // find the index of the highest int
            playerWinningHands.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            Console.WriteLine($"{playerWinningHands[0].Item2.Name} wins!!!");
            playerWinningHands[0].Item2.TotalMoney += Transactions.pot;
            foreach (var player in players)
            {
                Player.DisplayPlayersCards(player.Hand);
                Player.ResetPlayerCardsAndIncreaseBlindOrder(player);
                player.PlayerAmountBetted = 0;
            }
            Deck.Reset();
            players = Player.FixTurnOrder(players);
            Transactions.pot = 0;
            Console.WriteLine();
            Console.WriteLine("New Game");
            Transactions.IncreaseBlinds();
            Player.ShowBalances(players);
        }
    }
}
