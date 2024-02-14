using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Kukumberman.Minesweeper.Core;

[BurstCompile]
public struct FloodFillJob : IJob
{
    [ReadOnly]
    public int2 StartPosition;

    [ReadOnly]
    public GridUtility2D Grid;

    public NativeList<MinesweeperCell> Cells;

    [WriteOnly]
    public NativeList<int> Result;

    public void Execute()
    {
        var indexes = new NativeList<int>(8, Allocator.Temp);
        var visited = new NativeHashSet<int>(0, Allocator.Temp);
        var queue = new NativeQueue<int>(Allocator.Temp);

        var startIndex = Grid.ConvertTo1D(StartPosition.x, StartPosition.y);
        queue.Enqueue(startIndex);

        while (queue.Count > 0)
        {
            var cellIndex = queue.Dequeue();

            if (visited.Contains(cellIndex))
            {
                continue;
            }

            visited.Add(cellIndex);

            ref var nextCell = ref Cells.ElementAt(cellIndex);

            if (nextCell.BombNeighborCount == 0)
            {
                Grid.GetNeighboursNonAlloc(nextCell.X, nextCell.Y, indexes);
            }

            foreach (var index in indexes)
            {
                ref var neighborCell = ref Cells.ElementAt(index);
                queue.Enqueue(index);
                Result.Add(index);
            }

            indexes.Clear();
        }

        indexes.Dispose();
        visited.Dispose();
        queue.Dispose();
    }
}
