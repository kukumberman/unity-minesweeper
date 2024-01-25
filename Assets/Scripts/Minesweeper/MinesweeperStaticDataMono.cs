using UnityEngine;
using UnityEngine.UIElements;

namespace Kukumberman.Minesweeper
{
    public sealed class MinesweeperStaticDataMono : MonoBehaviour
    {
        [Header("UXML")]
        public VisualTreeAsset UxmlCell;
        public VisualTreeAsset UxmlInputPrompt;
        public VisualTreeAsset UxmlLanguageCollectionItemElement;

        [Space]
        public TextAsset LocalizationJsonTextAsset;
    }
}
