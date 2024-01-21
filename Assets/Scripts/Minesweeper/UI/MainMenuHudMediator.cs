using System;
using Core;
using Game.Core.UI;
using Game.States;
using Game.UI.Hud;
using Injection;
using Kukumberman.Minesweeper.States;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class MainMenuHudModel : Observable
    {
        //
    }

    public interface IMainMenuHudView : IHudWithModel<MainMenuHudModel>
    {
        event Action OnPlayButtonClicked;
    }

    public sealed class MainMenuHudMediator : Mediator<IMainMenuHudView>
    {
        [Inject]
        private GameStateManager _gameStateManager;

        protected override void Show()
        {
            _view.OnPlayButtonClicked += View_OnPlayButtonClicked;
        }

        protected override void Hide()
        {
            _view.OnPlayButtonClicked -= View_OnPlayButtonClicked;
        }

        private void View_OnPlayButtonClicked()
        {
            _gameStateManager.SwitchToState(new GameplayState());
        }
    }
}
