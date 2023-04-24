using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Threading;

namespace BlackJack__V0._3_
{
    public class Card
    {
        public string Value { get; }
        public string Suit { get; }

        public Card(string value, string suit)
        {
            Value = value;
            Suit = suit;
        }

        public string GetName()
        {
            return Value + " " + Suit;
        }
    }  //step 2


    public class Deck
    {
        private List<Card> cards;

        public Deck()
        {
            cards = new List<Card>();
        }

        public void CreateDeck()
        {
            string[] values = { "Туз", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Валет", "Дама", "Батя" };
            string[] suits = { "♣", "♦", "♥", "♠" };

            foreach (string suit in suits)
            {
                foreach (string value in values)
                {
                    cards.Add(new Card(value, suit));
                }
            }
        }

        public void Shuffle()
        {
            Random random = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card card = cards[k];
                cards[k] = cards[n];
                cards[n] = card;
            }
        }

        public Card Deal()
        {
            Card card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }  //step 3


    public class Player
    {
        public string Name { get; }
        public int Score { get; set; }
        public List<Card> Hand { get; }

        public Player(string name)
        {
            Name = name;
            Hand = new List<Card>();
        }

        public int GetCardValue(Card card)
        {
            int value = 0;
            int aceCount = 0;

            if (card.Value == "Туз")
            {
                value += 11;
                aceCount++;
            }
            else if (card.Value == "Валет" || card.Value == "Дама" || card.Value == "Батя")
            {
                value += 10;
            }
            else
            {
                value += int.Parse(card.Value);
            }

            while (value > 21 && aceCount > 0)
            {
                value -= 10;
                aceCount--;
            }
            this.Score += value;

            return value;
        }

        public string HandAsString()
        {
            string result = "";
            foreach (Card card in Hand)
            {
                result += card.Value;
                result += " ";
                result += card.Suit;
                result += ", ";
            }
            return result;
        }

        public void AddCard(Card card)
        {
            Hand.Add(card);
            this.GetCardValue(card);
        }
    }  //step 4


    interface IGameStrategy
    {
        bool WantCard(Player player);
    }

    class HumanStrategy : IGameStrategy
    {
        public bool WantCard(Player player)
        {

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("Хош карту? "); Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("(Y"); Console.ResetColor(); Console.Write(" / ");
            Console.ForegroundColor = ConsoleColor.Red; Console.Write("N "); Console.ResetColor(); Console.Write(") ? ");
            ConsoleKeyInfo k = Console.ReadKey(true);
            return (k.Key == ConsoleKey.Y);





            //string answer = Console.ReadLine().ToLower();
            //return (answer == "y");

        }
    }

    class Game
    {
        private Deck deck;
        private List<Player> players;
        private IGameStrategy gameStrategy;

        public Game(string playerName)
        {
            deck = new Deck();
            deck.CreateDeck();
            deck.Shuffle();
            players = new List<Player>();
            players.Add(new Player(playerName));
            players.Add(new Player("Дилер"));
            gameStrategy = new HumanStrategy();
        }

        public void ShowPlayer(Player player)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(player.Name + " была выдана карта.");
            Console.WriteLine(player.Name + " на руках: " + player.HandAsString());
            Console.WriteLine(player.Name + " имеет " + player.Score + " очков");
            Console.ResetColor();
            Console.WriteLine();
        }

        public void Start()
        {

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Добро пожаловать в "); Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Толератный Нигер Джек!"); Console.ResetColor();



            Console.WriteLine();

            while (true)
            {
                // Shuffle deck if less than 20% of cards remain
                //if (deck.PercentRemaining() < 20)
                //{
                //    Console.WriteLine("Shuffling deck...");
                //    deck.Shuffle();
                //}

                foreach (Player player in players)
                {
                    player.AddCard(deck.Deal());
                    player.AddCard(deck.Deal());
                    ShowPlayer(player);

                    while (true)
                    {
                        if (player != players[players.Count - 1])
                        {

                            if (gameStrategy.WantCard(player))
                            {
                                player.AddCard(deck.Deal());
                            }
                            else break;

                            if (player.Score > 21)
                            {
                                ShowPlayer(player);
                                Console.WriteLine();
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(player.Name + " проиграл! Дилер-мудак!");
                                Console.ResetColor();

                                return;
                            }
                        }

                        else
                        {
                            if (player.Score >= 17)
                            {
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.WriteLine("Дилеру больше карты не даём.");
                                Console.ResetColor();
                                break;
                            }
                            else
                            {
                                player.AddCard(deck.Deal());
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Дилер берёт карту.");
                                Console.ResetColor();
                                Console.WriteLine();
                            }

                            if (player.Score > 21)
                            {
                                ShowPlayer(player);
                                Console.WriteLine();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Дилер проиграл! Он лох,ты победил");
                                Console.ResetColor();

                                return;
                            }

                        }
                        ShowPlayer(player);
                    }
                }


                int maxScore = players.Max(player => player.Score);

                if (maxScore == 21)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Очко! Харооош");
                    Console.ResetColor();
                    Console.WriteLine();
                    return;
                }
                else if (maxScore > 21)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Перебор,ты проиграл!\n Дилер-мудак. Карты с киоска?\n Ай фак йор булщит");
                    Console.ResetColor();
                    Console.WriteLine();
                    return;
                }
                else if (maxScore == players[players.Count - 1].Score)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Ничья походу");
                    Console.ResetColor();
                    Console.WriteLine();
                    return;
                }
                else if (maxScore < players[players.Count - 1].Score)
                {
                    Console.WriteLine("Дилер победил!Да как так???");
                    Console.WriteLine();
                    return;
                }
                else { Console.WriteLine("Хз что тут,не я писал"); }
            }
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Game game = new Game("Я");
            game.Start();



        }
    }
}