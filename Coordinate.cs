using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Life_Console
{
    public class Coordinate
    {
        public int Y { get; set; }
        public int X { get; set; }

        /// <summary>
        /// Creates a Coordinate
        /// </summary>
        /// <param name="y">Row | Vertical Position</param>
        /// <param name="x">Column | Horizontal Position</param>
        public Coordinate(int y, int x)
        {
            Y = y; //vertical
            X = x; //horizontal
        }

    }
}
