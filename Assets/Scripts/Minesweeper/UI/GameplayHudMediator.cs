using System;
using Core;
using Game.Core.UI;
using Game.States;
using Game.UI.Hud;
using Injection;
using Kukumberman.Minesweeper.States;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class GameplayHudModel : Observable
    {
        public int ElapsedSeconds;
    }

    public interface IGameplayHudView : IHudWithModel<GameplayHudModel>
    {
        event Action OnMenuButtonClicked;
        event Action<int> OnCellClicked;
    }

    public sealed class GameplayHudMediator : Mediator<IGameplayHudView>
    {
        [Inject]
        private GameStateManager _gameStateManager;

        private GameplayHudModel _viewModel;

        protected override void Show()
        {
            _view.OnMenuButtonClicked += View_OnMenuButtonClicked;
            _view.OnCellClicked += View_OnCellClicked;

            _viewModel = new GameplayHudModel { ElapsedSeconds = 0, };

            _view.Model = _viewModel;
        }

        protected override void Hide()
        {
            _view.OnMenuButtonClicked -= View_OnMenuButtonClicked;
            _view.OnCellClicked -= View_OnCellClicked;
        }

        private void View_OnMenuButtonClicked()
        {
            _gameStateManager.SwitchToState(new MainMenuState());
        }

        private void View_OnCellClicked(int index)
        {
            UnityEngine.Debug.Log(index);
        }
    }
}
