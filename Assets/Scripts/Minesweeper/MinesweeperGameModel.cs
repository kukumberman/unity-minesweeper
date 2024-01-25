using Game.Config;
using Game.Domain;

namespace Kukumberman.Minesweeper
{
    public sealed class MinesweeperGameModel : GameModel
    {
        public int SelectedStageIndex;
        public string SeedAsText;
        public string PreferredLanguage;

        protected override void PopulateDefaultModel(GameConfig config)
        {
            SelectedStageIndex = 0;
            SeedAsText = "random";
        }
    }
}
