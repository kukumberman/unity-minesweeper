using UnityEngine;
using UnityEngine.UIElements;

namespace Kukumberman.Minesweeper.UI.Elements
{
    public sealed class LanguageCollectionItemElement
        : VisualElementWithModel<LanguageCollectionItemModel>
    {
        public new class UxmlFactory : UxmlFactory<LanguageCollectionItemElement, UxmlTraits> { }

        private VisualElement _imgSelection;
        private VisualElement _imgFlag;

        public void OnEnable()
        {
            _imgSelection = this.Q<VisualElement>("img-selection");
            _imgFlag = this.Q<VisualElement>("img-flag");

            Debug.Assert(_imgSelection != null);
            Debug.Assert(_imgFlag != null);
        }

        public void OnDisable()
        {
            _imgSelection = null;
            _imgFlag = null;
        }

        protected override void OnModelChanged(LanguageCollectionItemModel model)
        {
            _imgSelection.style.visibility = model.IsSelected
                ? Visibility.Visible
                : Visibility.Hidden;
            _imgFlag.style.backgroundImage = new StyleBackground(model.FlagSprite);
        }

        public void SetSelectionColor(Color color)
        {
            _imgSelection.style.unityBackgroundImageTintColor = new StyleColor(color);
        }
    }
}
