using System.Collections.Generic;

namespace Kukumberman.Shared
{
    public sealed class Grid2D
    {
        private readonly int _width;
        private readonly int _height;

        public int Width => _width;
        public int Height => _height;

        public Grid2D(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public List<int> GetNeighbours(int x, int y)
        {
            var list = new List<int>();
            GetNeighboursNonAlloc(x, y, list);
            return list;
        }

        public void GetNeighboursNonAlloc(int x, int y, List<int> indexes)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    var xx = x + i;
                    var yy = y + j;
                    if (IsInside(xx, yy))
                    {
                        var index = ConvertTo1D(xx, yy);
                        indexes.Add(index);
                    }
                }
            }
        }

        public bool IsInside(int x, int y)
        {
            var xx = x >= 0 && x < _width;
            var yy = y >= 0 && y < _height;
            return xx && yy;
        }

        public int ConvertTo1D(int x, int y)
        {
            return y * _width + x;
        }

        public bool ConvertTo2D(int index, out int x, out int y)
        {
            x = index % _width;
            y = index / _width;
            return IsInside(x, y);
        }
    }
}
