using System;

namespace Kukumberman.Minesweeper.Core
{
    public interface IMinesweeperService
    {
        event Action OnStateChanged;

        ref MinesweeperCell CellAt(int index);

        int CellCount { get; }

        int BombCount { get; }

        int Width { get; }

        int Height { get; }

        EMinesweeperState State { get; }

        void Dispose();

        void StartGame(MinesweeperGameSettings settings, int seed);

        void Restart();

        void RevealCell(int index);

        void FlagCell(int index);
    }
}
