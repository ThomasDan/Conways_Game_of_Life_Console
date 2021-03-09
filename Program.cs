using BoardState_Import;
using System;
using System.Collections.Generic;

namespace Life_Console
{
    static class Program
    {
        static bool Stagnation = false;
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            string action = "";
            while (action != "quit")
            {
                Stagnation = false;
                Console.Clear();
                Console.WriteLine("###############   G A M E   O F   L I F E   ###############\n-By Thomas C. A. Dänhardt, 23/06-20\n Version 2! Now with Stagnation Detection(TM)!\n");
                Console.WriteLine("What would you like to do?\n-Custom\n-Tumble\n-Quit");
                action = Console.ReadLine();
                if (action.ToLower() == "custom") { Setup(); }
                else if (action.ToLower() == "tumble") { Tumbler_Test(); }
            }
        }
        /// <summary>
        /// Setup your own board!
        /// </summary>
        static void Setup()
        {
            Console.WriteLine("\nSpecify the board dimensions!\nWidth?");
            int width = Integer_Getter();
            Console.WriteLine("And height?");
            int height = Integer_Getter();
            Console.WriteLine("For population, one in how many cells do you want alive? (2 means half (1/2), 3 means a third (1/3), etc.)");
            int population = Integer_Getter();

            bool[][] board = Generate_Board(height, width, population);
            Console.WriteLine("-------------------- B O A R D ----------------------\n");
            Display_Board(board);

            Console.WriteLine("How many rounds do you want the game to progress per calculation? (Skipped rounds will still be displayed)");
            Game_Loop(board, Integer_Getter());
        }
        /// <summary>
        /// The game loop. Runs until stagnation is detected or you type "Exit"
        /// </summary>
        /// <param name="board"></param>
        /// <param name="progression"></param>
        static void Game_Loop(bool[][] board, int progression)
        {
            int round = 0;
            string action = "";
            HashSet<BoardState> allBoards = new HashSet<BoardState>();

            // Game loop
            while (action.ToLower() != "exit" && !Stagnation)
            {
                for (int i = 0; i < progression; i++)
                {
                    round++;
                    board = Generation_Runner(board);
                    Console.WriteLine("----------------------- R O U N D " + round + " --------------------------\n");
                    Display_Board(board);
                    BoardState BSBoard = new BoardState(Copy_Array(board));
                    if (allBoards.Contains(BSBoard)) { Stagnation = true; break; }
                    allBoards.Add(BSBoard);
                }
                if (Stagnation)
                {
                    Console.WriteLine("\n#################################################################\n############# S T A G N A T I O N   D E T E C T E D #############\n#############              Q U I T T I N G          #############\n#################################################################\n");
                }
                Console.WriteLine("\nPress Enter to Continue, or type \"Exit\"");
                action = Console.ReadLine();
            }
        }
        /// <summary>
        /// Generates the board, as well as decides which cells are alive.
        /// </summary>
        /// <param name="height">Height of the board</param>
        /// <param name="width">Width of the board</param>
        /// <param name="population">One in X are alive</param>
        /// <returns>2D INT array, filled with 1s and 0s</returns>
        static bool[][] Generate_Board(int height, int width, int population)
        {
            bool[][] generated_board = new bool[height][];
            for (int i = 0; i < height; i++)
            {
                generated_board[i] = new bool[width];
                for (int j = 0; j < width; j++)
                {
                    // 0 = Alive, anything else (false) is Dead
                    generated_board[i][j] = (rnd.Next(population) == 0);
                }
            }
            return generated_board;
        }
        /// <summary>
        /// Translates the bools into a visible board.
        /// </summary>
        /// <param name="display_board">Takes a 2D int array</param>
        static void Display_Board(bool[][] display_board)
        {
            bool CellStatus;
            for (int i = 0; i < display_board.GetLength(0); i++)
            {
                for(int j = 0; j < display_board[i].GetLength(0); j++)
                {
                    CellStatus = display_board[i][j];
                    if(CellStatus)
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
        /// <summary>
        /// Runs a single round of Life for the board given
        /// </summary>
        /// <param name="board"></param>
        /// <returns>Updated board after a round of Life</returns>
        static bool[][] Generation_Runner(bool[][] board)
        {
            // ##################### Detection #######################
            bool CellStatus;
            int Living_Adjacents;
            List<Coordinate> Flip_List = new List<Coordinate>();

            // Note: i = row = y, j = column = x
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board[i].GetLength(0); j++)
                {
                    CellStatus = board[i][j];
                    Living_Adjacents = 0;

                    // Counting how many adjacent are alive
                    for(int y = -1; y < 2; y++)
                    {
                        for(int x = -1; x < 2; x++)
                        {
                            if(!(y == 0 && x == 0) && i+y >= 0 && i+y < board.GetLength(0) && j+x >= 0 && j+x < board[i].GetLength(0) && board[i + y][j + x])
                            {
                                Living_Adjacents++;
                            }
                        }
                    }
                    // If conditions are met, changes are queued for application.
                    if (CellStatus && (Living_Adjacents < 2 || Living_Adjacents > 3) || !CellStatus && Living_Adjacents == 3)
                    {
                        // Living cells die if there are too few adjacent, or too many. Dead cells revive if there are exactly 3 adjacent living.
                        Flip_List.Add(new Coordinate(i, j));
                    }
                }
            }

            // ###################### Application ############################
            foreach (Coordinate c in Flip_List)
            {
                if (board[c.Y][c.X])
                {
                    board[c.Y][c.X] = false;
                }
                else
                {
                    board[c.Y][c.X] = true;
                }
            }
            return board;
        }
        /// <summary>
        /// Safely acquires a valid integer
        /// </summary>
        /// <returns>Input from user</returns>
        static int Integer_Getter()
        {
            string input = Console.ReadLine();
            int output;
            while(!int.TryParse(input, out output) || output <= 0 || output >= 10000)
            {
                Console.WriteLine("Error: unable to convert to number, or number not Between 0 and 10.000!");
                input = Console.ReadLine();
            }
            return output;
        }
        /// <summary>
        /// Copies the contents of a bool[][] array. 
        /// </summary>
        /// <param name="A2C"></param>
        /// <returns></returns>
        static bool[][] Copy_Array(bool[][] A2C)
        {
            bool[][] output = new bool[A2C.GetLength(0)][];
            for (int i = 0; i < A2C.GetLength(0); i++)
            {
                output[i] = (bool[])A2C[i].Clone();
            }
            return output;
        }
        /// <summary>
        /// Runs a tumbler. Tests the Stagnation Detector, Game_Loop() and Generation_Runner()
        /// </summary>
        static void Tumbler_Test()
        {
            Console.WriteLine("\n\n################# Tumbler test mode, engaged! #####################\n\nThe TUmbler is an infinitely repeating arrangement, used for testing the Stagnation Detection(TM) system.\n");

            // 7x9
            bool[][] testboard = new bool[7][] {
                new bool[]{ false, true, false, false, false, false, false, true, false },
                new bool[]{ true, false, true, false, false, false, true, false, true },
                new bool[]{ true, false, false, true, false, true, false, false, true },
                new bool[]{ false, false, true, false, false, false, true, false, false },
                new bool[]{ false, false, true, true, false, true, true, false, false },
                new bool[]{ false, false, false, false, false, false, false, false, false },
                new bool[]{ false, false, false, false, false, false, false, false, false }
            };
            Game_Loop(testboard, 1);
        }
    }
}