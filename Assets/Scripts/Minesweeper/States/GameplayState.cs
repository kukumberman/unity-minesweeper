using Game.Managers;
using Game.States;
using Injection;
using Kukumberman.Minesweeper.Core;
using Kukumberman.Minesweeper.UI;

namespace Kukumberman.Minesweeper.States
{
    public sealed class GameplayState : GameState
    {
        [Inject]
        private HudManager _hudManager;

        [Inject]
        private IMinesweeperService _service;

        private readonly MinesweeperGameSettings _settings;
        private readonly int _seed;

        public GameplayState(MinesweeperGameSettings settings, int seed)
        {
            _settings = settings;
            _seed = seed;
        }

        public override void Initialize()
        {
            _service.StartGame(_settings, _seed);
            _hudManager.ShowAdditional<GameplayHudMediator>();
        }

        public override void Dispose()
        {
            _hudManager.HideAdditional<GameplayHudMediator>();
        }
    }
}
