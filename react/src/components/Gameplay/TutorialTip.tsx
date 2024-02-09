import { UnityEngine } from "@reactunity/renderer";

export type TutorialTipProps = {
  sprite: UnityEngine.Sprite;
  text: string;
};

export default function TutorialTip(props: TutorialTipProps) {
  return (
    <div
      style={{
        display: "flex",
        flexDirection: "row",
        alignItems: "center",
        height: 150,
      }}
    >
      <image
        source={props.sprite}
        style={{ scale: 0.5, width: 215, height: 215, translate: "-25px 0" }}
      />
      <text
        style={{
          fontSize: 25,
          width: 275,
          color: "white",
          translate: "-60px 0",
        }}
      >
        {props.text}
      </text>
    </div>
  );
}
