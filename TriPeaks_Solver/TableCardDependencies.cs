using System.Collections.ObjectModel;

namespace TriPeaks_Solver
{
    public static class TableCardDependencies
    {
        // a dictionary of which cards are covering up other cards,
        // so we know when to flip a card (when both covering it are removed)
        private static ReadOnlyCollection<Dependency> _deps = new(new List<Dependency>()
        {
            {new Dependency(0, 1, 10)},

            {new Dependency(1, 0, 10)},
            {new Dependency(1, 2, 11)},

            {new Dependency(2, 1, 11)},
            {new Dependency(2, 3, 12)},

            {new Dependency(3, 2, 12)},
            {new Dependency(3, 4, 13)},

            {new Dependency(4, 3, 13)},
            {new Dependency(4, 5, 14)},

            {new Dependency(5, 4, 14)},
            {new Dependency(5, 6, 15)},

            {new Dependency(6, 5, 15)},
            {new Dependency(6, 7, 16)},

            {new Dependency(7, 6, 16)},
            {new Dependency(7, 8, 17)},

            {new Dependency(8, 7, 17)},
            {new Dependency(8, 9, 18)},

            {new Dependency(9, 8, 18)},

            {new Dependency(10, 11, 19)},

            {new Dependency(11, 10, 19)},
            {new Dependency(11, 12, 20)},

            {new Dependency(12, 11, 20)},

            {new Dependency(13, 14, 21)},

            {new Dependency(14, 13, 21)},
            {new Dependency(14, 15, 22)},

            {new Dependency(15, 14, 22)},

            {new Dependency(16, 17, 23)},

            {new Dependency(17, 16, 23)},
            {new Dependency(17, 18, 24)},

            {new Dependency(18, 17, 24)},

            {new Dependency(19, 20, 25)},

            {new Dependency(20, 19, 25)},

            {new Dependency(21, 22, 26)},

            {new Dependency(22, 21, 26)},

            {new Dependency(23, 24, 27)},

            {new Dependency(24, 23, 27)}
        });

        public static void FlipDependent(Card movedCard, Card?[] tableCards)
        {
            if (movedCard.Position < 25)
            {
                foreach (Dependency dep in _deps.Where(x => x.Principal == movedCard.Position))
                {
                    if (tableCards[dep.Partner] == null)
                        tableCards[dep.Dependent]?.Flip();
                }
            }
        }

        private class Dependency
        {
            public readonly int Principal, Partner, Dependent;

            public Dependency(int principal, int partner, int dependent)
            {
                Principal = principal;
                Partner = partner;
                Dependent = dependent;
            }
        }
    }

    public class PossibleMove
    {
        public Value Under, Over;

        public PossibleMove(Card card)
        {
            Under = card.Value == Value.Ace ? Value.King : card.Value - 1;
            Over = card.Value == Value.King ? Value.Ace : card.Value + 1;
        }
    }
}