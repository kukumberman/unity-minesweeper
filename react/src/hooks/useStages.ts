import useAppContext from "./useAppContext";
import useLocalization from "./useLocalization";

export default function useStages() {
  const { getLocalizedValue } = useLocalization();

  const { stageIndex, setStageIndex } = useAppContext();

  function formatStageName(stage: MinesweeperStage) {
    const settings = stage.Settings;
    const key = `stage_${stage.Name.toLowerCase()}`;
    const name = getLocalizedValue(key);
    return `${name} ${settings.Width}x${settings.Height} (${settings.BombCount})`;
  }

  const cfg = Globals.GameConfig.Config;

  return {
    stageIndex,
    setStageIndex,
    currentStage: cfg.Stages[stageIndex] as MinesweeperStage,
    stageTitles: cfg.Stages.map(formatStageName) as string[],
  };
}
