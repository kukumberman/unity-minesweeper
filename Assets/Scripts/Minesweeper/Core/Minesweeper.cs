using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Game.Utilities;
using Kukumberman.Shared;

namespace Kukumberman.Minesweeper.Core
{
    public sealed class MinesweeperGame
    {
        private Grid2D _grid;
        private MinesweeperCell[] _cells;

        private int _bombCount;
        private Random _random;

        public MinesweeperCell[] CellsRef => _cells;
        public int Width => _grid.Width;
        public int Height => _grid.Height;
        public int BombCount => _bombCount;
        public Grid2D Grid => _grid;

        public MinesweeperGame(MinesweeperGameSettings settings)
        {
            _grid = new Grid2D(settings.Width, settings.Height);
            _cells = new MinesweeperCell[settings.Width * settings.Height];

            _bombCount = settings.BombCount;

            FillEmptyCells();
        }

        public void Play(int seed = 0)
        {
            _random = new Random(seed);
            FillRandomBombs();
        }

        private void FillEmptyCells()
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                _grid.ConvertTo2D(i, out var x, out var y);

                _cells[i] = new MinesweeperCell()
                {
                    Index = i,
                    X = x,
                    Y = y,
                    BombNeighborCount = 0,
                    IsBomb = false,
                    IsFlag = false,
                    IsRevealed = false
                };
            }
        }

        private void FillRandomBombs()
        {
            _cells.Shuffle(_random);

            for (int i = 0; i < _bombCount; i++)
            {
                _cells[i].IsBomb = true;
            }

            Array.Sort(_cells, CellComparisonByIndex);

            var indexes = new List<int>(8);

            for (int i = 0; i < _cells.Length; i++)
            {
                indexes.Clear();
                ref var cell = ref _cells[i];
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

    [StructLayout(LayoutKind.Sequential)]
    public struct MinesweeperCell
    {
        public int Index;
        public int X;
        public int Y;
        public int BombNeighborCount;
        public bool IsBomb;
        public bool IsFlag;
        public bool IsRevealed;
    }

    [Serializable]
    public sealed class MinesweeperGameSettings
    {
        public int Width;
        public int Height;
        public int BombCount;
    }

    public enum EMinesweeperState
    {
        None,
        Playing,
        Win,
        Defeat,
    }
}
