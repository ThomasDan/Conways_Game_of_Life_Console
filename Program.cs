using System;
using System.Collections.Generic;

namespace Life_Console
{
    public class Program
    {
        static CLI cli = new CLI();
        static void Main(string[] args)
        {
            string action = "";
            while (action != "quit")
            {
                action = cli.RunMainMenu();
                switch (action)
                {
                    case "custom":
                        Setup();
                        break;
                    case "tumble":
                        TumblerTest();
                        break;
                }
            }
        }

        /// <summary>
        /// Setup your own board!
        /// </summary>
        static void Setup()
        {
            int width = cli.IntegerGetter("\nSpecify the board dimensions!\nWidth?");
            int height = cli.IntegerGetter("\nAnd height?");
            bool randomizedPopulation = cli.BoolGetter("\nRandomized population?");
            int offset = randomizedPopulation ? 
                    cli.IntegerGetter(
                        "\nOffset? - To create an initial border of dead space", // Question to display
                        0, // minimum value
                        (width < height ? width : height) / 2 // Maximum value
                        )
                    : 0;
            int population = randomizedPopulation ? cli.IntegerGetter("\nPopulation Percentage? 0-100", 0, 100) : 0;
            
            bool clownVomit = cli.BoolGetter("\nEnable Clown Vomit?\nIt's.. uh.. Special..");

            Board board = new Board(height, width, clownVomit, offset, population);

            if(!randomizedPopulation) EditBoard(board);

            cli.WriteLine("\n---------------------- B O A R D ------------------------\n");
            cli.DisplayBoard(board);

            GameLoop(board, cli.IntegerGetter("How many rounds do you want the game to progress per calculation? (Skipped rounds will still be displayed)", 1, 500));
        }

        /// <summary>
        /// Opens a special menu, where you can flip the individual cells of the board.
        /// </summary>
        /// <param name="board"></param>
        static void EditBoard(Board board)
        {
            string action = "";
            Coordinate coordinate = new Coordinate(-1, -1);

            while (action != "done")
            {
                action = cli.RunEditBoardMenu(board);
                if(int.TryParse(action, out int potentialX) && potentialX > 0 && potentialX <= board.RowCount)
                {
                    coordinate.X = potentialX - 1;
                    coordinate.Y = cli.IntegerGetter("Column = " + potentialX + ", Row = ..?", 1, board.ColumnCount) - 1;
                    board.State[coordinate.Y][coordinate.X] = !board.State[coordinate.Y][coordinate.X];
                }
            }
        }


        /// <summary>
        /// Runs a tumbler. Tests the Stagnation Detector, GameLoop() and AdvanceLifeCycle()
        /// </summary>
        static void TumblerTest()
        {
            Console.WriteLine("\n\n################# Tumbler test mode, engaged! #####################\n\nThe TUmbler is an infinitely repeating arrangement, used for testing the Stagnation Detection™ system.\n");

            // 7x9
            Board testBoard = new Board(
                new bool[7][]
                {
                    new bool[]{ false, true, false, false, false, false, false, true, false },
                    new bool[]{ true, false, true, false, false, false, true, false, true },
                    new bool[]{ true, false, false, true, false, true, false, false, true },
                    new bool[]{ false, false, true, false, false, false, true, false, false },
                    new bool[]{ false, false, true, true, false, true, true, false, false },
                    new bool[]{ false, false, false, false, false, false, false, false, false },
                    new bool[]{ false, false, false, false, false, false, false, false, false }
                });
            GameLoop(testBoard, 1);
        }

        /// <summary>
        /// The game loop. Runs until stagnation is detected or you type "Exit"
        /// </summary>
        /// <param name="board"></param>
        /// <param name="progression"></param>
        static void GameLoop(Board board, int progression)
        {
            int round = 0;
            string action = "";

            // Game loop
            while (action != "exit" && !board.Stagnated)
            {
                for (int i = 0; i < progression; i++)
                {
                    round++;
                    AdvanceLifeCycle(board);
                    if (board.Stagnated) i = progression;
                    cli.DisplayRoundResult(round, board);
                }
                cli.DisplayPostProgressionOptions(board.Stagnated);
                action = cli.ReadLineLowered();
            }
        }

        /// <summary>
        /// Advances the board one life-cycle
        /// </summary>
        /// <param name="board"></param>
        static void AdvanceLifeCycle(Board board)
        {
            List<Coordinate> flipList = board.FindFlips();
            board.ApplyFlips(flipList);
            board.Hash();
        }
    }
}