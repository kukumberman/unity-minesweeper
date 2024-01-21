using Game;
using UnityEngine;
using Kukumberman.Minesweeper.States;

namespace Kukumberman.Minesweeper
{
    public sealed class MinesweeperInitialStateBehavirour : MonoBehaviour
    {
        [SerializeField]
        private GameStartBehaviour _gameStart;

        private void Awake()
        {
            _gameStart.InitialStateFunc = () =>
            {
                return new GameInitializeState();
            };
        }
    }
}
