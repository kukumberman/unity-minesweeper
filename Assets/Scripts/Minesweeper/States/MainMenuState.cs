using Game.Managers;
using Game.States;
using Injection;
using Kukumberman.Minesweeper.UI;

namespace Kukumberman.Minesweeper.States
{
    public sealed class MainMenuState : GameState
    {
        [Inject]
        private HudManager _hudManager;

        public override void Initialize()
        {
            _hudManager.ShowAdditional<MainMenuHudMediator>();
        }

        public override void Dispose()
        {
            _hudManager.HideAdditional<MainMenuHudMediator>();
        }
    }
}
