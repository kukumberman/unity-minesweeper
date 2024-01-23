using System;
using System.Collections.Generic;
using Core;
using Game.Core;
using Game.Core.UI;
using Game.States;
using Game.UI.Hud;
using Injection;
using UnityEngine;
using Kukumberman.Minesweeper.States;
using Kukumberman.Minesweeper.Core;
using Kukumberman.Minesweeper.Enums;
using Kukumberman.Minesweeper.ScriptableObjects;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class GameplayHudModel : Observable
    {
        public int RemainingBombCount;
        public int ElapsedSeconds;
        public Vector2Int GridSize;
        public List<CellElementModel> CellModels;
    }

    public interface IGameplayHudView : IHudWithModel<GameplayHudModel>
    {
        event Action OnMenuButtonClicked;
        event Action OnRestartButtonClicked;
        event Action<int> OnCellClicked;
        event Action<int> OnCellAsFlagClicked;
    }

    public sealed class GameplayHudMediator : Mediator<IGameplayHudView>
    {
        [Inject]
        private GameStateManager _gameStateManager;

        [Inject]
        private SpriteCollectionScriptableObject _sprites;

        [Inject]
        private IMinesweeperService _service;

        [Inject]
        private Timer _timer;

        private GameplayHudModel _viewModel;

        protected override void Show()
        {
            _view.OnMenuButtonClicked += View_OnMenuButtonClicked;
            _view.OnRestartButtonClicked += View_OnRestartButtonClicked;
            _view.OnCellClicked += View_OnCellClicked;
            _view.OnCellAsFlagClicked += View_OnCellAsFlagClicked;

            _service.OnStateChanged += Service_OnStateChanged;

            _timer.ONE_SECOND_TICK += Timer_ONE_SECOND_TICK;

            _viewModel = new GameplayHudModel
            {
                RemainingBombCount = GetRemainingBombCount(),
                ElapsedSeconds = 0,
                GridSize = new Vector2Int(_service.Game.Width, _service.Game.Height),
                CellModels = new List<CellElementModel>(),
            };

            for (int i = 0, length = _service.Game.CellsRef.Length; i < length; i++)
            {
                var cell = _service.Game.CellsRef[i];

                var model = new CellElementModel()
                {
                    BombNeighborCount = 0,
                    SpriteForeground = null,
                    SpriteBackground = _sprites.Get(ESpriteType.CellLocked),
                };
                _viewModel.CellModels.Add(model);
            }

            _view.Model = _viewModel;
        }

        protected override void Hide()
        {
            _view.OnMenuButtonClicked -= View_OnMenuButtonClicked;
            _view.OnRestartButtonClicked -= View_OnRestartButtonClicked;
            _view.OnCellClicked -= View_OnCellClicked;
            _view.OnCellAsFlagClicked -= View_OnCellAsFlagClicked;

            _service.OnStateChanged -= Service_OnStateChanged;

            _timer.ONE_SECOND_TICK -= Timer_ONE_SECOND_TICK;
        }

        private void View_OnMenuButtonClicked()
        {
            _gameStateManager.SwitchToState(new MainMenuState());
        }

        private void View_OnRestartButtonClicked()
        {
            _viewModel.ElapsedSeconds = 0;
            _viewModel.SetChanged();

            _service.Restart();
        }

        private void View_OnCellClicked(int index)
        {
            _service.RevealCell(index);
            SyncState();
        }

        private void View_OnCellAsFlagClicked(int index)
        {
            _service.FlagCell(index);
            SyncState();
        }

        private void Service_OnStateChanged()
        {
            SyncState();
        }

        private void Timer_ONE_SECOND_TICK()
        {
            _viewModel.ElapsedSeconds += 1;
            _viewModel.SetChanged();
        }

        private void SyncState()
        {
            for (int i = 0; i < _service.Game.CellsRef.Length; i++)
            {
                var cell = _service.Game.CellsRef[i];
                var cellModel = _viewModel.CellModels[i];

                cellModel.SpriteBackground = cell.IsRevealed
                    ? _sprites.Get(ESpriteType.CellUnlocked)
                    : _sprites.Get(ESpriteType.CellLocked);

                if (!cell.IsRevealed && cell.IsFlag)
                {
                    cellModel.SpriteForeground = _sprites.Get(ESpriteType.Flag);
                }
                else if (cell.IsRevealed && cell.IsBomb)
                {
                    cellModel.SpriteForeground = _sprites.Get(ESpriteType.Bomb);
                }
                else
                {
                    cellModel.SpriteForeground = null;
                }

                if (cell.IsRevealed && !cell.IsBomb)
                {
                    cellModel.BombNeighborCount = cell.BombNeighborCount;
                }
                else
                {
                    cellModel.BombNeighborCount = 0;
                }

                cellModel.SetChanged();
            }

            _viewModel.RemainingBombCount = GetRemainingBombCount();
            _viewModel.SetChanged();
        }

        private int GetRemainingBombCount()
        {
            var baseCount = _service.Game.BombCount;
            var flagCount = 0;

            for (int i = 0, length = _service.Game.CellsRef.Length; i < length; i++)
            {
                var cell = _service.Game.CellsRef[i];

                if (cell.IsFlag)
                {
                    flagCount += 1;
                }
            }

            return baseCount - flagCount;
        }
    }
}
