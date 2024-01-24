using UnityEngine;
using UnityEngine.UIElements;

namespace Kukumberman.Minesweeper.UI.Elements
{
    public sealed class InputPromptElement : VisualElementWithModel<InputPromptElementModel>
    {
        public new class UxmlFactory : UxmlFactory<InputPromptElement, UxmlTraits> { }

        private VisualElement _imgElement;
        private Label _txtLabel;

        protected override void OnModelChanged(InputPromptElementModel model)
        {
            _imgElement.style.backgroundImage = new StyleBackground(model.Sprite);
            _txtLabel.text = model.Text;
        }

        public void Setup()
        {
            _imgElement = this[0];
            _txtLabel = this[1] as Label;

            Debug.Assert(_imgElement != null);
            Debug.Assert(_txtLabel != null);
        }
    }
}
