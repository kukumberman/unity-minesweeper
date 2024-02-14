using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Kukumberman.Minesweeper.Core;

[BurstCompile]
public struct CalculateFlagsJob : IJobParallelFor
{
    [ReadOnly]
    public NativeList<MinesweeperCell> Cells;

    [WriteOnly]
    public int CountOfRevealed;

    [WriteOnly]
    public int CountOfBombsWithFlag;

    [WriteOnly]
    public int CountOfFlags;

    public void Execute(int index)
    {
        var cell = Cells[index];

        if (cell.IsRevealed && !cell.IsBomb)
        {
            CountOfRevealed += 1;
        }
        else if (!cell.IsRevealed && cell.IsBomb && cell.IsFlag)
        {
            CountOfBombsWithFlag += 1;
        }

        if (cell.IsFlag)
        {
            CountOfFlags += 1;
        }
    }
}
