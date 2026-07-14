using System.Text.RegularExpressions;
using TriPeaks_Solver;
// 28 cards on table
// 24 cards in deck
// define card placements
// S=Spades H=Hearts C=Clubs D=Diamonds
// A,2-10,J,Q,K

string[] previousForkMoves;

// select input file, read in, and validate values
string inputFile = GetInputFile();
string input = File.ReadAllText(inputFile);
string[] tokens = Regex.Replace(input, @"\s+", " ").Split(" ");
string exceptionMessage = ValidateInput(tokens);

if (!exceptionMessage.Equals(string.Empty))
//if (message.Equals(string.Empty))
//    Console.WriteLine("Input represents a valid deck.");
//else
{
    Console.WriteLine($"Input does not represent a valid deck. {exceptionMessage}");
    return;
}

// deal the table cards
// starts on the bottom left
Card[] tableCards = new Card[28];
Dictionary<Value, int> tableCounts = new();
for (int i = 0; i < 28; i++)
{
    bool faceUp = i < 10;
    try
    {
        tableCards[i] = new Card(tokens[i], faceUp, i);

        if (tableCounts.TryGetValue(tableCards[i].Value, out int count))
            tableCounts[tableCards[i].Value] = ++count;
        else
            tableCounts.Add(tableCards[i].Value, 1);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        return;
    }
}

// order the deck cards
List<Card> deckCards = new();
for (int i = 28; i < 52; i++)
{
    bool faceUp = false;
    try
    {
        deckCards.Add(new Card(tokens[i], faceUp, i));
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        return;
    }
}

// sole initial fork is the starting state of the current game
List<Fork> forks = new()
{
    new Fork(null, tableCards, tableCounts, deckCards, new List<Card>(), new()
            {
                {Value.Ace, 4},
                {Value.Two, 4},
                {Value.Three, 4},
                {Value.Four, 4},
                {Value.Five, 4},
                {Value.Six, 4},
                {Value.Seven, 4},
                {Value.Eight, 4},
                {Value.Nine, 4},
                {Value.Ten, 4},
                {Value.Jack, 4},
                {Value.Queen, 4},
                {Value.King, 4}
            }, deckCards[0])
};
//previousForkMoves = forks[0].GetMoves().Split("|");

Fork? solution = null;

Console.Write("Checking for solutions. 0 moves checked. ");
// scan for possible moves
while (forks.Count > 0)
{
    if (forks[0].IsSolution)
    {
        solution = forks[0];
        forks.Remove(forks[0]);
        break;
    }

    Fork[] newForks = forks[0].Explore();
    forks.Remove(forks[0]); // remove this fork once it's explored
    foreach (Fork newFork in newForks)
    {
        forks.Insert(0, newFork);
        //PrintForkDiff(newFork);
    }

    Console.SetCursorPosition(24, Console.CursorTop);
    Console.Write($"{Fork.movesChecked} moves checked. ");
}

// print solutions
string message = solution == null ? "No solutions found." : $"\r\nSolution found:\r\n{solution.GetMoves()}";
Console.Write(message);

static string GetInputFile()
{
    IEnumerable<string> files = Directory.EnumerateFiles("../../../InputFiles");
    string listFiles = "";
    for (int i = 0; i < files.Count(); i++)
    {
        string file = files.ElementAt(i);
        listFiles += $"{i + 1} - {file.Substring(file.LastIndexOf('/') + 1)}\r\n";
    }

    Console.Write($"{listFiles}\r\n\r\nSelect Input File: ");
    int index;
    while (true)
    {
        string? choice = Console.ReadLine();

        if (int.TryParse(choice, out index))
        {
            try
            {
                // added 1 earlier to make it 1-based; subtract 1 to bring it back to 0-based
                choice = files.ElementAt(index - 1);
                // break only if above doesn't throw exception (valid index)
                return choice;
            }
            catch { }
        }

        Console.Write("\r\nInvalid choice. Please enter a number corresponding to the files listed above: ");
    }
}

static string ValidateInput(string[] tokens)
{
    // This method does *NOT* check if values are valid cards.
    // It ONLY checks whether there are 52 unique values.
    // Validity of individual values as cards will be checked when the card objects are created.

    if (tokens.Length == 52)
    {
        Dictionary<string, int> duplicateTest = new Dictionary<string, int>();
        foreach (string token in tokens)
        {
            try
            {
                duplicateTest.Add(token, 0);
            }
            catch
            {
                return "There are duplicate cards in the deck provided. The deck should have 52 unique cards.";
            }
        }
    }
    else
        return $"There are {tokens.Length} cards in the deck provided. There should be exactly 52.";

    return string.Empty;
}

void PrintForkDiff(Fork fork)
{
    Console.WriteLine();
    string ret = "";
    string[] moves = fork.GetMoves().Split("|");

    for (int i = 1; i < moves.Length; i++)
    {
        try
        {
            if (moves[i].Equals(previousForkMoves[i]))
            {
                for (int j = 0; j < previousForkMoves[i].Length; j++)
                    ret += " ";
            }
            else
                ret += moves[i];
        }
        catch (IndexOutOfRangeException)
        {
            // we reached the end of the previous string
            // just add the rest of the moves
            ret += moves[i];
        }

        ret += " | ";
    }

    previousForkMoves = moves;
    Console.Write(ret.Substring(0, ret.Length - 3));
}