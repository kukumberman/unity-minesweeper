using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Kukumberman.Minesweeper.Core;

[BurstCompile]
public struct IsAboutToWinJob : IJob
{
    public NativeList<MinesweeperCell> Cells;

    [ReadOnly]
    public int BombCount;

    [WriteOnly]
    public bool Result;

    public void Execute()
    {
        Result = Calculate();
    }

    private bool Calculate()
    {
        var length = Cells.Length;

        var revealedCount = 0;

        var bombFlagCount = 0;
        var totalFlagCount = 0;

        for (int i = 0; i < length; i++)
        {
            ref var cell = ref Cells.ElementAt(i);

            if (cell.IsRevealed && !cell.IsBomb)
            {
                revealedCount += 1;
            }
            else if (!cell.IsRevealed && cell.IsBomb && cell.IsFlag)
            {
                bombFlagCount += 1;
            }

            if (cell.IsFlag)
            {
                totalFlagCount += 1;
            }
        }

        if (length - revealedCount == BombCount)
        {
            return true;
        }

        if (bombFlagCount == BombCount && totalFlagCount == BombCount)
        {
            return true;
        }

        return length == (revealedCount + bombFlagCount);
    }
}
