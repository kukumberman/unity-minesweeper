using System;
using System.Collections.Generic;
using Core;
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
        public int ElapsedSeconds;
        public Vector2Int GridSize;
        public List<CellElementModel> CellModels;
    }

    public interface IGameplayHudView : IHudWithModel<GameplayHudModel>
    {
        event Action OnMenuButtonClicked;
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

        private GameplayHudModel _viewModel;

        protected override void Show()
        {
            _view.OnMenuButtonClicked += View_OnMenuButtonClicked;
            _view.OnCellClicked += View_OnCellClicked;
            _view.OnCellAsFlagClicked += View_OnCellAsFlagClicked;

            _service.OnStateChanged += Service_OnStateChanged;

            var gridSize = new Vector2Int(9, 9);

            _service.StartGame(
                new MinesweeperGameSettings()
                {
                    Width = gridSize.x,
                    Height = gridSize.y,
                    BombCount = 10,
                    Seed = 0,
                }
            );

            _viewModel = new GameplayHudModel
            {
                ElapsedSeconds = 0,
                GridSize = gridSize,
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
            _view.OnCellClicked -= View_OnCellClicked;
            _view.OnCellAsFlagClicked -= View_OnCellAsFlagClicked;

            _service.OnStateChanged -= Service_OnStateChanged;
        }

        private void View_OnMenuButtonClicked()
        {
            _gameStateManager.SwitchToState(new MainMenuState());
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
        }
    }
}
