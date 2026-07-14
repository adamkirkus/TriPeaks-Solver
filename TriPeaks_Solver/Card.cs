using System.Collections.ObjectModel;

namespace TriPeaks_Solver
{
    public class Card
    {
        public Suit Suit;
        public Value Value;
        public bool IsFaceUp;
        public int Position;

        public Card(string token, bool faceUp, int position)
        {
            if (token.Length != 2)
                throw new Exception($"Token '{token}' does not represent a valid card.");
            if (!SuitKey.TryGetValue(token.ToUpper()[1], out Suit))
                throw new Exception($"Suit value {token[1]} is not in Dictionary.");
            if (!ValueKey.TryGetValue(token.ToUpper()[0], out Value))
                throw new Exception($"Value {token[0]} is not in Dictionary");

            IsFaceUp = faceUp;
            Position = position;
        }

        public Card (Suit suit, Value value, bool faceUp, int position)
        {
            Suit = suit;
            Value = value;
            IsFaceUp = faceUp;
            Position = position;
        }

        public void Flip()
        {
            IsFaceUp = !IsFaceUp;
        }
        public string GetFullName()
        {
            return $"{Value} of {Suit}";
        }

        public Card Clone()
        {
            return new Card(Suit, Value, IsFaceUp, Position);
        }

        public static readonly ReadOnlyDictionary<char, Suit> SuitKey = new(new Dictionary<char, Suit>()
        {
            {'S', Suit.Spades},
            {'H', Suit.Hearts},
            {'C', Suit.Clubs},
            {'D', Suit.Diamonds}
        });

        public static readonly ReadOnlyDictionary<char, Value> ValueKey = new(new Dictionary<char, Value>()
        {
            {'A', Value.Ace},
            {'2', Value.Two},
            {'3', Value.Three},
            {'4', Value.Four},
            {'5', Value.Five},
            {'6', Value.Six},
            {'7', Value.Seven},
            {'8', Value.Eight},
            {'9', Value.Nine},
            {'0', Value.Ten}, // 0 is 10 to simplify input
            {'J', Value.Jack},
            {'Q', Value.Queen},
            {'K', Value.King}
        });
    }

    public enum Suit
    {
        Spades,
        Hearts,
        Clubs,
        Diamonds
    }

    public enum Value
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }
}

