using System;
using System.Collections.Generic;
using Game.UI.Hud;
using Injection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using Kukumberman.Minesweeper.UI.Elements;
using Kukumberman.Extra;

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
        public event Action<string> OnLanguageItemButtonClicked;

        private Button _btnPlay;
        private TextField _inputFieldSeed;
        private Button _btnRandomizeSeed;
        private RadioButtonGroup _radioButtonGroup;
        private Label _labelStage;
        private Label _labelSeed;
        private Label _labelLanguage;
        private VisualElement _imgBackground;
        private LanguageCollectionElement _languageCollection;

        private List<ValueAnimation<float>> _tweens = new();

        private GradientColorBehaviour _gradientColor;

        protected override void OnEnable()
        {
            _btnPlay = this.Q<Button>("btn-play");
            _btnRandomizeSeed = this.Q<Button>("btn-randomize-seed");
            _inputFieldSeed = this.Q<TextField>("input-seed");
            _radioButtonGroup = this.Q<RadioButtonGroup>();
            _labelStage = this.Q<Label>("label-stage");
            _labelSeed = this.Q<Label>("label-seed");
            _labelLanguage = this.Q<Label>("label-language");
            _imgBackground = this.Q<VisualElement>("img-background");
            _languageCollection = this.Q<LanguageCollectionElement>();

            Debug.Assert(_btnPlay != null);
            Debug.Assert(_btnRandomizeSeed != null);
            Debug.Assert(_inputFieldSeed != null);
            Debug.Assert(_radioButtonGroup != null);
            Debug.Assert(_labelStage != null);
            Debug.Assert(_labelSeed != null);
            Debug.Assert(_labelLanguage != null);
            Debug.Assert(_imgBackground != null);
            Debug.Assert(_languageCollection != null);

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

            _languageCollection.OnEnable();
            _languageCollection.OnItemClicked += LanguageCollection_OnItemClicked;

            _gradientColor = Context.Current.Get<GradientColorBehaviour>();
            _gradientColor.OnColorChanged.AddListener(OnGradientColorChanged);
        }

        protected override void OnDisable()
        {
            _btnPlay.clicked -= BtnPlay_clicked;
            _btnRandomizeSeed.clicked -= BtnRandomizeSeed_clicked;
            _inputFieldSeed.UnregisterValueChangedCallback(InputFieldSeedValueChangedHandler);
            _radioButtonGroup.UnregisterValueChangedCallback(
                RadioButtonGroupStageValueChangedHandler
            );

            _languageCollection.OnDisable();
            _languageCollection.OnItemClicked -= LanguageCollection_OnItemClicked;
            _languageCollection = null;

            _btnPlay = null;
            _btnRandomizeSeed = null;
            _inputFieldSeed = null;
            _radioButtonGroup = null;
            _labelStage = null;
            _labelSeed = null;
            _labelLanguage = null;
            _imgBackground = null;

            for (int i = 0; i < _tweens.Count; i++)
            {
                var tween = _tweens[i];
                tween.Stop();
                tween.Recycle();
            }

            _tweens.Clear();

            _gradientColor.OnColorChanged.RemoveListener(OnGradientColorChanged);
            _gradientColor = null;
        }

        protected override void OnApplyModel(MainMenuHudModel model)
        {
            _imgBackground.style.backgroundImage = new StyleBackground(model.Background);
            _radioButtonGroup.choices = model.StageNames;
            _languageCollection.Model = model.LanguageCollectionModel;
        }

        protected override void OnModelChanged(MainMenuHudModel model)
        {
            _radioButtonGroup.choices = model.StageNames;

            _inputFieldSeed.SetValueWithoutNotify(model.SeedAsText);
            _radioButtonGroup.SetValueWithoutNotify(model.SelectedStageIndex);

            UpdateLocalizedText();
        }

        private void UpdateLocalizedText()
        {
            _btnPlay.text = Model.I18N.LabelPlay;
            _labelStage.text = Model.I18N.LabelStage;
            _labelSeed.text = Model.I18N.LabelSeed;
            _labelLanguage.text = Model.I18N.LabelLanguage;
            _btnRandomizeSeed.text = Model.I18N.LabelRandomizeSeed;
        }

        private void OnGradientColorChanged(Color color)
        {
            _languageCollection.SetSelectionColor(color);
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

        private void LanguageCollection_OnItemClicked(string language)
        {
            OnLanguageItemButtonClicked.SafeInvoke(language);
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
