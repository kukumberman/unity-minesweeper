using Game.Core.UI;
using Game.Domain;
using Game.Managers;
using Game.States;
using Injection;
using UnityEngine;
using Kukumberman.Minesweeper.Enums;
using Kukumberman.Minesweeper.UI;
using Kukumberman.Minesweeper.ScriptableObjects;
using Kukumberman.Minesweeper.Managers;

namespace Kukumberman.Minesweeper.States
{
    public sealed class GameInitializeState : GameState
    {
        [Inject]
        private MinesweeperStartBehaviour _game;

        [Inject]
        private Context _context;

        [Inject]
        private GameStateManager _gameStateManager;

        [Inject]
        private MinesweeperGameConfigScriptableObject _gameConfig;

        [Inject]
        private MinesweeperStaticDataMono _staticData;

        public override void Initialize()
        {
            var gameModel = GameModel.Load<MinesweeperGameModel>(_gameConfig.Config);
            _context.Install(gameModel);

            _context.Install(new LocalizationManager());

            var hudManagerService = new HudManagerService()
            {
                UGUI = _context.GetByName("HudManager_UGUI") as HudManager,
                UITK = _context.GetByName("HudManager_UITK") as HudManager,
                Active = null,
            };

            hudManagerService.Active = hudManagerService.UITK;

            _context.Install(hudManagerService);
            _context.Install(hudManagerService.UITK);

            var hudManager = hudManagerService.UITK;

            hudManager.Orientation = Game.Enums.HudOrientation.Default;
            if (hudManager.HudFactory is HudFactoryUITK hudFactory)
            {
                hudFactory.Bind(typeof(IMainMenuHudView), () => new MainMenuHudViewUITK());
                hudFactory.Bind(typeof(IGameplayHudView), () => new GameplayHudViewUITK());
            }

            _context.ApplyInstall();

            _context.Get<SpriteStorage>().Initialize();
            _context
                .Get<LocalizationManager>()
                .CreateFromJson(_staticData.LocalizationJsonTextAsset.text);

            var nextState = GetNextState();
            _gameStateManager.SwitchToState(nextState);
        }

        public override void Dispose()
        {
            //
        }

        private GameState GetNextState()
        {
            if (!_game.OverrideInitialState)
            {
                return new MainMenuState();
            }

            switch (_game.InitialState)
            {
                case StateType.MainMenu:
                    return new MainMenuState();
                case StateType.Gameplay:
                    var seed = 0.ToString().GetHashCode();
                    return new GameplayState(_gameConfig.Config.Stages[0].Settings, seed);
                default:
                    Debug.LogWarning(
                        $"Initial switch for state [{_game.InitialState}] is not implemented"
                    );
                    return new MainMenuState();
            }
        }
    }
}
