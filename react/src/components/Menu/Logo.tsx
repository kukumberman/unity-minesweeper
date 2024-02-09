import { Style, UnityEngine } from "@reactunity/renderer";

export default function Logo() {
  const rootStyle: Style = {
    width: "100%",
    height: "100%",
    position: "absolute",
    alignItems: "center",
  };

  const sprite = Globals.SpriteStorage.Get("logo") as UnityEngine.Sprite;

  return (
    <div name="logo-flex-wrapper" style={rootStyle}>
      <image
        name="img-logo"
        source={sprite}
        style={{ translate: "0px 69px", pointerEvents: "none" }}
      />
    </div>
  );
}
