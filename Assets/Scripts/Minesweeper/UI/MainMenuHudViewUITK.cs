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
        public event Action OnRandomizeSeedButtonClicked;
        public event Action<string> OnSeedInputFieldValueChanged;
        public event Action<int> OnSelectedStageIndexChanged;

        private Button _btnPlay;
        private TextField _inputFieldSeed;
        private Button _btnRandomizeSeed;
        private RadioButtonGroup _radioButtonGroup;

        protected override void OnEnable()
        {
            _btnPlay = this.Q<Button>("btn-play");
            _btnRandomizeSeed = this.Q<Button>("btn-randomize-seed");
            _inputFieldSeed = this.Q<TextField>("input-seed");
            _radioButtonGroup = this.Q<RadioButtonGroup>();

            Debug.Assert(_btnPlay != null);
            Debug.Assert(_btnRandomizeSeed != null);
            Debug.Assert(_inputFieldSeed != null);
            Debug.Assert(_radioButtonGroup != null);

            _btnPlay.clicked += BtnPlay_clicked;
            _btnRandomizeSeed.clicked += BtnRandomizeSeed_clicked;
            _inputFieldSeed.RegisterValueChangedCallback(InputFieldSeedValueChangedHandler);
            _radioButtonGroup.RegisterValueChangedCallback(
                RadioButtonGroupStageValueChangedHandler
            );
        }

        protected override void OnDisable()
        {
            _btnPlay.clicked -= BtnPlay_clicked;
            _btnRandomizeSeed.clicked -= BtnRandomizeSeed_clicked;
            _inputFieldSeed.UnregisterValueChangedCallback(InputFieldSeedValueChangedHandler);
            _radioButtonGroup.UnregisterValueChangedCallback(
                RadioButtonGroupStageValueChangedHandler
            );

            _btnPlay = null;
            _btnRandomizeSeed = null;
            _inputFieldSeed = null;
            _radioButtonGroup = null;
        }

        protected override void OnApplyModel(MainMenuHudModel model)
        {
            _radioButtonGroup.choices = model.StageNames;
        }

        protected override void OnModelChanged(MainMenuHudModel model)
        {
            _inputFieldSeed.SetValueWithoutNotify(model.SeedAsText);
            _radioButtonGroup.SetValueWithoutNotify(model.SelectedStageIndex);
        }

        private void BtnPlay_clicked()
        {
            OnPlayButtonClicked.SafeInvoke();
        }

        private void BtnRandomizeSeed_clicked()
        {
            OnRandomizeSeedButtonClicked.SafeInvoke();
        }

        private void InputFieldSeedValueChangedHandler(ChangeEvent<string> evt)
        {
            OnSeedInputFieldValueChanged.SafeInvoke(evt.newValue);
        }

        private void RadioButtonGroupStageValueChangedHandler(ChangeEvent<int> evt)
        {
            OnSelectedStageIndexChanged.SafeInvoke(evt.newValue);
        }
    }
}
