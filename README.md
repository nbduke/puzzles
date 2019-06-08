# puzzles
I like writing programs to play simple puzzles and games that I myself am often too impatient to play. Most of these programs are really basic command-line applications with text file inputs, but some involve more advanced GUIs.

## 2048 Player
This Windows Forms application replicates (poorly) the popular [2048 game](http://2048game.com/) and provides options to let the computer help you play or even play for you. It should go without saying that all credit for the game itself goes to its makers. I am not intending to copy their work or profit from it in any way. I just wanted to have some algorithmic fun with what I consider to be an intriguing and challenging little game.

## Crossword Lottery
This pretty useless program aims to help you hack crossword lottery tickets (examples [here](https://www.kylottery.com/apps/scratch_offs/games/Crossword_708) and [here](https://www.illinoislottery.com/games-hub/instant-tickets/crossword-ticket)) to maximize your gambling returns (I still don't recommend it).

The idea, credit for which I owe to my good friend Devin Kelsey, is that the information displayed on this kind of lottery ticket can in principle help you distinguish good tickets from bad ones. Each ticket contains a crossword puzzle that has been filled in already (i.e. the words are known) along with a hidden subset of English letters. You scratch the ticket to reveal the letters, and your prize depends on how many words you can make from the letters. You don't know what letters you'll get, but you do know how many - and, critically, you know the letters you'd need in order to complete the words that maximize your score. Using a probability model over subsets of the English language, a computer can easily calculate the expected value of a given ticket.

## Cryptogram Solver
I enjoy solving [cryptogram puzzles](https://en.wikipedia.org/wiki/Cryptogram), so I thought I'd write a program to solve them too. My solver isn't a general-purpose substitution cipher decoder - it solves cryptograms by taking advantage of the facts that word boundaries are known and that the letter substitutions are consistent throughout the ciphertext.

## Six Degrees
I'm a big fan of movies, and I've always been fascinated by the [Six Degrees of Kevin Bacon](https://en.wikipedia.org/wiki/Six_Degrees_of_Kevin_Bacon) concept. I enjoy challenging myself and other movie buffs to find connections between actors and actresses by thinking about the movies they've worked on together. I love the feeling of nostalgia that comes from navigating through my memories of great movies and their talented casts.

This program tries to find the shortest path between any two actors, utilizing a bidirectional search over [The Movie Database](https://www.themoviedb.org/) dataset.

## Sudoku Solver
Solves standard sudoku puzzles by modeling the puzzle as a constraint satisfaction problem and applying a heuristic-guided backtracking search to find the solution.

## Wordament
Wordament is a word game very similar to Boggle: given a 4x4 grid of tiles, each containing one or more letters, create as many words as you can by concatenating adjacent tiles. (You can find app versions for Android, iOS, and Windows.)

This Windows Forms application serves as a guide to help you ~~cheat at~~ play Wordament. First, you copy the grid of tiles into a form. Then, the program walks you through finding every possible word, in descending order by point value, of course.

## Wordplay
Wordplay is a collection of smaller interactive command-line applications that let you do interesting manipulations with words. You can:
- Unscramble - Given an arbitrary string, find all words that can be constructed from its letters. You can also find anagrams of the whole string - basically, a word unscrambler.
- Transform - Transform one word into another by changing, inserting, or deleting one letter at a time, producing a different English word with each change.
