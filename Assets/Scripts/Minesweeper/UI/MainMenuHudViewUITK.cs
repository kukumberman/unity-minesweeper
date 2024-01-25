using System;
using System.Collections.Generic;
using Game.UI.Hud;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class MainMenuHudViewUITK
        : BaseHudWithModelUITK<MainMenuHudModel>,
            IMainMenuHudView
    {
        private const int kTweenRotationMaxWobbleAngle = 10;
        private const int kTweenRotationLoopDurationMs = 5000;

        public event Action OnPlayButtonClicked;
        public event Action OnRandomizeSeedButtonClicked;
        public event Action<string> OnSeedInputFieldValueChanged;
        public event Action<int> OnSelectedStageIndexChanged;

        private Button _btnPlay;
        private TextField _inputFieldSeed;
        private Button _btnRandomizeSeed;
        private RadioButtonGroup _radioButtonGroup;
        private VisualElement _imgBackground;

        private List<ValueAnimation<float>> _tweens = new();

        protected override void OnEnable()
        {
            _btnPlay = this.Q<Button>("btn-play");
            _btnRandomizeSeed = this.Q<Button>("btn-randomize-seed");
            _inputFieldSeed = this.Q<TextField>("input-seed");
            _radioButtonGroup = this.Q<RadioButtonGroup>();
            _imgBackground = this.Q<VisualElement>("img-background");

            Debug.Assert(_btnPlay != null);
            Debug.Assert(_btnRandomizeSeed != null);
            Debug.Assert(_inputFieldSeed != null);
            Debug.Assert(_radioButtonGroup != null);
            Debug.Assert(_imgBackground != null);

            _btnPlay.clicked += BtnPlay_clicked;
            _btnRandomizeSeed.clicked += BtnRandomizeSeed_clicked;
            _inputFieldSeed.RegisterValueChangedCallback(InputFieldSeedValueChangedHandler);
            _radioButtonGroup.RegisterValueChangedCallback(
                RadioButtonGroupStageValueChangedHandler
            );

            var elementsToTween = this.Query<VisualElement>(null, "wobble-rotation").ToList();

            for (int i = 0; i < elementsToTween.Count; i++)
            {
                var tween = CreateRotationTween(elementsToTween[i]);
                tween.autoRecycle = false;
                tween.OnCompleted(() => tween.Start());
                _tweens.Add(tween);
            }
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
            _imgBackground = null;

            for (int i = 0; i < _tweens.Count; i++)
            {
                var tween = _tweens[i];
                tween.Stop();
                tween.Recycle();
            }

            _tweens.Clear();
        }

        protected override void OnApplyModel(MainMenuHudModel model)
        {
            _imgBackground.style.backgroundImage = new StyleBackground(model.Background);
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

        private ValueAnimation<float> CreateRotationTween(VisualElement element)
        {
            return element.experimental.animation
                .Start(0f, 1f, kTweenRotationLoopDurationMs, RotationTweenOnValueChanged)
                .Ease(Easing.Linear);
        }

        private void RotationTweenOnValueChanged(VisualElement ve, float value01)
        {
            var sinInput = 2 * Mathf.PI * value01;
            var sinOutput = Mathf.Sin(sinInput);
            var angle = sinOutput * kTweenRotationMaxWobbleAngle;
            ve.style.rotate = new StyleRotate(new Rotate(new Angle(angle, AngleUnit.Degree)));
        }
    }
}
