using System.Collections.Generic;
using Tile;

namespace Utilities
{
    public static class Utility
    {
        public static List<TileDef> SurroundingTiles(int x, int y, ref List<TileDef> tileDef)
        {
            var topTile = new TileDef(x, y - 1);
            var bottomTile = new TileDef(x, y + 1);
            var leftTile = new TileDef(x - 1, y);
            var rightTile = new TileDef(x + 1, y);

            return new List<TileDef> { topTile, bottomTile, leftTile, rightTile };
        }
    }
}