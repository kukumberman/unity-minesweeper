using System;
using System.Collections.Generic;
using Core;
using Game.Core.UI;
using Game.States;
using Game.UI.Hud;
using Injection;
using UnityEngine;
using Kukumberman.Minesweeper.States;
using Kukumberman.Minesweeper.ScriptableObjects;
using Kukumberman.Minesweeper.Managers;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class MainMenuHudModel : Observable
    {
        public sealed class Localization
        {
            public string LabelPlay;
            public string LabelStage;
            public string LabelSeed;
            public string LabelRandomizeSeed;
            public string LabelLanguage;
        }

        public string SeedAsText;
        public int SelectedStageIndex;
        public string[] StageNames;
        public Texture2D Background;

        public LanguageCollectionModel LanguageCollectionModel;

        public Localization I18N;
    }

    public interface IMainMenuHudView : IHudWithModel<MainMenuHudModel>
    {
        event Action OnPlayButtonClicked;
        event Action OnRandomizeSeedButtonClicked;
        event Action<string> OnSeedInputFieldValueChanged;
        event Action<int> OnSelectedStageIndexChanged;
        event Action<string> OnLanguageItemButtonClicked;
    }

    public sealed class MainMenuHudMediator : Mediator<IMainMenuHudView>
    {
        [Inject]
        private GameStateManager _gameStateManager;

        [Inject]
        private MinesweeperGameConfigScriptableObject _gameConfig;

        [Inject]
        private SpriteStorage _spriteStorage;

        [Inject]
        private LocalizationManager _localizationManager;

        [Inject]
        private MinesweeperGameModel _gameModel;

        [Inject]
        private BlurEffectBehaviour _blurEffect;

        private MainMenuHudModel _viewModel;

        protected override void Show()
        {
            _view.OnPlayButtonClicked += View_OnPlayButtonClicked;
            _view.OnRandomizeSeedButtonClicked += View_OnRandomizeSeedButtonClicked;
            _view.OnSeedInputFieldValueChanged += View_OnSeedInputFieldValueChanged;
            _view.OnSelectedStageIndexChanged += View_OnSelectedStageIndexChanged;
            _view.OnLanguageItemButtonClicked += View_OnLanguageItemButtonClicked;

            _localizationManager.OnLanguageChanged += LocalizationManager_OnLanguageChanged;

            _viewModel = new MainMenuHudModel()
            {
                SeedAsText = 0.ToString(),
                SelectedStageIndex = 0,
                StageNames = new string[_gameConfig.Config.Stages.Count],
                Background = _blurEffect.Result,
                LanguageCollectionModel = new LanguageCollectionModel()
                {
                    ItemModels = new List<LanguageCollectionItemModel>(),
                },
                I18N = new MainMenuHudModel.Localization(),
            };

            _viewModel.SeedAsText = _gameModel.SeedAsText;
            _viewModel.SelectedStageIndex = Mathf.Clamp(
                _gameModel.SelectedStageIndex,
                0,
                _gameConfig.Config.Stages.Count
            );

            PopulateStageNames();

            PopulateLanguageCollectionModel();

            PopulateLocalizedText();

            _view.Model = _viewModel;
        }

        protected override void Hide()
        {
            _view.OnPlayButtonClicked -= View_OnPlayButtonClicked;
            _view.OnRandomizeSeedButtonClicked -= View_OnRandomizeSeedButtonClicked;
            _view.OnSeedInputFieldValueChanged -= View_OnSeedInputFieldValueChanged;
            _view.OnSelectedStageIndexChanged -= View_OnSelectedStageIndexChanged;
            _view.OnLanguageItemButtonClicked -= View_OnLanguageItemButtonClicked;

            _localizationManager.OnLanguageChanged -= LocalizationManager_OnLanguageChanged;
        }

        private void PopulateStageNames()
        {
            for (int i = 0; i < _viewModel.StageNames.Length; i++)
            {
                _viewModel.StageNames[i] = StageToDisplayString(_gameConfig.Config.Stages[i]);
            }
        }

        private void PopulateLanguageCollectionModel()
        {
            var languages = _localizationManager.SupportedLanguages;

            for (int i = 0; i < languages.Count; i++)
            {
                var itemModel = new LanguageCollectionItemModel()
                {
                    Language = languages[i],
                    IsSelected = languages[i] == _localizationManager.CurrentLanguage,
                    FlagSprite = _spriteStorage.Get(string.Format("flag_{0}", languages[i])),
                };
                _viewModel.LanguageCollectionModel.ItemModels.Add(itemModel);
            }
        }

        private void PopulateLocalizedText()
        {
            _viewModel.SetChanged();
            _viewModel.I18N.LabelPlay = _localizationManager.GetValue("button_play");
            _viewModel.I18N.LabelStage = _localizationManager.GetValue("label_stage");
            _viewModel.I18N.LabelSeed = _localizationManager.GetValue("label_seed");
            _viewModel.I18N.LabelRandomizeSeed = _localizationManager.GetValue("button_randomize");
            _viewModel.I18N.LabelLanguage = _localizationManager.GetValue("label_language");

            PopulateStageNames();

            _viewModel.SetChanged();
        }

        private void View_OnPlayButtonClicked()
        {
            var stage = _gameConfig.Config.Stages[_viewModel.SelectedStageIndex];
            var seed = _viewModel.SeedAsText.GetHashCode();

            _gameStateManager.SwitchToState(new GameplayState(stage.Settings, seed));
        }

        private void View_OnRandomizeSeedButtonClicked()
        {
            _viewModel.SeedAsText = GenerateRandomSeed();
            _viewModel.SetChanged();

            _gameModel.SeedAsText = _viewModel.SeedAsText;
            _gameModel.Save();
        }

        private void View_OnSeedInputFieldValueChanged(string newValue)
        {
            _viewModel.SeedAsText = newValue;
            _viewModel.SetChanged();

            _gameModel.SeedAsText = _viewModel.SeedAsText;
            _gameModel.Save();
        }

        private void View_OnSelectedStageIndexChanged(int index)
        {
            _viewModel.SelectedStageIndex = index;
            _viewModel.SetChanged();

            _gameModel.SelectedStageIndex = _viewModel.SelectedStageIndex;
            _gameModel.Save();
        }

        private void View_OnLanguageItemButtonClicked(string language)
        {
            if (!_localizationManager.TrySetLanguage(language))
            {
                return;
            }

            for (int i = 0; i < _viewModel.LanguageCollectionModel.ItemModels.Count; i++)
            {
                var itemModel = _viewModel.LanguageCollectionModel.ItemModels[i];
                itemModel.IsSelected = itemModel.Language == _localizationManager.CurrentLanguage;
                itemModel.SetChanged();
            }
        }

        private void LocalizationManager_OnLanguageChanged()
        {
            PopulateLocalizedText();
        }

        private static string GenerateRandomSeed()
        {
            var ms = DateTime.Now.Millisecond;
            var text = ms.ToString();

            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(text);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToBase64String(hashBytes).Substring(0, 8);
            }
        }

        private string StageToDisplayString(MinesweeperStage stage)
        {
            var key = string.Format("stage_{0}", stage.Name.ToLower());
            var name = _localizationManager.GetValue(key);
            return string.Format(
                "{0} {1}x{2} ({3})",
                name,
                stage.Settings.Width,
                stage.Settings.Height,
                stage.Settings.BombCount
            );
        }
    }
}
