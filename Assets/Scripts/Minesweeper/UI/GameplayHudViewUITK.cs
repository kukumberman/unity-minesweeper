using System;
using Game.UI.Hud;
using UnityEngine;
using UnityEngine.UIElements;
using Kukumberman.Minesweeper.Core;
using Kukumberman.Minesweeper.UI.Elements;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class GameplayHudViewUITK
        : BaseHudWithModelUITK<GameplayHudModel>,
            IGameplayHudView
    {
        public event Action OnMenuButtonClicked;
        public event Action OnRestartButtonClicked;
        public event Action OnHelpButtonClicked;
        public event Action<int> OnCellClicked;
        public event Action<int> OnCellAsFlagClicked;

        private Button _btnMenu;
        private Label _txtBombRemaining;
        private Button _btnRestart;
        private VisualElement _imgStateIcon;
        private Label _txtElapsedTime;
        private VisualElement _grid;
        private VisualElement _gridParent;
        private VisualElement _elementToReceiveDrag;
        private VisualElement _topBar;
        private VisualElement _imgBackground;
        private VisualElement _contentListInputPrompts;
        private VisualElement _btnHelp;
        private VisualElement _helpPanel;

        private VisualTreeAsset _uxmlCell;
        private VisualTreeAsset _uxmlInputPrompt;

        private const int kCellSize = 100;
        private const int kClickThresholdMs = 200;
        private const float kMinScale = 0.5f;
        private const float kMaxScale = 2f;
        private const int kHelpPanelLeftWhenVisible = 50;
        private const int kHelpPanelLeftWhenHidden = -400;

        private VisualElement _clickedCell;
        private bool _isDragging;
        private DateTime _clickedAtTime;
        private Vector3 _clickRelativeOffset;

        private bool _isPanelVisible;

        protected override void OnEnable()
        {
            // Dima:
            // no way to add reference in inspector like in MonoBehaviour implementation
            // and GameplayHudModel should not contain UXML data
            var staticData = GameObject.FindObjectOfType<MinesweeperStaticDataMono>();
            _uxmlCell = staticData.UxmlCell;
            _uxmlInputPrompt = staticData.UxmlInputPrompt;

            _btnMenu = this.Q<Button>("btn-menu");
            _btnRestart = this.Q<Button>("btn-restart");
            _imgStateIcon = _btnRestart[0];
            _txtBombRemaining = this.Q<Label>("label-bomb-remaining");
            _txtElapsedTime = this.Q<Label>("label-elapsed-time");
            _grid = this.Q<VisualElement>("grid");
            _gridParent = _grid.parent;
            _elementToReceiveDrag = _gridParent.parent;
            _topBar = _gridParent[0];
            _imgBackground = this.Q<VisualElement>("img-background");
            _contentListInputPrompts = this.Q<VisualElement>("list");
            _btnHelp = this.Q<VisualElement>("btn-help");
            _helpPanel = this.Q<VisualElement>("help-panel");

            Debug.Assert(_btnMenu != null);
            Debug.Assert(_btnRestart != null);
            Debug.Assert(_txtBombRemaining != null);
            Debug.Assert(_txtElapsedTime != null);
            Debug.Assert(_imgStateIcon != null);
            Debug.Assert(_grid != null);
            Debug.Assert(_imgBackground != null);
            Debug.Assert(_contentListInputPrompts != null);
            Debug.Assert(_btnHelp != null);
            Debug.Assert(_helpPanel != null);

            _btnMenu.clicked += BtnMenu_clicked;
            _btnRestart.clicked += BtnRestart_clicked;
            _btnHelp.RegisterCallback<ClickEvent>(BtnHelpClickEventHandler);
        }

        protected override void OnDisable()
        {
            _btnMenu.clicked -= BtnMenu_clicked;
            _btnRestart.clicked -= BtnRestart_clicked;

            _btnMenu = null;
            _btnRestart = null;
            _imgStateIcon = null;
            _txtBombRemaining = null;
            _txtElapsedTime = null;
            _grid = null;
            _gridParent = null;
            _elementToReceiveDrag = null;
            _topBar = null;
            _imgBackground = null;

            _contentListInputPrompts.Clear();
            _contentListInputPrompts = null;
            _btnHelp.UnregisterCallback<ClickEvent>(BtnHelpClickEventHandler);
            _btnHelp = null;
            _helpPanel = null;
        }

        protected override void OnApplyModel(GameplayHudModel model)
        {
            _imgBackground.style.backgroundImage = new StyleBackground(model.Background);

            CreateSimpleGrid();
            CreateInputPrompts();
        }

        protected override void OnModelChanged(GameplayHudModel model)
        {
            _txtElapsedTime.text = string.Format("{0}", model.ElapsedSeconds);
            _txtBombRemaining.text = string.Format("{0}", model.RemainingBombCount);
            _imgStateIcon.style.backgroundImage = new StyleBackground(model.StateIconSprite);

            var interactive = model.State == EMinesweeperState.Playing;

            _grid
                .Query<CellElement>()
                .ForEach(cell =>
                {
                    cell.SetInteractive(interactive);
                });
        }

        void IGameplayHudView.ToggleHelpPanel()
        {
            SetHelpPanelVisibleState(!_isPanelVisible);
        }

        private void BtnMenu_clicked()
        {
            OnMenuButtonClicked.SafeInvoke();
        }

        private void BtnRestart_clicked()
        {
            OnRestartButtonClicked.SafeInvoke();
        }

        private void BtnHelpClickEventHandler(ClickEvent evt)
        {
            OnHelpButtonClicked.SafeInvoke();
        }

        private void CellPointerDownEventHandler(PointerDownEvent evt)
        {
            _clickedCell = evt.currentTarget as VisualElement;
            _clickedAtTime = DateTime.Now;
        }

        private void CellPointerUpEventHandler(PointerUpEvent evt)
        {
            var element = evt.currentTarget as CellElement;

            if (element != _clickedCell)
            {
                return;
            }

            if (!element.ContainsPoint(evt.localPosition))
            {
                return;
            }

            var elapsed = DateTime.Now - _clickedAtTime;
            if (elapsed.TotalMilliseconds > kClickThresholdMs)
            {
                return;
            }

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

        private void GridPointerDownEventHandler(PointerDownEvent evt)
        {
            _isDragging = true;

            var elementPosition = _gridParent.worldTransform.GetPosition();
            elementPosition = UIElementsExtensions.GetTopLeft(_gridParent);
            _clickRelativeOffset = elementPosition - evt.position;
        }

        private void GridPointerUpEventHandler(PointerUpEvent evt)
        {
            _isDragging = false;
        }

        private void GridPointerMoveEventHandler(PointerMoveEvent evt)
        {
            if (_isDragging)
            {
                HandleDrag(evt.position);
            }
        }

        private void GridMouseWheelEventHandler(WheelEvent evt)
        {
            var scrollMultiplier = 5;
            var scrollValue = -1 * evt.delta.y * scrollMultiplier;

            var scale = _gridParent.resolvedStyle.scale.value;
            scale.x += scrollValue;
            scale.y += scrollValue;

            scale.x = Mathf.Clamp(scale.x, kMinScale, kMaxScale);
            scale.y = Mathf.Clamp(scale.y, kMinScale, kMaxScale);

            _gridParent.style.scale = new StyleScale(new Scale(scale));

            var position = UIElementsExtensions.GetTopLeft(_gridParent);
            position = ClampPosition(position);
            UIElementsExtensions.SetTopLeft(_gridParent, position);
        }

        private void CreateSimpleGrid()
        {
            var gridSize = Model.GridSize;

            _grid.style.width = new StyleLength(
                new Length(kCellSize * gridSize.x, LengthUnit.Pixel)
            );
            _grid.style.height = new StyleLength(
                new Length(kCellSize * gridSize.y, LengthUnit.Pixel)
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
                cell.SetSize(kCellSize);
                cell.Index = i;
                cell.RegisterCallback<PointerDownEvent>(CellPointerDownEventHandler);
                cell.RegisterCallback<PointerUpEvent>(CellPointerUpEventHandler);
                cell.Model = Model.CellModels[i];
                _grid.Add(cell);
            }

            _gridParent.style.top = new StyleLength(new Length(25, LengthUnit.Pixel));

            _elementToReceiveDrag.RegisterCallback<PointerDownEvent>(GridPointerDownEventHandler);
            _elementToReceiveDrag.RegisterCallback<PointerUpEvent>(GridPointerUpEventHandler);
            _elementToReceiveDrag.RegisterCallback<PointerMoveEvent>(GridPointerMoveEventHandler);
            _elementToReceiveDrag.RegisterCallback<WheelEvent>(GridMouseWheelEventHandler);
        }

        private void HandleDrag(Vector3 mousePosition)
        {
            var position = mousePosition + _clickRelativeOffset;
            position = ClampPosition(position);
            UIElementsExtensions.SetTopLeft(_gridParent, position);
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            var elementScale = _gridParent.resolvedStyle.scale.value;

            var elementSize = Vector2.zero;
            elementSize.x = _gridParent.resolvedStyle.width;
            elementSize.y = _gridParent.resolvedStyle.height;

            var elementHalfSize = elementSize * 0.5f;

            var cellPadding = new Vector2(kCellSize, kCellSize) * elementScale;

            var scaleOffset = Vector2.zero;
            scaleOffset.x = elementHalfSize.x * (1 - elementScale.x);
            scaleOffset.y = elementHalfSize.y * (1 - elementScale.y);

            var min = Vector2.zero;
            min.x = 0 - elementSize.x;
            min.y = 0 - elementSize.y;
            min += scaleOffset;
            min += cellPadding;

            var max = Vector2.zero;
            max.x = _elementToReceiveDrag.resolvedStyle.width;
            max.y = _elementToReceiveDrag.resolvedStyle.height;
            max -= scaleOffset;
            max -= cellPadding;

            max.y -= _topBar.resolvedStyle.height * elementScale.y;

            position.x = Mathf.Clamp(position.x, min.x, max.x);
            position.y = Mathf.Clamp(position.y, min.y, max.y);

            return position;
        }

        private void CreateInputPrompts()
        {
            _contentListInputPrompts.Clear();

            for (int i = 0; i < Model.InputPromptModels.Count; i++)
            {
                var element = _uxmlInputPrompt.Instantiate()[0] as InputPromptElement;

                foreach (var styleSheet in _uxmlInputPrompt.stylesheets)
                {
                    element.styleSheets.Add(styleSheet);
                }

                element.Setup();
                element.Model = Model.InputPromptModels[i];
                _contentListInputPrompts.Add(element);
            }
        }

        private void SetHelpPanelVisibleState(bool visible)
        {
            var value = visible ? kHelpPanelLeftWhenVisible : kHelpPanelLeftWhenHidden;
            _helpPanel.style.left = new StyleLength(new Length(value, LengthUnit.Pixel));

            _isPanelVisible = visible;
        }
    }
}
