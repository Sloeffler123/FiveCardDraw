
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace SetUp
{
    public class Deck
    {
        public static List<(int, string)> WholeDeck = DeckSetUp();
        public Deck(List<(int, string)> deck)
        {
            WholeDeck = DeckSetUp();
        }
        public static void Main()
        {
            //make players and get their cards
            var playerOne = Player.MakePlayer(WholeDeck, "Player1", 0);
            var playerTwo = Player.MakePlayer(WholeDeck, "Player2", 1);
            var playerThree = Player.MakePlayer(WholeDeck, "Player3", 2);
            var playerFour = Player.MakePlayer(WholeDeck, "Player4", 3);
            // player list 
            List<Player> players = new List<Player>
            {
                playerOne,
                playerTwo,
                playerThree,
                playerFour,

            };
            // 
            foreach (var player in players)
            {
                Console.WriteLine($"Player : {string.Join(" ", player.Hand)}, {player.TotalMoney}, {player.Name}, {player.BlindOrder}, {player.Fold}");
            }
            
        }

        // makes deck of 52 cards
        public static List<(int, string)> DeckSetUp()
        {

            List<(int, string)> suitsAndCards = new();
            string[] suits = { "Spades", "Diamonds", "Clubs", "Hearts" };
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

        // gets players cards randomly
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

        // determine of card is face or not
        public static void DisplayPlayersCards(List<(int, string)> playersHand)
        {
            List<string> playersDeck = new();
            foreach (var card in playersHand)
            {
                if (card.Item1 == 11)
                {
                    playersDeck.Add($"Jack {card.Item2}");
                }
                else if (card.Item1 == 12)
                {
                    playersDeck.Add($"Queen {card.Item2}");
                }
                else if (card.Item1 == 13)
                {
                    playersDeck.Add($"King {card.Item2}");
                }
                else if (card.Item1 == 14)
                {
                    playersDeck.Add($"Ace {card.Item2}");
                }
                else
                {
                    playersDeck.Add(card.ToString());
                }
            }
            Console.WriteLine(string.Join(" ", playersDeck));
        }


    }

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

        // takes the deck setup as the param
        public static Player MakePlayer(List<(int, string)> deck, string name, int turnOrder)
        {
            Player newPlayer = new(Deck.GetPlayersCards(deck), 1000, name, turnOrder, false);
            return newPlayer;
        }

        // call this if its first player or if player raises
        public static int PlayerBet(Player player)
        {
            Console.WriteLine($"{player} how much would you like to bet? ");
            string bet = Console.ReadLine();
            int playerBet = int.Parse(bet);
            player.TotalMoney -= playerBet;
            return playerBet;
        }
        // call this on second player through forth
        public static void CallRaiseFold(Player player, Transactions transaction, int playerBet)
        {

            Console.WriteLine($"{player.Name} what would you like to do? (C)all, (F)old, (R)aise");
            string userInput = Console.ReadLine().ToUpper();
            if (userInput == "C" | userInput == "CALL")
            {
                player.TotalMoney -= playerBet;
                transaction.pot += playerBet;
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
                transaction.pot += playerBet + playerRaise;
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
        public static void SwapCards(Player player, Deck deck)
        {
            Console.WriteLine("How many cards would you like to swap? (0-3)");

            string input = Console.ReadLine().ToUpper();
            int userInput = int.Parse(input);
            Deck.DisplayPlayersCards(player.Hand);
            Console.WriteLine("Please input the order of the cards you want to swap from left to right(0-5).");

            for (int i = userInput; i < userInput; i++)
            {
                string swap = Console.ReadLine();
                int swapPosition = int.Parse(swap);
                player.Hand[swapPosition] = GetCard();

            }
            Console.WriteLine("New Deck");
            Deck.DisplayPlayersCards(player.Hand);
        }

        public static (int, string) GetCard()
        {
            Random randomCard = new();
            (int, string) newCard = Deck.WholeDeck[randomCard.Next(0, Deck.WholeDeck.Count)];
            Deck.WholeDeck.Remove(newCard);
            return newCard;
        }
    }

    public class Transactions
    {
        public int round = 0;
        public int pot = 0;
        public int bigBlind = 25;
        public int smallBlind = 10;

    }
}

