using System;
using System.Linq;
using Core;
using Game.Core.UI;
using Game.States;
using Game.UI.Hud;
using Injection;
using UnityEngine;
using Kukumberman.Minesweeper.States;
using Kukumberman.Minesweeper.ScriptableObjects;

namespace Kukumberman.Minesweeper.UI
{
    public sealed class MainMenuHudModel : Observable
    {
        public string SeedAsText;
        public int SelectedStageIndex;
        public string[] StageNames;
        public Texture2D Background;
    }

    public interface IMainMenuHudView : IHudWithModel<MainMenuHudModel>
    {
        event Action OnPlayButtonClicked;
        event Action OnRandomizeSeedButtonClicked;
        event Action<string> OnSeedInputFieldValueChanged;
        event Action<int> OnSelectedStageIndexChanged;
    }

    public sealed class MainMenuHudMediator : Mediator<IMainMenuHudView>
    {
        [Inject]
        private GameStateManager _gameStateManager;

        [Inject]
        private MinesweeperGameConfigScriptableObject _gameConfig;

        [Inject]
        private BlurEffectBehaviour _blurEffect;

        private MainMenuHudModel _viewModel;

        protected override void Show()
        {
            _view.OnPlayButtonClicked += View_OnPlayButtonClicked;
            _view.OnRandomizeSeedButtonClicked += View_OnRandomizeSeedButtonClicked;
            _view.OnSeedInputFieldValueChanged += View_OnSeedInputFieldValueChanged;
            _view.OnSelectedStageIndexChanged += View_OnSelectedStageIndexChanged;

            _viewModel = new MainMenuHudModel()
            {
                SeedAsText = 0.ToString(),
                SelectedStageIndex = 0,
                StageNames = _gameConfig.Config.Stages.Select(StageToDisplayString).ToArray(),
                Background = _blurEffect.Result,
            };

            _view.Model = _viewModel;
        }

        protected override void Hide()
        {
            _view.OnPlayButtonClicked -= View_OnPlayButtonClicked;
            _view.OnRandomizeSeedButtonClicked -= View_OnRandomizeSeedButtonClicked;
            _view.OnSeedInputFieldValueChanged -= View_OnSeedInputFieldValueChanged;
            _view.OnSelectedStageIndexChanged -= View_OnSelectedStageIndexChanged;
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
        }

        private void View_OnSeedInputFieldValueChanged(string newValue)
        {
            _viewModel.SeedAsText = newValue;
            _viewModel.SetChanged();
        }

        private void View_OnSelectedStageIndexChanged(int index)
        {
            _viewModel.SelectedStageIndex = index;
            _viewModel.SetChanged();
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

        private static string StageToDisplayString(MinesweeperStage stage)
        {
            return string.Format(
                "{0} {1}x{2} ({3})",
                stage.Name,
                stage.Settings.Width,
                stage.Settings.Height,
                stage.Settings.BombCount
            );
        }
    }
}
