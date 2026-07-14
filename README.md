# TriPeaks-Solver
Generates solutions for games of TriPeaks Solitaire

# Instructions
1. The program looks for files in the InputFiles directory and outputs a numbered list of files to the console. The user is to select a number from the list corresponding to the game file they want to choose.

     Game file syntax is as follows:
        Each card must be listed in a text file, specified by its rank and suit. Valid rank values are single digits ranging from 2-9 representing corresponding number rank cards, with 0 representing a 10 card, as well as A, J, Q, and K representing Aces and face cards. Suits are represented by the first letter of their name, so S, H, C, and D representing Spades, Hearts, Clubs, and Diamonds.

     The order of the cards in the file should begin from the bottom left of the cards on the table (the first face-up row), progressing across each row left to right and then moving up to the next row until you reach the "peaks" (the last row of 3 cards). Following the peak cards are the deck cards, starting with the first face up card and progressing through the deck. In a new game, since only the first row of table cards is face-up, you may have to play different variations to discover the positions of all cards, even if you are unable to fully complete each game. For the deck cards, you can just cycle through the deck to figure this out. Each card must be separated by one or more whitespace characters. While not necessary, it can be helpful to separate the rows of cards with newlines and additional spaces, resulting roughly in a layout of the inverted peaks with the deck across the bottom. An example of a valid input file is as follows:

        4D 6D AS AH 8D 8H 2H 5H 0S 9S
         KH 2S 0H 7H 9D 4S 4H 5C 6H
           JH KS    7D 7S    0D 3H
             3D       5D      9H

        QS QH QD 3C 6C JD 9C 6S 3S AC 7C 4C QC 8C JS KD 2D AD 2C JC 0C KC 5S 8S
     The inversion of the peaks in the file is for ease of creating the file, as you can start entering from the first row which is already revealed. The same goes for the deck, so you can just enter the cards in the order you see them in-game.
   
     The program will let you know if your file is invalid.

  3. Once you have selected your input file, the program will run a simulation of all valid moves and return a valid list of moves, if any exist. It will not necessarily be the optimal move sequence, just the first valid sequence found. If no valid sequence is found, the program will output a message to the console specifying this.
    
  4. Follow each move on the list and click the card specified if it is face-up on the board. If the card is not face-up, that means it will be the next one in the deck, so you should cycle to the next deck card. Following these moves should lead to a valid solution of the puzzle.

# How it works
The program contains mapped information about the game and deck, and uses the specific layout from the input file to find a solution. First, the game's layout is mapped to show which cards are in the deck, on the table, face-up (able to be played), and face-down (covered by another card and thus unable to be played). Since the next move must be one above or below the last card on the discard pile, the game can use information about which cards are able to be played to determine valid next moves and explore branches from there. The game's mapped layout starts with the first row face-up, and keeps track of when face-down cards are uncovered and can be flipped face-up. When there are no more cards on the table, the game is won and the branch is returned as a valid solution. Since just the first valid solution is returned, it is not necessarily the optimal solution. It would be possible to find all valid solution branches and return the smallest one, however this would increase the calculation times significantly, and since there is really no advantage to finding shorter solutions, it is sufficient just to solve the puzzle as quickly as possible. It is also entirely likely that it actually does return the most efficient solution, since the shortest path is likely to be calculated first, however I have not confirmed this.
  
The early iterations of the program were essentially just a brute force, but search times for complex games were insanely long. This was reduced drastically by introducing more advanced logic to determine when a branch was no longer a viable solution. To do this, a mapping was added containing all card values in a valid deck, along with their initial counts (4) each. As each branch is explored, cards that are added to the discard pile are subtracted from the counts in the deck value map, since they can no longer be played. This results in a meta-overview of the game's state, allowing a branch to be discarded when any card left on the table has no more valid moves (i.e. all cards one above or below have already been discarded). Since all table cards must be played in a valid solution, this adds a little bit of calculation time upfront to avoid a massive amount of calculation time exploring hopeless branches where at least one card on the table will never be able to be played. The result was that the time to find a solution was cut from sometimes 30+ minutes for complex games to almost instant in basically every case.
