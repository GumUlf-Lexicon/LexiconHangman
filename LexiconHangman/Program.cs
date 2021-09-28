using System;
using System.Linq;
using System.Text;

namespace LexiconHangman
{
	class Program
	{

		// Maximun allowed wrong guesses. Do not change this value!
		public const int maxWrongGuesses = 10;

		static void Main()
		{
			// Get a random word from the built in list of words
			string word = GetWord().ToUpper();

			// Create an array with chars to present the correct guessed
			// letters and thier position in the word. Letters not yet 
			// correctly guessed are represented by '_' (underscore).
			// The array is also used to check if a letter is already
			// guessed by the user.
			char[] correctLetters = new char[word.Length];
			Array.Fill(correctLetters, '_');

			// Create a StringBuilder to store all the wrongly guessed
			// letters, so they can be presented to the user. It is
			// also be used to check if a letter i already guessed,
			// together with correctLetters above.
			StringBuilder wrongLetters = new StringBuilder();

			// Start the game
			int allowedWrongGuessesLeft = maxWrongGuesses;
			bool victory = false;
			string message = null;

			// Main loop. Runs until the number of wrong guesses are
			// equal to maxWrongGuesses, or the user has won.
			while(allowedWrongGuessesLeft > 0 && !victory)
			{

				// Print out the "game board"
				int wrongGuessesMade = maxWrongGuesses - allowedWrongGuessesLeft;
				WriteInfo(wrongGuessesMade, correctLetters, wrongLetters, message);
				message = null;

				// Get the guess from the user 
				// The returned value is at least one letter long
				// and contains only letters, so no check for that
				// is needed.
				string guessMessage = "Enter your guess (" + allowedWrongGuessesLeft + " left): ";
				string guess = GetGuessFromUser(guessMessage).ToUpper();

				if(guess.Length > 1 && guess.Equals(word))
				{
					// The player guessed the correct word
					victory = true;
				}
				else if(guess.Length > 1)
				{
					// The player guesses the wrong word
					message = $"Sorry, {guess} is not the correct word!";
					allowedWrongGuessesLeft--;
				}
				else if(Array.IndexOf(correctLetters, guess[0]) >= 0 || wrongLetters.ToString().Contains(guess))
				{
					// The guessed letter exists in either correctLetters or
					// in wrongLetters. The user is notified that it is already
					// guessed. The remaining allowed wrong guesses are not 
					// changed.
					message = $"You have already guessed {guess}!";
				}
				else if(word.Contains(guess))
				{
					// The player guessed a correct letter
					int position = word.IndexOf(guess);
					while(position >= 0)
					{
						// Put the correct guess letter in the right positions in
						// correctLetter
						correctLetters[position] = guess[0];
						position = word.IndexOf(guess, position + 1);
					}

					if(word.Equals(string.Concat(correctLetters)))
					{
						// The whole word is correctly guessed
						victory = true;
					}
				}
				else
				{
					// The player guessed a letter that is not in the word, they
					// are notified about that, and the remaining allowed wrong
					// guesses is decreased by one.
					message = $"Sorry, '{guess}' is not in the word!";
					_ = wrongLetters.Append(guess);
					allowedWrongGuessesLeft--;
				}
			}

			// The game is over, time to present the result
			WriteInfo(maxWrongGuesses - allowedWrongGuessesLeft, correctLetters, wrongLetters, message);

			if(victory)
			{
				// The player got the word right and won the game.
				Console.WriteLine("Congratulations, you won!");
			}
			else
			{
				// The player was not able to guess the right word and lost.
				Console.WriteLine("Sorry, you lost!");
				Console.WriteLine($"The correct word is \"{word}\"!");
			}

		}

		// Get a random word from the built in list of words.
		private static string GetWord()
		{
			// Create the list of words
			string[] words = new string[] { "blomma", "höstglöd", "bo", "å", "klippa", "simma", "blå", "tidning", "längta", "stor" };

			// Get a random word from the list
			var rand = new Random();
			string word = words[rand.Next(words.Length)];

			return word;
		}

