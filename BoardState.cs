using System;

namespace BoardState_Import
{
    /**
     * Represents the state of the board in the game of life.
     */
    public class BoardState
    {
        /**
         * Defines whether or not a given cell contains an organism.
         */
        public bool[][] State { get; }

        public int RowCount => State.Length;

        public int ColumnCount => State[0].Length;

        /// <summary>
        /// Ensures that the 
        /// </summary>
        /// <param name="state"></param>
        public BoardState(bool[][] state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (state.Length == 0 || state[0].Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(state), "The board size has to be larger than 0!");
            }
            State = state;
        }

        protected bool Equals(BoardState other)
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
            return Equals((BoardState)obj);
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

        public static bool operator ==(BoardState left, BoardState right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BoardState left, BoardState right)
        {
            return !Equals(left, right);
        }
    }
}
