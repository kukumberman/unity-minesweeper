using System;
using System.Collections.Generic;
using Core;
using Game.Core.UI;
using Game.States;
using Game.UI.Hud;
using Injection;
using UnityEngine;
using Kukumberman.Minesweeper.States;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class GameplayHudModel : Observable
    {
        public int ElapsedSeconds;
        public Vector2Int GridSize;
        public List<CellElementModel> CellModels;
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

        [Inject]
        private MinesweeperStaticDataMono _staticData;

        private GameplayHudModel _viewModel;

        protected override void Show()
        {
            _view.OnMenuButtonClicked += View_OnMenuButtonClicked;
            _view.OnCellClicked += View_OnCellClicked;

            var gridSize = new Vector2Int(5, 6);

            _viewModel = new GameplayHudModel
            {
                ElapsedSeconds = 0,
                GridSize = gridSize,
                CellModels = new List<CellElementModel>(),
            };

            for (int i = 0, length = gridSize.x * gridSize.y; i < length; i++)
            {
                var model = new CellElementModel()
                {
                    // Dima: test
                    NeighborCount = i % 9,
                    SpriteForeground = null,
                    SpriteBackground = _staticData.SpriteCellUnlocked,
                };
                _viewModel.CellModels.Add(model);
            }

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
            // Dima: test
            var cellModel = _viewModel.CellModels[index];
            cellModel.NeighborCount += 1;
            cellModel.SetChanged();
        }
    }
}
