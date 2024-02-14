using Unity.Collections;

public struct GridUtility2D
{
    public int Width;
    public int Height;

    public int ConvertTo1D(int x, int y)
    {
        return y * Width + x;
    }

    public bool ConvertTo2D(int index, out int x, out int y)
    {
        x = index % Width;
        y = index / Width;
        return IsInside(x, y);
    }

    public bool IsInside(int x, int y)
    {
        var xx = x >= 0 && x < Width;
        var yy = y >= 0 && y < Height;
        return xx && yy;
    }

    public void GetNeighboursNonAlloc(int x, int y, NativeList<int> indexes)
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
}
