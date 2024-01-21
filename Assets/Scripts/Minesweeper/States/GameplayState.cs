using Game.Managers;
using Game.States;
using Injection;
using Kukumberman.Minesweeper.UI;

namespace Kukumberman.Minesweeper.States
{
    public sealed class GameplayState : GameState
    {
        [Inject]
        private HudManager _hudManager;

        public override void Initialize()
        {
            _hudManager.ShowAdditional<GameplayHudMediator>();
        }

        public override void Dispose()
        {
            _hudManager.HideAdditional<GameplayHudMediator>();
        }
    }
}
