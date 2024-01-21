using System;
using Game.UI.Hud;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class MainMenuHudViewUITK
        : BaseHudWithModelUITK<MainMenuHudModel>,
            IMainMenuHudView
    {
        public event Action OnPlayButtonClicked;

        private Button _btnPlay;

        protected override void OnEnable()
        {
            _btnPlay = this.Q<Button>("btn-play");

            Debug.Assert(_btnPlay != null);

            _btnPlay.clicked += BtnPlay_clicked;
        }

        protected override void OnDisable()
        {
            _btnPlay.clicked -= BtnPlay_clicked;

            _btnPlay = null;
        }

        protected override void OnModelChanged(MainMenuHudModel model)
        {
            //
        }

        private void BtnPlay_clicked()
        {
            OnPlayButtonClicked.SafeInvoke();
        }
    }
}
