using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Kukumberman.Minesweeper.Core;
using Kukumberman.Shared;
using Random = System.Random;

public sealed class MinesweeperBurstService : IMinesweeperService
{
    public event Action OnStateChanged;

    public int CellCount => _cells.Length;

    public int BombCount => _settings.BombCount;

    public int Width => _settings.Width;

    public int Height => _settings.Height;

    public EMinesweeperState State => _state;

    private NativeList<MinesweeperCell> _cells;

    private MinesweeperGameSettings _settings;
    private int _seed;

    private EMinesweeperState _state;

    private int _total;
    private Grid2D _grid;
    private Random _random;

    public ref MinesweeperCell CellAt(int index)
    {
        return ref _cells.ElementAt(index);
    }

    public void Dispose()
    {
        if (_cells.IsCreated)
        {
            _cells.Dispose();
        }
    }

    public void StartGame(MinesweeperGameSettings settings, int seed)
    {
        _settings = settings;
        _seed = seed;
        _random = new Random(_seed);

        _state = EMinesweeperState.Playing;

        _grid = new Grid2D(Width, Height);

        if (_cells.IsCreated)
        {
            _cells.Dispose();
        }

        _total = settings.Width * settings.Height;
        _cells = new NativeList<MinesweeperCell>(_total, Allocator.Persistent);

        FillEmptyCells();
        FillRandomBombs();
    }

    public void Restart()
    {
        StartGame(_settings, _seed);

        DispatchStateChangedEvent();
    }

    public void RevealCell(int index)
    {
        ref var cell = ref CellAt(index);

        if (cell.IsFlag)
        {
            return;
        }

        if (cell.IsRevealed)
        {
            return;
        }

        cell.IsRevealed = true;

        if (cell.IsBomb)
        {
            _state = EMinesweeperState.Defeat;

            RevealBombs();

            return;
        }
        else if (cell.BombNeighborCount == 0)
        {
            FloodFillViaJob(cell);
        }

        CheckIfWin();
    }

    public void FlagCell(int index)
    {
        ref var cell = ref CellAt(index);

        if (!cell.IsRevealed)
        {
            cell.IsFlag = !cell.IsFlag;
        }

        CheckIfWin();
    }

    [Obsolete(
        "Burst is supposed to be used for JobSystem only, no performance gain by using this method"
    )]
    private void FloodFill(MinesweeperCell cell)
    {
        var visited = new NativeHashSet<int2>(0, Allocator.Temp);

        var queue = new NativeQueue<int2>(Allocator.Temp);

        queue.Enqueue(new int2(cell.X, cell.Y));

        var indexes = new List<int>();

        while (queue.Count > 0)
        {
            var position = queue.Dequeue();

            if (visited.Contains(position))
            {
                continue;
            }

            visited.Add(position);

            var cellIndex = _grid.ConvertTo1D(position.x, position.y);
            ref var nextCell = ref CellAt(cellIndex);

            if (nextCell.BombNeighborCount == 0)
            {
                _grid.GetNeighboursNonAlloc(nextCell.X, nextCell.Y, indexes);
            }

            foreach (var index in indexes)
            {
                ref var neighborCell = ref CellAt(index);
                queue.Enqueue(new int2(neighborCell.X, neighborCell.Y));
                neighborCell.IsRevealed = true;
                neighborCell.IsFlag = false;
            }

            indexes.Clear();
        }

        visited.Dispose();
        queue.Dispose();
    }

    private void FloodFillViaJob(MinesweeperCell cell)
    {
        var indexes = new NativeList<int>(0, Allocator.TempJob);

        var job = new FloodFillJob()
        {
            StartPosition = new int2(cell.X, cell.Y),
            Grid = new GridUtility2D { Width = Width, Height = Height, },
            Cells = _cells,
            Result = indexes,
        };
        var jobHandle = job.Schedule();
        jobHandle.Complete();

        for (int i = 0, length = indexes.Length; i < length; i++)
        {
            var idx = indexes[i];
            ref var neighborCell = ref _cells.ElementAt(idx);
            neighborCell.IsRevealed = true;
            neighborCell.IsFlag = false;
        }

        indexes.Dispose();
    }

    private void RevealBombs()
    {
        for (int i = 0, length = CellCount; i < length; i++)
        {
            ref var cell = ref CellAt(i);

            if (cell.IsBomb && !cell.IsFlag)
            {
                cell.IsRevealed = true;
            }
        }
    }

    private void CheckIfWin()
    {
        if (IsAboutToWinViaJobParallel())
        {
            _state = EMinesweeperState.Win;
        }
    }

    [Obsolete]
    private bool IsAboutToWin()
    {
        var length = CellCount;

        var revealedCount = 0;

        var bombFlagCount = 0;
        var totalFlagCount = 0;

        for (int i = 0; i < length; i++)
        {
            ref var cell = ref CellAt(i);

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

    [Obsolete]
    private bool IsAboutToWinViaJob()
    {
        var job = new IsAboutToWinJob()
        {
            BombCount = BombCount,
            Cells = _cells,
            Result = false,
        };
        var jobHandle = job.Schedule();
        jobHandle.Complete();

        return job.Result;
    }

    private bool IsAboutToWinViaJobParallel()
    {
        var length = CellCount;

        var job = new CalculateFlagsJob()
        {
            Cells = _cells,
            CountOfRevealed = 0,
            CountOfBombsWithFlag = 0,
            CountOfFlags = 0,
        };
        var jobHandle = job.Schedule(_cells.Length, 4);
        jobHandle.Complete();

        if (length - job.CountOfRevealed == BombCount)
        {
            return true;
        }

        if (job.CountOfBombsWithFlag == BombCount && job.CountOfFlags == BombCount)
        {
            return true;
        }

        return length == (job.CountOfRevealed + job.CountOfBombsWithFlag);
    }

    private void DispatchStateChangedEvent()
    {
        OnStateChanged.SafeInvoke();
    }

    private void FillEmptyCells()
    {
        for (int i = 0; i < _total; i++)
        {
            _grid.ConvertTo2D(i, out var x, out var y);

            var cell = new MinesweeperCell()
            {
                Index = i,
                X = x,
                Y = y,
                BombNeighborCount = 0,
                IsBomb = false,
                IsFlag = false,
                IsRevealed = false
            };

            _cells.Add(in cell);
        }
    }

    private void FillRandomBombs()
    {
        _cells.ShuffleList(_random);

        for (int i = 0; i < BombCount; i++)
        {
            ref var cell = ref CellAt(i);
            cell.IsBomb = true;
        }

        var comparer = Comparer<MinesweeperCell>.Create(CellComparisonByIndex);
        _cells.Sort(comparer);

        var indexes = new List<int>(8);

        for (int i = 0; i < _cells.Length; i++)
        {
            indexes.Clear();
            ref var cell = ref CellAt(i);
            _grid.GetNeighboursNonAlloc(cell.X, cell.Y, indexes);

            for (int j = 0; j < indexes.Count; j++)
            {
                var idx = indexes[j];
                if (_cells[idx].IsBomb)
                {
                    cell.BombNeighborCount += 1;
                }
            }
        }
    }

    private static int CellComparisonByIndex(MinesweeperCell lhs, MinesweeperCell rhs)
    {
        return lhs.Index - rhs.Index;
    }
}
