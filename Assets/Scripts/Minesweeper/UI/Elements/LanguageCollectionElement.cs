using System;
using System.Collections.Generic;
using Core;
using Injection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class LanguageCollectionModel : Observable
    {
        public List<LanguageCollectionItemModel> ItemModels;
    }

    public sealed class LanguageCollectionItemModel : Observable
    {
        public bool IsSelected;
        public string Language;
        public Sprite FlagSprite;
    }
}

namespace Kukumberman.Minesweeper.UI.Elements
{
    public sealed class LanguageCollectionElement : VisualElementWithModel<LanguageCollectionModel>
    {
        public new class UxmlFactory : UxmlFactory<LanguageCollectionElement, UxmlTraits> { }

        public event Action<string> OnItemClicked;

        private VisualElement _contentParent;

        private VisualTreeAsset _uxmlItem;

        private List<LanguageCollectionItemElement> _items = new();

        public void OnEnable()
        {
            var staticData = Context.Current.Get<MinesweeperStaticDataMono>();
            _uxmlItem = staticData.UxmlLanguageCollectionItemElement;

            _contentParent = this.Q<VisualElement>("content");
        }

        public void OnDisable()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].OnDisable();
            }

            _items.Clear();
        }

        protected override void OnApplyModel(LanguageCollectionModel model)
        {
            if (model != null)
            {
                Create();
            }
        }

        protected override void OnModelChanged(LanguageCollectionModel model) { }

        public void SetSelectionColor(Color color)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Model.IsSelected)
                {
                    _items[i].SetSelectionColor(color);
                }
            }
        }

        private void Create()
        {
            _contentParent.Clear();

            for (int i = 0; i < Model.ItemModels.Count; i++)
            {
                var item = _uxmlItem.Instantiate()[0] as LanguageCollectionItemElement;

                foreach (var styleSheet in _uxmlItem.stylesheets)
                {
                    item.styleSheets.Add(styleSheet);
                }

                item.OnEnable();
                item.Model = Model.ItemModels[i];
                item.RegisterCallback<ClickEvent>(LanguageItemElementClickHandler);
                _contentParent.Add(item);
                _items.Add(item);
            }
        }

        private void LanguageItemElementClickHandler(ClickEvent evt)
        {
            var element = evt.currentTarget as LanguageCollectionItemElement;
            var language = element.Model.Language;
            OnItemClicked.SafeInvoke(language);
        }
    }
}
