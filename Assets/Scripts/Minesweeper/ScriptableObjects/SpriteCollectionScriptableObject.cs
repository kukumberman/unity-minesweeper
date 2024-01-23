using UnityEngine;
using Kukumberman.Minesweeper.Enums;
using AYellowpaper.SerializedCollections;

namespace Kukumberman.Minesweeper.ScriptableObjects
{
    [CreateAssetMenu(fileName = "", menuName = "SO/" + nameof(SpriteCollectionScriptableObject))]
    public sealed class SpriteCollectionScriptableObject : ScriptableObject
    {
        [SerializeField]
        private SerializedDictionary<ESpriteType, Sprite> _sprites;

        public Sprite Get(ESpriteType type)
        {
            if (_sprites.TryGetValue(type, out var sprite))
            {
                return sprite;
            }

            return null;
        }
    }
}
