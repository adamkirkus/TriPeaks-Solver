using System.Security.Cryptography;
using System.Text;

namespace TriPeaks_Solver
{
    public class Fork
    {
        public static long movesChecked = 0;
        private static Dictionary<string, bool> checksums = new();

        public bool IsSolution = false;

        private Fork? _parent;
        private Card _card;
        private State _state;

        public Fork(Fork parent, Card card) : this(parent, parent._state.TableCards, parent._state.OnTableCounts,
            parent._state.DeckCards, parent._state.Discard, parent._state.InPlayCounts, card)
        { }

        public Fork(Fork? parent, Card?[] tableCards, Dictionary<Value, int> tableCounts, List<Card> deckCards, List<Card> discard,
            Dictionary<Value, int> inPlayCounts, Card card)
        {
            _parent = parent;
            _card = card.Clone();
            _state = new State(tableCards, tableCounts, deckCards, discard, inPlayCounts, _card);

            //IsSolution = _state.DeckCards.Count + _state.Discard.Count == 52;
            IsSolution = !new List<Card?>(_state.TableCards).Where(x => x != null).Any();

            movesChecked++;
            //checksums.Add(Checksum(), true);
        }

        public string Checksum()
        {
            byte[] abc = MD5.HashData(Encoding.UTF8.GetBytes(GetMoves()));

            return Get0xStringFromBytes(abc);
        }

        private string Get0xStringFromBytes(byte[] input)
        {
            string ret = "";
            foreach (byte b in input)
            {
                ret += b.ToString("X2");
            }

            return ret;
        }

        public string GetMoves()
        {
            string ret = "";
            Fork? temp = this;
            while (temp != null)
            {
                //ret = $"{temp.GetCardName()} | {ret}";
                ret = $"{temp.GetCardName()}\r\n{ret}";
                temp = temp._parent;
            }

            return ret;
        }

        public string GetCardName()
        {
            return _card.GetFullName();
        }

        public Fork[] Explore()
        {
            // collect a list of all face-up cards that are one above or below the current card's value
            List<Card> possibleMoves = new();
            foreach (Card? tableCard in _state.TableCards)
            {
                if (tableCard == null)
                    continue;

                // don't continue down this fork if any table cards don't have a possible match from this state
                if (CheckIfDeadEnd(tableCard))
                    return Array.Empty<Fork>();

                if (tableCard.IsFaceUp && MoveIsPossible(tableCard, _card))
                    possibleMoves.Add(tableCard);
            }

            List<Fork> forks = new();

            // draw from the deck
            // always a possible move unless it's the last card in the deck
            if (_state.DeckCards.Count > 0)
                forks.Add(new Fork(this, _state.DeckCards[0]));

            possibleMoves.ForEach(card => forks.Add(new Fork(this, card)));

            return forks.ToArray();
        }

        private bool CheckIfDeadEnd(Card card)
        {
            if (_state.NumMovesRemaining(card) < _state.OnTableCounts[card.Value])
                return true;

            return false;
        }

        private static bool MoveIsPossible(Card card1, Card card2)
        {
            PossibleMove poss = new(card1);

            return card2.Value == poss.Under || card2.Value == poss.Over;
        }

        private class State
        {
            public Card?[] TableCards = new Card[28];
            public List<Card> DeckCards = new(), Discard = new();
            public Dictionary<Value, int> OnTableCounts = new(), InPlayCounts = new();

            public State(Card?[] tableCards, Dictionary<Value, int> tableCounts, List<Card> deckCards, List<Card> discard,
                Dictionary<Value, int> inPlayCounts, Card card)
            {
                // deep copy game state at time of fork so we can try other routes and come back if they didn't work
                for (int i = 0; i < tableCards.Length; i++)
                {
                    TableCards[i] = tableCards[i]?.Clone();
                }
                tableCounts.ToList().ForEach(v => OnTableCounts.Add(v.Key, v.Value));
                deckCards.ForEach(c => DeckCards.Add(c.Clone()));
                discard.ForEach(c => Discard.Add(c.Clone()));
                inPlayCounts.ToList().ForEach(v => InPlayCounts.Add(v.Key, v.Value));

                // card on top of discard is being covered (i.e. taken out of play)
                if (Discard.Count > 0)
                    --InPlayCounts[Discard[0].Value];

                // move the target card to the top of the discard pile
                Discard.Insert(0, card);

                // card moved from table
                if (card.Position < 28)
                {
                    TableCards[card.Position] = null;
                    --OnTableCounts[card.Value];

                    // after the card is moved, flip any uncovered cards
                    TableCardDependencies.FlipDependent(card, TableCards);
                }
                // card drawn from deck
                else
                {
                    card.Flip();
                    DeckCards.Remove(DeckCards.Where(x => x.Position == card.Position).ElementAt(0));
                }
            }

            public int NumMovesRemaining(Card card)
            {
                PossibleMove poss = new(card);

                return InPlayCounts[poss.Under] + InPlayCounts[poss.Over];
            }
        }
    }
}