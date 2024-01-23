using System;

namespace Kukumberman.Minesweeper.Core
{
    public interface IMinesweeperService
    {
        event Action OnStateChanged;

        MinesweeperGame Game { get; }

        EMinesweeperState State { get; }

        void StartGame(MinesweeperGameSettings settings, int seed);

        void Restart();

        void RevealCell(int index);

        void FlagCell(int index);
    }
}
