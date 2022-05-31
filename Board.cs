using System;
using System.Collections.Generic;

namespace Life_Console
{
    /**
     * Represents the state of the board in the game of life.
     */
    public class Board
    {
        static Random rnd = new Random();
        private bool stagnated;
        private bool clownVomit;

        private HashSet<Board> boardStateHashes = new HashSet<Board>();
        public bool Stagnated { get { return this.stagnated; } }
        public bool ClownVomit { get { return this.clownVomit; } }

        /**
         * Defines whether or not a given cell contains an organism.
         */
        public bool[][] State { get; }

        public int RowCount => State.Length;

        public int ColumnCount => State[0].Length;

        public Board(bool[][] state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (state.Length == 0 || state[0].Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(state), "The board dimensions (y and x) have to be larger than 0!");
            }
            this.stagnated = false;
            State = state;
        }


        public Board(int height, int width, bool clownVomitEnabled = false, int offset = 0, int populationPercentage = 0)
        {
            if (height == 0 || width == 0 || populationPercentage < 0 || offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height) + " | " + nameof(width) + " | " + nameof(populationPercentage), "The board dimensions (y and x) have to be larger than 0, and pop at least 0!");
            }

            this.clownVomit = clownVomitEnabled;
            this.stagnated = false;
            this.State = this.GenerateRandomizedBoard(height, width, offset, populationPercentage);
        }


        /// <summary>
        /// Generates the board, as well as decides which cells are alive.
        /// </summary>
        /// <param name="height">Height of the board</param>
        /// <param name="width">Width of the board</param>
        /// <param name="populationPercentage">Percentage of Living Squares</param>
        /// <returns>2D Jagged Bool Array, filled with TRUEs and FALSEs</returns>
        private bool[][] GenerateRandomizedBoard(int height, int width, int offset, int populationPercentage)
        {
            bool[][] generatedBoard = new bool[height][];
            for (int y = 0; y < height; y++)
            {
                generatedBoard[y] = new bool[width];
                for (int x = 0; x < width; x++)
                {
                    // If the coordinate falls within the offset border, the cell is Dead.
                    if(x < offset || x >= width - offset || y < offset || y >= height - offset)
                    {
                        generatedBoard[y][x] = false;
                    }
                    else
                    {
                        // If the random returns a value less than or equal to the populationPercentage, the cell is Alive
                        generatedBoard[y][x] = rnd.Next(1, 101) <= populationPercentage;
                    }
                }
            }
            return generatedBoard;
        }

        /// <summary>
        /// Finds the coordinates of the cells that are to die or become alive.
        /// </summary>
        /// <returns>List of coordinates of cells that are to "flip"</returns>
        public List<Coordinate> FindFlips()
        {
            bool cellStatus;
            int livingAdjacents;
            List<Coordinate> flipList = new List<Coordinate>();

            // Note: i = row = y, j = column = x
            for (int y = 0; y < this.State.GetLength(0); y++)
            {
                for (int x = 0; x < this.State[y].GetLength(0); x++)
                {
                    cellStatus = this.State[y][x];
                    livingAdjacents = CountLivingAdjacents(new Coordinate(y, x));


                    // If conditions are met, changes are queued for application.
                    if (cellStatus && (livingAdjacents < 2 || livingAdjacents > 3) || !cellStatus && livingAdjacents == 3)
                    {
                        // Living cells die if there are too few adjacent, or too many. Dead cells revive if there are exactly 3 adjacent living.
                        flipList.Add(new Coordinate(y, x));
                    }
                }
            }

            return flipList;
        }

        /// <summary>
        /// "Flips" living and dead cells to their opposite, according to the flipList Coordinates
        /// </summary>
        /// <param name="flipList">List of Coordinates of cells to change State</param>
        public void ApplyFlips(List<Coordinate> flipList)
        {
            foreach(Coordinate c in flipList)
            {
                this.State[c.Y][c.X] = !this.State[c.Y][c.X];
            }
        }

        /// <summary>
        /// Counts how many living adjacents cells a cell has.
        /// </summary>
        /// <param name="c">Coordinate of the Cell to be checked.</param>
        /// <returns>How many living neighbours it has</returns>
        private int CountLivingAdjacents(Coordinate c)
        {
            int livingAdjacents = 0;

            // These two nested for-loops give the relative coordinates for all possible neighbours
            for (int y = -1; y < 2; y++)
            {
                // IF: It doesn't go out the top and doesn't go out the bottom of the board on the Row Axis...
                if (c.Y + y >= 0 && c.Y + y < this.State.GetLength(0))
                { 
                    for (int x = -1; x < 2; x++)
                    {
                        if (
                            !(y == 0 && x == 0) && // Exlcuding itself
                            c.X + x >= 0 && // It doesn't go out the left side of the board on the Column axis
                            c.X + x < this.State[c.Y].GetLength(0) && // It doesn't go out the right side of the board on the Column axis
                            this.State[c.Y + y][c.X + x] // It is alive (True)
                            )
                        {
                            livingAdjacents++;
                        }
                    }
                }
            }
            return livingAdjacents;
        }

        /// <summary>
        /// Counts the number of living cells on the board.
        /// </summary>
        /// <returns>Number of living cells.</returns>
        public int CountTotalLivingSquares()
        {
            int count = 0;

            for (int i = 0; i < this.State.Length; i++)
            {
                for (int j = 0; j < this.State[0].Length; j++)
                {
                    if (this.State[i][j]) count++;
                }
            }

            return count;
        }


        protected bool Equals(Board other)
        {
            for (int rowNo = 0; rowNo < RowCount; rowNo++)
            {
                for (int colNo = 0; colNo < ColumnCount; colNo++)
                {
                    if (this.State[rowNo][colNo] != other.State[rowNo][colNo])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Board)obj);
        }

        public static bool operator ==(Board left, Board right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Board left, Board right)
        {
            return !Equals(left, right);
        }


        public override int GetHashCode()
        {
            int hashCode = 0;

            foreach (bool[] row in State)
            {
                foreach (bool cell in row)
                {
                    hashCode = hashCode * 2 + (cell ? 1 : 0);
                }
            }
            return hashCode;
        }

        public void Hash()
        {
            Board hashBoard = this.Clone();
            if (this.boardStateHashes.Contains(hashBoard))
            {
                this.stagnated = true;
            }
            this.boardStateHashes.Add(hashBoard);
        }


        /// <summary>
        /// Creates a clone of this board.
        /// </summary>
        /// <returns>The Clone</returns>
        public Board Clone()
        {
            bool[][] clonedArray = new bool[this.State.GetLength(0)][];
            for (int i = 0; i < this.State.GetLength(0); i++)
            {
                clonedArray[i] = (bool[])this.State[i].Clone();
            }

            return new Board(clonedArray);
        }
    }
}
