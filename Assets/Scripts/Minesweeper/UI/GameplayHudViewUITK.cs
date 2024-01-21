using System;
using Game.UI.Hud;
using UnityEngine;
using UnityEngine.UIElements;
using Kukumberman.Minesweeper.UI.Elements;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class GameplayHudViewUITK
        : BaseHudWithModelUITK<GameplayHudModel>,
            IGameplayHudView
    {
        public event Action OnMenuButtonClicked;
        public event Action<int> OnCellClicked;
        public event Action<int> OnCellAsFlagClicked;

        private Button _btnMenu;
        private Label _txtElapsedTime;
        private VisualElement _grid;

        private VisualTreeAsset _uxmlCell;

        protected override void OnEnable()
        {
            // Dima:
            // no way to add reference in inspector like in MonoBehaviour implementation
            // and GameplayHudModel should not contain UXML data
            var staticData = GameObject.FindObjectOfType<MinesweeperStaticDataMono>();
            _uxmlCell = staticData.UxmlCell;

            _btnMenu = this.Q<Button>("btn-menu");
            _txtElapsedTime = this.Q<Label>("label-elapsed-time");
            _grid = this.Q<VisualElement>("grid");

            Debug.Assert(_btnMenu != null);
            Debug.Assert(_txtElapsedTime != null);
            Debug.Assert(_grid != null);

            _btnMenu.clicked += BtnMenu_clicked;
        }

        protected override void OnDisable()
        {
            _btnMenu.clicked -= BtnMenu_clicked;

            _btnMenu = null;
            _txtElapsedTime = null;
            _grid = null;
        }

        protected override void OnApplyModel(GameplayHudModel model)
        {
            CreateSimpleGrid();
        }

        protected override void OnModelChanged(GameplayHudModel model)
        {
            _txtElapsedTime.text = string.Format("{0}", model.ElapsedSeconds);
        }

        private void BtnMenu_clicked()
        {
            OnMenuButtonClicked.SafeInvoke();
        }

        private void CellPointerUpEventHandler(PointerUpEvent evt)
        {
            var element = evt.currentTarget as CellElement;
            var idx = element.Index;

            if (evt.button == 0)
            {
                OnCellClicked.SafeInvoke(idx);
            }
            else if (evt.button == 1)
            {
                OnCellAsFlagClicked.SafeInvoke(idx);
            }
        }

        private void CreateSimpleGrid()
        {
            var cellSize = 100;
            var gridSize = Model.GridSize;

            _grid.style.width = new StyleLength(
                new Length(cellSize * gridSize.x, LengthUnit.Pixel)
            );
            _grid.style.height = new StyleLength(
                new Length(cellSize * gridSize.y, LengthUnit.Pixel)
            );

            _grid.Clear();

            for (int i = 0, length = Model.CellModels.Count; i < length; i++)
            {
                var cell = _uxmlCell.Instantiate()[0] as CellElement;

                foreach (var styleSheet in _uxmlCell.stylesheets)
                {
                    cell.styleSheets.Add(styleSheet);
                }

                cell.Setup();
                cell.SetSize(cellSize);
                cell.Index = i;
                cell.RegisterCallback<PointerUpEvent>(CellPointerUpEventHandler);
                cell.Model = Model.CellModels[i];
                _grid.Add(cell);
            }
        }
    }
}
