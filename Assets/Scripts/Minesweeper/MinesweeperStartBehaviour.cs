using UnityEngine;
using Kukumberman.Minesweeper.Enums;

namespace Kukumberman.Minesweeper
{
    public sealed class MinesweeperStartBehaviour : MonoBehaviour
    {
        [SerializeField]
        private bool _overrideInitialState;

        [SerializeField]
        private StateType _initialState;

        public bool OverrideInitialState => Application.isEditor ? _overrideInitialState : false;

        public StateType InitialState => _initialState;
    }
}
