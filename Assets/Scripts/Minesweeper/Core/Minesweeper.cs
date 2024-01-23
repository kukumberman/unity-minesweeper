using System;
using System.Collections.Generic;
using System.Linq;
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

        private int[] _enumerableRange;

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

            _enumerableRange = Enumerable.Range(0, _cells.Length).ToArray();

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
            Array.Sort(_enumerableRange);
            _enumerableRange.Shuffle(_random);

            for (int i = 0; i < _bombCount; i++)
            {
                var idx = _enumerableRange[i];
                _cells[idx].IsBomb = true;
            }

            var indexes = new List<int>(_cells.Length);

            for (int i = 0; i < _cells.Length; i++)
            {
                indexes.Clear();
                var cell = _cells[i];
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
    }

    public sealed class MinesweeperCell
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
}