		// Write out the "game board" and game state
		private static void WriteInfo(
			int numberOfWrongGuesses, 
			char[] correctLetters, 
			StringBuilder wrongLetters, 
			string message)
		{
			Console.Clear();

			// Print header
			Console.WriteLine("Time to play Hangman!");
			Console.WriteLine("  (Swedish words)");
			Console.WriteLine();

			// Print the current hangman figure
			PrintHangman(numberOfWrongGuesses);

			if(wrongLetters.Length > 0)
			{
				// Only show the line with wrong letters if there are any.
				Console.WriteLine();
				Console.WriteLine($"Wrong letters: {wrongLetters}");
			}

			// Write out the correctly guessed letter in the word
			// so far. Letters yet not correctly guessed are shown
			// as '_' (underscore). A space is used between each
			// letter to make it easier to read.
			Console.WriteLine();
			Console.WriteLine(string.Join(" ", correctLetters));
			Console.WriteLine();

			// Print out message about for example wrongly guess letter
			if(!string.IsNullOrWhiteSpace(message))
			{
				Console.WriteLine(message);
				Console.WriteLine();
			}
		}


		// Get guess from user
		private static string GetGuessFromUser(string message)
		{
			string guess;

			// Keep trying to get a get valid guess from user until
			// a guess with atleast one letter, and only letters is
			// recieved from the user
			do
			{
				Console.Write(message);
				guess = Console.ReadLine().Trim();

			} while(string.IsNullOrWhiteSpace(guess) || !guess.All(char.IsLetter));

			return guess;
		}

		// Print out the hangman-figure with regard to how many
		// wrong guesses the user has done.
		private static void PrintHangman(int numberOfWrongGuesses)
		{

			// The hangman figure 
			//
			//    ________      0
			//    │ /    |      1
			//    │/     O      2
			//    │     \│/     3
			//    │      │      4
			//    │     / \     5
			//    │             6
			//────┴───────────  7

			// Create the different lines depending on how many wrong guesses
			// have been done.
			string line0, line1, line2, line3, line4, line5, line6, line7;

			// The ground is always present
			//if(numberOfWrongGuesses >= 0)
			//{
			line0 = line1 = line2 = line3 = line4 = line5 = line6 = "";
			line7 = "────────────────";
			//}

			// A pole is added
			if(numberOfWrongGuesses >= 1)
			{
				line0 = "";
				line1 = line2 = line3 = line4 = line5 = line6 = "    │";
				line7 = "────┴───────────";
			}

			// A beam is added
			if(numberOfWrongGuesses >= 2)
			{
				line0 = "    ________";
			}

			// A support is added
			if(numberOfWrongGuesses >= 3)
			{
				line1 += " /";
				line2 += "/";
			}

			// A rope is added
			if(numberOfWrongGuesses >= 4)
			{
				line1 += "    |";
			}

			// A head is added
			if(numberOfWrongGuesses >= 5)
			{
				line2 += "     O";
			}

			// A body is added
			if(numberOfWrongGuesses >= 6)
			{
				line3 += "      │";
				line4 += "      │";
			}

			// A left arm is added
			if(numberOfWrongGuesses >= 7)
			{
				line3 = "    │     \\│";
			}

			// A right arm is added
			if(numberOfWrongGuesses >= 8)
			{
				line3 += "/";
			}

			// A left leg is added
			if(numberOfWrongGuesses >= 9)
			{
				line5 += "     /";
			}

			// A right leg is added
			// (Game is lost, the person has been hanged.)
			if(numberOfWrongGuesses >= 10)
			{
				line5 += " \\";
			}

			// Print the figure
			Console.WriteLine(
				$"{line0}\n" +
				$"{line1}\n" +
				$"{line2}\n" +
				$"{line3}\n" +
				$"{line4}\n" +
				$"{line5}\n" +
				$"{line6}\n" +
				$"{line7}");



		}

	}
}
