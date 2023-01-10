using System.Collections.Generic;

namespace Tile
{
    public struct DestroyTiles
    {
        public int x;

        public SortedList<int, int> index;

        public DestroyTiles(int x)
        {
            this.x = x;
            index = new SortedList<int, int>();
        }
    }
}