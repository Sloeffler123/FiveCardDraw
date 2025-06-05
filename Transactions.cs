namespace TransactionSetUp
{
    public class Transactions
    {
        public static int round = 0;
        public static int pot = 0;
        public static int bigBlind = 25;
        public static int smallBlind = 10;

        public static void DisplayPotAndBlinds()
        {
            Console.WriteLine($"Big Blind: {bigBlind} - Small Blind: {smallBlind} - Pot: {pot}");
        } 

    }
}