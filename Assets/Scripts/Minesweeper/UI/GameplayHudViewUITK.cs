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

        private Button _btnMenu;
        private Label _txtElapsedTime;
        private VisualElement _grid;

        protected override void OnEnable()
        {
            _btnMenu = this.Q<Button>("btn-menu");
            _txtElapsedTime = this.Q<Label>("label-elapsed-time");
            _grid = this.Q<VisualElement>("grid");

            Debug.Assert(_btnMenu != null);
            Debug.Assert(_txtElapsedTime != null);
            Debug.Assert(_grid != null);

            _btnMenu.clicked += BtnMenu_clicked;

            CreateSimpleGrid();
        }

        protected override void OnDisable()
        {
            _btnMenu.clicked -= BtnMenu_clicked;

            _btnMenu = null;
            _txtElapsedTime = null;
            _grid = null;
        }

        protected override void OnModelChanged(GameplayHudModel model)
        {
            _txtElapsedTime.text = string.Format("{0}", model.ElapsedSeconds);
        }

        private void BtnMenu_clicked()
        {
            OnMenuButtonClicked.SafeInvoke();
        }

        private void CellClickHandler(ClickEvent evt)
        {
            var element = evt.currentTarget as CellElement;

            OnCellClicked.SafeInvoke(element.Index);
        }

        private void CreateSimpleGrid()
        {
            var cellSize = 100;
            var gridSize = new Vector2Int(5, 6);

            _grid.style.width = new StyleLength(
                new Length(cellSize * gridSize.x, LengthUnit.Pixel)
            );
            _grid.style.height = new StyleLength(
                new Length(cellSize * gridSize.y, LengthUnit.Pixel)
            );

            _grid.Clear();

            var staticData = GameObject.FindObjectOfType<MinesweeperStaticDataMono>();

            for (int i = 0, length = gridSize.x * gridSize.y; i < length; i++)
            {
                var cell = staticData.UxmlCell.Instantiate()[0] as CellElement;

                foreach (var styleSheet in staticData.UxmlCell.stylesheets)
                {
                    cell.styleSheets.Add(styleSheet);
                }

                var neighborCount = i % 9;

                cell.Setup();
                cell.SetSize(cellSize);
                cell.SetNeighborCount(neighborCount);
                cell.SetForegroundSprite(null);
                cell.SetBackgroundSprite(staticData.SpriteCellUnlocked);
                cell.Index = i;
                cell.RegisterCallback<ClickEvent>(CellClickHandler);
                _grid.Add(cell);
            }
        }
    }
}
