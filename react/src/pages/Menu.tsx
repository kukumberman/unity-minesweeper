import { Style } from "@reactunity/renderer";
import { useLocalization, useSeed, useStages } from "src/hooks";
import {
  Logo,
  Memes,
  PlayButton,
  StageSection,
  SeedSection,
  FlagButtonCollection,
} from "src/components/Menu";

import "src/styles/buttons.scss";

type MenuProps = {
  onPlayClicked: () => void;
};

export default function Menu(props: MenuProps) {
  const {
    currentLanguage,
    setCurrentLanguage,
    getLocalizedValue,
    supportedLanguages,
  } = useLocalization();

  const { stageIndex, setStageIndex, stageTitles } = useStages();
  const { seed, setSeed } = useSeed();

  const flagButtonClickHandler = (index: number) => {
    const newLanguage = supportedLanguages[index];
    setCurrentLanguage(newLanguage);
  };

  const onStageChanged = (evt: any) => {
    setStageIndex(evt.newValue);
  };

  const onSeedChanged = (newValue: string) => {
    setSeed(newValue);
  };

  const buttonRandomizeClickHandler = () => {
    setSeed(generateSeed());
  };

  const rootStyle: Style = {
    width: "100%",
    height: "100%",
    position: "absolute",
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    justifyContent: "center",
  };

  const centerStyle: Style = {
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    justifyContent: "center",
    translate: "0px -100px",
  };

  return (
    <div style={rootStyle}>
      <Logo />
      <Memes />
      <div style={centerStyle}>
        <PlayButton
          text={getLocalizedValue("button_play")}
          onClick={props.onPlayClicked}
        />
        <StageSection
          title={getLocalizedValue("label_stage")}
          index={stageIndex}
          stages={stageTitles}
          onChange={onStageChanged}
        />
        <SeedSection
          title={getLocalizedValue("label_seed")}
          seed={seed}
          onSeedChanged={onSeedChanged}
          onRandomizeClicked={buttonRandomizeClickHandler}
          buttonText={getLocalizedValue("button_randomize")}
        />
      </div>
      <FlagButtonCollection
        currentLanguage={currentLanguage}
        supportedLanguages={supportedLanguages}
        onClick={flagButtonClickHandler}
      />
    </div>
  );
}

function generateSeed() {
  const n = Math.floor(Math.random() * 1_000_000);
  return n.toString().padStart(8, "0");
}
