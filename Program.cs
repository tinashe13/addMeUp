using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {

        string input = "--in abc.txt --out xyz.txt";
        string[] inputs = input.Split(' ');

        string inputFile = "";
        string outputFile = "";

        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] == "--in")
            {
                inputFile = inputs[i + 1];
            }
            else if (inputs[i] == "--out")
            {
                outputFile = inputs[i + 1];
            }
        }

        Console.WriteLine($"Input file: {inputFile}");
        Console.WriteLine($"Output file: {outputFile}");

        // Read all the lines from the file and store them in a list
        List<Player> players = null;
        try
        {
            players = File.ReadAllLines(inputFile).Select(p => new Player(p)).ToList();
            // Do something with players
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: Invalid input file. " + ex.Message);
        }


        // Calculate the points for each player
        if (players == null || players.Count == 0)
        {
            Console.WriteLine("No players found.");
        }
        else
        {
            foreach (Player player in players)
            {
                player.CalculatePoints();
            }
        }

        try
        {
            // Find the winner(s)
            List<Player> winners = players.OrderByDescending(p => p.Points).Where(p => p.Points == players.Max(pl => pl.Points)).ToList();
            string winnerString = string.Join(",", winners.Select(p => p.Name));

            // Recalculate scores for tied players using suit score
            if (winners.Count > 1)
            {
                foreach (Player player in winners)
                {
                    player.CalculateSuitScore();
                }

                winners = winners.OrderByDescending(p => p.SuitScore).Where(p => p.SuitScore == winners.Max(pl => pl.SuitScore)).ToList();
                winnerString = string.Join(",", winners.Select(p => p.Name));
            }

            // Write the output to a file
            string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            using (StreamWriter file = new StreamWriter(Path.Combine(directory, outputFile)))
            {
                file.WriteLine($"{winnerString}:{winners[0].Points}");
            }

            Console.WriteLine($"Winner(s): {winnerString}, with {winners[0].Points} points.");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"Argument is null. {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    class Player
    {
        public string Name { get; private set; }
        public string[] Cards { get; private set; }
        public int Points { get; private set; }
        public int SuitScore { get; private set; }

        public Player(string playerString)
        {
            string[] nameCards = playerString.Split(':');
            Name = nameCards[0];
            Cards = nameCards[1].Split(',');
        }

        public void CalculatePoints()
        {
            int[] cardValues = new int[Cards.Length];

            for (int i = 0; i < Cards.Length; i++)
            {
                string card = Cards[i];
                string value = card.Substring(0, card.Length - 1);

                if (value == "A")
                {
                    cardValues[i] = 11;
                }
                else if (value == "J")
                {
                    cardValues[i] = 11;
                }
                else if (value == "Q")
                {
                    cardValues[i] = 12;
                }
                else if (value == "K")
                {
                    cardValues[i] = 13;
                }
                else
                {
                    cardValues[i] = int.Parse(value);
                }
            }

            Array.Sort(cardValues);
            Array.Reverse(cardValues);

            for (int i = 0; i < Math.Min(3, cardValues.Length); i++)
            {
                Points += cardValues[i];
            }
        }

        public void CalculateSuitScore()
        {
            SuitScore = Cards.Max(card => GetSuitValue(card));
        }

        private int GetSuitValue(string card)
        {
            string suit = card.Substring(card.Length - 1, 1);

            switch (suit)
            {
                case "D":
                    return 1;
                case "H":
                    return 2;
                case "S":
                    return 3;
                case "C":
                    return 4;
                default:
                    return 0;
            }
        }
    }
    
}