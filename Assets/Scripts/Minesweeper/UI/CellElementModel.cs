using Core;
using UnityEngine;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class CellElementModel : Observable
    {
        public bool IsRevealed;
        public int BombNeighborCount;
        public Sprite SpriteBackground;
        public Sprite SpriteForeground;
    }
}
