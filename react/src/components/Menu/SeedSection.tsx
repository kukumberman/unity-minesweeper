import { ReactUnity } from "@reactunity/renderer";
import { useRef } from "react";

type SeedSectionProps = {
  title: string;
  seed: string;
  buttonText: string;
  onSeedChanged: (newValue: string) => void;
  onRandomizeClicked: () => void;
};

export default function SeedSection(props: SeedSectionProps) {
  const inputFieldRef = useRef<ReactUnity.UGUI.InputComponent>(null);

  const handleSeedValueChanged = (evt) => {
    const value = evt.newValue;
    props.onSeedChanged(value);
  };

  const fontSize = 25;

  return (
    <div name="container-seed" style={{ marginTop: 25 }}>
      <text
        name="label-seed"
        style={{
          fontSize: 30,
          fontStyle: "bold",
          textAlign: "center",
          verticalAlign: "middle",
        }}
      >
        {props.title}
      </text>
      <div style={{ display: "flex", flexDirection: "row" }}>
        <input
          name="input-seed"
          value={props.seed}
          ref={inputFieldRef}
          onChange={handleSeedValueChanged}
          style={{ width: 200, fontSize: fontSize }}
        />
        <button
          name="btn-randomize-seed"
          onClick={props.onRandomizeClicked}
          className="btn bg-danger-hoverable text-white-hoverable"
          style={{ borderRadius: 6 }}
        >
          <text style={{ height: "100%", fontSize: fontSize }}>
            {props.buttonText}
          </text>
        </button>
      </div>
    </div>
  );
}
