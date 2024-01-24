using System;
using System.Collections.Generic;
using Game.Config;
using UnityEngine;
using Kukumberman.Minesweeper.Core;

namespace Kukumberman.Minesweeper.ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "",
        menuName = "SO/" + nameof(MinesweeperGameConfigScriptableObject)
    )]
    public sealed class MinesweeperGameConfigScriptableObject : ScriptableObject
    {
        public MinesweeperGameConfig Config;
    }

    [Serializable]
    public sealed class MinesweeperGameConfig : GameConfig
    {
        public List<MinesweeperStage> Stages;
    }

    [Serializable]
    public sealed class MinesweeperStage
    {
        public string Name;
        public MinesweeperGameSettings Settings;
    }
}
