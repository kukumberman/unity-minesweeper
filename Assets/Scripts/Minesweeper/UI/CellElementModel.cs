﻿using Core;
using UnityEngine;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class CellElementModel : Observable
    {
        public int BombNeighborCount;
        public Sprite SpriteBackground;
        public Sprite SpriteForeground;
    }
}