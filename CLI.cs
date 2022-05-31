using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life_Console
{
    internal class CLI
    {
        static Random rnd = new Random();

        internal string RunMainMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n############   C O N W A Y ' S   G A M E   O F   L I F E   ############\n\n\nBy Thomas C. A. Dänhardt  |  25/03-22 - Version 3: Revenge Of The Refactored!\n\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("What would you like to do?\n-Custom\n-Tumble\n-Quit");
            return this.ReadLineLowered();
        }

        internal string RunEditBoardMenu(Board board)
        {
            Console.Clear();
            this.DisplayBoardEdit(board);
            Console.WriteLine("\nType \"Done\" to quit, otherwise type a column (x) number");
            return this.ReadLineLowered();
        }

        /// <summary>
        /// Used to display the board for the purpose of editing it for a non-randomized game.
        /// </summary>
        /// <param name="board"></param>
        private void DisplayBoardEdit(Board board)
        {

            Console.WriteLine("\n     -==========-   E D I T   B O A R D   -==========-");
            int rowDigits = board.RowCount.ToString().Length;
            int columnDigits = board.ColumnCount.ToString().Length;
            if (columnDigits < 2) columnDigits = 2; // The digit count may just be 1 from "5" or "9", but the graphic cells are 2 wide.

            for (int i = 0; i < rowDigits+1; i++)
            {
                Console.Write(" ");
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;


            for (int j = 0; j < board.ColumnCount; j++)
            {
                // Max Column Digits (i.e. 4 for 1000-9999, 3 for 100-999) ... +1 for space separation ... minus the current number of digits
                for (int k = 0; k < columnDigits + 1 - (j + 1).ToString().Length; k++)
                {
                    Console.Write(" ");
                }
                Console.Write(j+1);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            for (int y = 0; y < board.RowCount; y++)
            {
                // Here we write the number, and proper spacing, of the row-count on the left.
                for (int l = 0; l < rowDigits + 1 - (y+1).ToString().Length; l++)
                {
                    Console.Write(" ");
                }

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(y+1);
                Console.ForegroundColor = ConsoleColor.White;

                // On even-numbered rows, the cells will be Gray, to help differentiate the rows.
                if (y % 2 == 0) Console.ForegroundColor = ConsoleColor.Gray;

                for (int x = 0; x < board.ColumnCount; x++)
                {
                    for (int m = 0; m < columnDigits - 1; m++) // +1 - 2 for the characters ▓▓ or ░░
                    {
                        Console.Write(" ");
                    }

                    if (board.State[y][x])
                    {
                        Console.Write("▓▓");
                    }
                    else
                    {
                        Console.Write("░░");
                    }
                }
                Console.WriteLine();
            }
        }

        internal void DisplayRoundResult(int round, Board board)
        {
            Console.WriteLine("\n-------------------------- R O U N D " + round + " --------------------------\n");
            this.DisplayBoard(board);
        }

        /// <summary>
        /// Used after every life-cycle advancement.
        /// </summary>
        /// <param name="stagnant"></param>
        internal void DisplayPostProgressionOptions(bool stagnant)
        {
            if (stagnant)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n#####################################################################\n############### S T A G N A T I O N   D E T E C T E D ###############\n###############      . . . Q U I T T I N G . . .      ###############\n#####################################################################\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine("\nPress Enter to Continue, or type \"Exit\"");
        }


        /// <summary>
        /// Safely acquires a valid integer between, by default, 0 and 10.000
        /// </summary>
        /// <returns>Input from user</returns>
        internal int IntegerGetter(string userQuery, int minVal = 0, int maxVal = 10000)
        {
            int output;
            Console.WriteLine(userQuery);
            while (!int.TryParse(Console.ReadLine(), out output) || output < minVal || output > maxVal)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Unable to convert to number, or number not between " + minVal + " and " + maxVal + "!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            return output;
        }

        /// <summary>
        /// Acquires a bool input from the user.
        /// </summary>
        /// <param name="question">The question which requires a Yes/No input from the user.</param>
        /// <returns>Bool: "Y" = true, "N" = false</returns>
        internal bool BoolGetter(string question)
        {
            Console.WriteLine(question + "\nYes or No");
            while (true)
            {
                string selection = this.ReadLineLowered();
                if (selection == "y" || selection == "yes") return true;
                else if (selection == "n" || selection == "no") return false;
                Console.WriteLine("Please type \"Yes\", \"Y\", \"No\" or \"N\". Capitalization optional.");
            }
        }

        internal void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        internal string ReadLineLowered()
        {
            return Console.ReadLine().ToLower();
        }

        /// <summary>
        /// Translates the bools into a visible board.
        /// </summary>
        /// <param name="display_board">Takes a 2D bool jagged array</param>
        internal void DisplayBoard(Board board)
        {
            for (int i = 0; i < board.State.GetLength(0); i++)
            {
                for (int j = 0; j < board.State[i].GetLength(0); j++)
                {
                    if (board.State[i][j])
                    {
                        if (board.ClownVomit)
                        {
                            Console.ForegroundColor = ClownVomit(i + j);
                            Console.Write("▓▓");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else Console.Write("▓▓");
                    }
                    else
                    {
                        Console.Write("░░");
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Funny clown colouring with slight randomness.
        /// </summary>
        /// <param name="ij">The combined coordinate values (x/y)</param>
        /// <returns>a very funny clown colour</returns>
        private ConsoleColor ClownVomit(int ij)
        {
            ConsoleColor colour;
            switch ((ij + rnd.Next(-1, 2)) % 7)
            {
                case 0:
                    colour = ConsoleColor.Red;
                    break;
                case 1:
                    colour = ConsoleColor.DarkRed;
                    break;
                case 2:
                    colour = ConsoleColor.DarkYellow;
                    break;
                case 3:
                    colour = ConsoleColor.DarkGreen;
                    break;
                case 4:
                    colour = ConsoleColor.DarkCyan;
                    break;
                case 5:
                    colour = ConsoleColor.DarkBlue;
                    break;
                case 6:
                    colour = ConsoleColor.DarkMagenta;
                    break;
                default:
                    colour = ConsoleColor.Magenta;
                    break;
            }
            return colour;
        }
    }
}
