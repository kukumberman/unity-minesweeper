using UnityEngine;
using UnityEngine.UIElements;

namespace Kukumberman.Minesweeper.UI.Elements
{
    public sealed class CellElement : VisualElementWithModel<CellElementModel>
    {
        public new class UxmlFactory : UxmlFactory<CellElement, UxmlTraits> { }

        private VisualElement _imgBackground;
        private VisualElement _imgForeground;
        private Label _labelNeighborCount;

        public int Index { get; set; }

        protected override void OnModelChanged(CellElementModel model)
        {
            SetNeighborCount(model.BombNeighborCount);
            SetBackgroundSprite(model.SpriteBackground);
            SetForegroundSprite(model.SpriteForeground);

            this.EnableInClassList("cell-hidden", !model.IsRevealed);
        }

        public void Setup()
        {
            _imgBackground = this;
            _imgForeground = this.Q<VisualElement>("img-foreground");
            _labelNeighborCount = this.Q<Label>();
        }

        public void SetSize(int pixelSize)
        {
            style.width = new StyleLength(new Length(pixelSize, LengthUnit.Pixel));
            style.height = style.width;

            // Dima: 69 pixels per each 100 pixels in size
            var fontSizePx = 69 * pixelSize / 100f;
            _labelNeighborCount.style.fontSize = new StyleLength(
                new Length(fontSizePx, LengthUnit.Pixel)
            );
        }

        public void SetNeighborCount(int count)
        {
            if (count == 0)
            {
                _labelNeighborCount.text = string.Empty;
            }
            else
            {
                _labelNeighborCount.text = string.Format("{0}", count);
            }

            for (int i = 1; i <= 8; i++)
            {
                var className = string.Format("neighbor-{0}", i);
                var active = i == count;
                _labelNeighborCount.EnableInClassList(className, active);
            }
        }

        public void SetBackgroundSprite(Sprite sprite)
        {
            _imgBackground.style.backgroundImage = new StyleBackground(sprite);
        }

        public void SetBackgroundTintColor(Color color)
        {
            _imgBackground.style.unityBackgroundImageTintColor = new StyleColor(color);
        }

        public void SetForegroundSprite(Sprite sprite)
        {
            _imgForeground.style.display = sprite != null ? DisplayStyle.Flex : DisplayStyle.None;
            _imgForeground.style.backgroundImage = new StyleBackground(sprite);
        }
    }
}
