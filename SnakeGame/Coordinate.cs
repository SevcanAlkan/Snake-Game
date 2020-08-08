using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    internal struct Coordinate
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Coordinate(int x = 0, int y = 0)
        {
            this.X = x;
            this.Y = y;
        }

        public void Update(int? x, int? y)
        {
            if (x != null && x != 0)
                this.X = x.Value;

            if (y != null && y != 0)
                this.Y = y.Value;
        }
    }
}
