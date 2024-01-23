using Injection;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kukumberman.Minesweeper.Core
{
    [Injectable(typeof(IMinesweeperService))]
    public sealed class MinesweeperService : MonoBehaviour, IMinesweeperService
    {
        public event Action OnStateChanged;

        private MinesweeperGame _game;
        private EMinesweeperState _state;

        private MinesweeperGameSettings _settings;
        private int _seed;

        public MinesweeperGame Game => _game;

        public EMinesweeperState State => _state;

        [ContextMenu(nameof(RevealAll))]
        private void RevealAll()
        {
            for (int i = 0, length = _game.CellsRef.Length; i < length; i++)
            {
                var cell = _game.CellsRef[i];

                cell.IsRevealed = true;
            }

            DispatchStateChangedEvent();
        }

        public void StartGame(MinesweeperGameSettings settings, int seed)
        {
            _settings = settings;
            _seed = seed;

            _game = new MinesweeperGame(settings);
            _game.Play(seed);

            _state = EMinesweeperState.Playing;
        }

        public void Restart()
        {
            StartGame(_settings, _seed);

            DispatchStateChangedEvent();
        }

        public void RevealCell(int index)
        {
            var cell = _game.CellsRef[index];

            if (cell.IsFlag)
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
                FloodFill(cell);
            }

            CheckIfWin();
        }

        public void FlagCell(int index)
        {
            var cell = _game.CellsRef[index];

            if (!cell.IsRevealed)
            {
                cell.IsFlag = !cell.IsFlag;
            }

            CheckIfWin();
        }

        private void FloodFill(MinesweeperCell cell)
        {
            var visited = new HashSet<Vector2Int>();

            var queue = new Queue<Vector2Int>();

            queue.Enqueue(new Vector2Int(cell.X, cell.Y));

            var indexes = new List<int>();

            var grid = _game.Grid;

            while (queue.Count > 0)
            {
                var position = queue.Dequeue();

                if (visited.Contains(position))
                {
                    continue;
                }

                visited.Add(position);

                var cellIndex = grid.ConvertTo1D(position.x, position.y);
                var nextCell = _game.CellsRef[cellIndex];

                if (nextCell.BombNeighborCount == 0)
                {
                    grid.GetNeighboursNonAlloc(nextCell.X, nextCell.Y, indexes);
                }

                foreach (var index in indexes)
                {
                    var neighborCell = _game.CellsRef[index];
                    queue.Enqueue(new Vector2Int(neighborCell.X, neighborCell.Y));
                    neighborCell.IsRevealed = true;
                }

                indexes.Clear();
            }
        }

        private void RevealBombs()
        {
            for (int i = 0, length = _game.CellsRef.Length; i < length; i++)
            {
                var cell = _game.CellsRef[i];

                if (cell.IsBomb && !cell.IsFlag)
                {
                    cell.IsRevealed = true;
                }
            }
        }

        private void CheckIfWin()
        {
            if (IsAboutToWin())
            {
                _state = EMinesweeperState.Win;
            }
        }

        private bool IsAboutToWin()
        {
            var length = _game.CellsRef.Length;

            var revealedCount = 0;

            var bombFlagCount = 0;
            var totalFlagCount = 0;

            for (int i = 0; i < length; i++)
            {
                var cell = _game.CellsRef[i];

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

            if (length - revealedCount == _game.BombCount)
            {
                return true;
            }

            if (bombFlagCount == _game.BombCount && totalFlagCount == _game.BombCount)
            {
                return true;
            }

            return length == (revealedCount + bombFlagCount);
        }

        private void DispatchStateChangedEvent()
        {
            OnStateChanged.SafeInvoke();
        }
    }
}
