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

        public MinesweeperGame Game => _game;

        [ContextMenu(nameof(RevealAll))]
        private void RevealAll()
        {
            for (int i = 0, length = _game.CellsRef.Length; i < length; i++)
            {
                var cell = _game.CellsRef[i];

                cell.IsRevealed = true;
            }

            OnStateChanged.SafeInvoke();
        }

        public void StartGame(MinesweeperGameSettings settings)
        {
            _game = new MinesweeperGame(settings);
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
                Debug.Log("todo: cell.IsBomb");
            }
            else if (cell.BombNeighborCount == 0)
            {
                FloodFill(cell);
            }
        }

        public void FlagCell(int index)
        {
            var cell = _game.CellsRef[index];

            if (!cell.IsRevealed)
            {
                cell.IsFlag = !cell.IsFlag;
            }
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
    }
}
