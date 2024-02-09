import { useLocalization } from "src/hooks";
import { FlagButton } from "./FlagButton";

function getFlagSprite(language: string) {
  const key = `flag_${language}`;
  return Globals.SpriteStorage.Get(key);
}

type FlagButtonCollectionProps = {
  currentLanguage: string;
  supportedLanguages: string[];
  onClick: (index: number) => void;
};

export default function FlagButtonCollection(props: FlagButtonCollectionProps) {
  const { getLocalizedValue } = useLocalization();

  function getTitle() {
    const arg0 = getLocalizedValue("label_language");
    const arg1 = props.currentLanguage;
    return `${arg0} (${arg1})`;
  }

  return (
    <div
      style={{
        position: "absolute",
        left: 50,
        bottom: 50,
      }}
    >
      <label style={{ fontSize: 30, fontWeight: "bold", marginBottom: 20 }}>
        {getTitle()}
      </label>
      <div style={{ display: "flex", flexDirection: "row" }}>
        {props.supportedLanguages.map((name, index) => (
          <FlagButton
            key={name}
            index={index}
            isSelected={name === props.currentLanguage}
            sprite={getFlagSprite(name)}
            outlineSprite={Globals.SpriteStorage.Get("flag_Outline")}
            onClick={props.onClick}
          />
        ))}
      </div>
    </div>
  );
}
