import { Style } from "@reactunity/renderer";
import { useEffect, useRef } from "react";
import TutorialTip, { TutorialTipProps } from "./TutorialTip";

type TutorialPanelProps = {
  visible: boolean;
  tips: TutorialTipProps[];
  onClick: () => void;
};

export function TutorialPanel(props: TutorialPanelProps) {
  const panelRef = useRef(null);
  const buttonRef = useRef(null);

  useEffect(() => {
    // Dima: see Memes.tsx
    setTimeout(() => {
      panelRef.current.Style.translate = "0 50%";
      buttonRef.current.Style.translate = "50% 50%";
    }, 0);
  }, []);

  const panelStyle: Style = {
    width: 450,
    borderRadius: 20,
    backgroundColor: new Interop.UnityEngine.Color(0, 0, 0, 1 / 4),
    position: "absolute",
    left: props.visible ? 50 : -400,
    top: "50%",
    translate: "0 50%",
    transition: "left 0.5s ease",
  };

  const buttonStyle: Style = {
    width: 69,
    height: 69,
    position: "absolute",
    top: "50%",
    right: 0,
    translate: "50% 50%",
    border: "none",
    backgroundColor: "none",
    padding: 0,
  };

  return (
    <div name="help-panel" style={panelStyle} ref={panelRef}>
      <div name="list">
        {props.tips.map((tip, index) => (
          <TutorialTip key={index} sprite={tip.sprite} text={tip.text} />
        ))}
      </div>
      <button
        name="btn-help"
        style={buttonStyle}
        onClick={props.onClick}
        ref={buttonRef}
      >
        <image source={Globals.SpriteStorage.Get("emoji_sos")} />
      </button>
    </div>
  );
}
