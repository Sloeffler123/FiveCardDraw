using System.Runtime.CompilerServices;
using PlayerSetUp;

namespace TransactionSetUp
{
    public class Transactions
    {
        public static int round = 0;
        public static int pot = 0;
        public static int bigBlind = 25;
        public static int smallBlind = 10;
        public static int amountToCall = 0;

        public static void DisplayPotAndBlinds()
        {
            Console.WriteLine($"Big Blind: {bigBlind} - Small Blind: {smallBlind} - Pot: {pot}");
        }

        public static void IncreaseBlinds()
        {
            bigBlind += 25;
            smallBlind += 15;
        }

        public static void AddBlindsToPot(List<Player> players)
        {
            foreach (var player in players)
            {
                if (player.BlindOrder == 0)
                {
                    player.TotalMoney -= bigBlind;
                    player.PlayerAmountBetted = bigBlind;
                }
                else if (player.BlindOrder == 1)
                {
                    player.TotalMoney -= smallBlind;
                    player.PlayerAmountBetted = smallBlind;
                }
            }
            pot += bigBlind + smallBlind;
        }
        // public static bool CheckIfAmountSuffecient()
        // {
        // }
    }
}