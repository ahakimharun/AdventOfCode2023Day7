using System.Collections;
using System.Diagnostics;

const string file = @"C:\Users\SaLiVa\source\repos\AdventOfCode2023Day7\AdventOfCode2023Day7\Day7Input.txt";

List<Hand> HandList = [];

using (var reader = File.OpenText(file))
{
    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        
            var cardbid = line.Split(" ");
            var hand = cardbid[0];
            var bid = int.Parse(cardbid[1]);

            var newhand = new Hand(hand, bid);
            HandList.Add(newhand);
    }
}

var orderedList = HandList.OrderBy(x => x.TypeValue).ThenBy(x => x.OrderValue).ToList();
var result = 0;
for (var i = 0; i < orderedList.Count; i++)
{
    result += (i + 1) * orderedList[i].Bid;
}

var listofj = orderedList.Where(x => x.Cards.Contains('J'));
Console.WriteLine(result);

public static class Card
{
    public static SortedDictionary<char, string> OriginalCardValues { get; set; } = new SortedDictionary<char, string>()
    {
        { 'A', "14" },
        { 'K', "13" },
        { 'Q', "12" },
        { 'J', "11" },
        { 'T', "10" },
        { '9', "09" },
        { '8', "08" },
        { '7', "07" },
        { '6', "06" },
        { '5', "05" },
        { '4', "04" },
        { '3', "03" },
        { '2', "02" }
    };
    
    public static SortedDictionary<char, string> JokerCardValues { get; set; } = new SortedDictionary<char, string>()
    {
        { 'A', "14" },
        { 'K', "13" },
        { 'Q', "12" },
        { 'T', "10" },
        { '9', "09" },
        { '8', "08" },
        { '7', "07" },
        { '6', "06" },
        { '5', "05" },
        { '4', "04" },
        { '3', "03" },
        { '2', "02" },
        { 'J', "01" } // Make the Joker the weakest card and position
    };
}

public class Hand
{
    public Hand(string cards, int bid)
    {
        Cards = cards;
        Bid = bid;
        CalculateType();
        CalculateOrder(false);
        
        // Part 2
        ImplementJokerRule();
    }

    private void ImplementJokerRule()
    {
        var oldCards = Cards;
        if (Cards.Contains('J'))
        {
            // Find out if the Joker is pretending to be a series of other cards or not
        
            if (!Cards.ToCharArray().Any(x => x != 'J'))
            {
                CalculateType();
            }
            else
            {
                // Get the highest card value with the most common occurence (without 'J')
                var commonChar = Cards.ToCharArray().Where(x => x!= 'J').GroupBy(x => x)
                    .OrderByDescending(x => x.Count())
                    .ThenByDescending(x => Card.JokerCardValues[x.Key])
                    .First()
                    .Key;
                
                // Make the Joker cards pretend to be that card
                Cards = Cards.Replace('J', commonChar);
                CalculateType();

                // Change card string back and calculate order
                Cards = oldCards;
            }
            CalculateOrder(true);
        }
    }

    private void CalculateType()
    {
        //Reset the value
        TypeValue = 0;
        foreach (var cv in Card.OriginalCardValues)
        {
            var val = Cards.Count(c => c == cv.Key) switch
            {
                1 => 10,
                2 => 100,
                3 => 1000,
                4 => 10000,
                5 => 100000,
                _ => 0
            };
            TypeValue += val;
        }
    }

    private void CalculateOrder(bool isJoker)
    {
        var outputstring = string.Empty;
        
        foreach (var c in Cards)
        {
            var charscore = string.Empty;
            charscore = !isJoker ? Card.OriginalCardValues.First(cv => cv.Key == c).Value : Card.JokerCardValues.First(cv => cv.Key == c).Value;
            outputstring += charscore;
        }

        OrderValue = int.Parse(outputstring);
    }

    public int Bid { get; }
    public string Cards { get; set; }
    public int TypeValue { get; private set; }
    public int OrderValue { get; private set; }
}