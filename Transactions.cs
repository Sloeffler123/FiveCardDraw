using System.Runtime.CompilerServices;

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

        public static void AddBlindsToPot()
        {
            pot += bigBlind + smallBlind;
        }
        // public static bool CheckIfAmountSuffecient()
        // {
        // }
    }
}