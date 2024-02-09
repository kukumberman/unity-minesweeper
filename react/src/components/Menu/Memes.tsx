import { ReactUnity, Style, UnityEngine } from "@reactunity/renderer";
import { useEffect, useRef } from "react";
import "src/styles/Memes.scss";

export default function Memes() {
  const refs = useRef<ReactUnity.UGUI.UGUIComponent[]>(Array(3));

  useEffect(() => {
    setTimeout(() => {
      // Dima:
      // for unknown reason this property is not applied, set it manually
      refs.current[0].Style.translate = "0 40%";
      refs.current[1].Style.translate = "0 50%";
      refs.current[2].Style.translate = "-50% 0";
    }, 0);
  }, []);

  const rootStyle: Style = {
    width: "100%",
    height: "100%",
    position: "absolute",
  };

  const sprite1 = Globals.SpriteStorage.Get(
    "meme-Dog-Patron"
  ) as UnityEngine.Sprite;
  const sprite2 = Globals.SpriteStorage.Get(
    "meme-Dog-female_mirror"
  ) as UnityEngine.Sprite;
  const sprite3 = Globals.SpriteStorage.Get(
    "meme-Arihovich"
  ) as UnityEngine.Sprite;

  return (
    <div name="memes" style={rootStyle}>
      <image
        source={sprite1}
        className="meme"
        style={{
          position: "absolute",
          left: 100,
          top: "50%",
          width: 409,
          height: 610,
          translate: "0 40%",
        }}
        ref={(el) => (refs.current[0] = el)}
      />
      <image
        source={sprite2}
        className="meme"
        style={{
          position: "absolute",
          top: "50%",
          right: 100,
          width: 433,
          height: 577,
          translate: "0 50%",
        }}
        ref={(el) => (refs.current[1] = el)}
      />
      <div
        className="arihovich-outer"
        style={{
          position: "absolute",
          left: "69%",
          bottom: 0,
          width: 547,
          height: 456,
          scale: 0.69,
          transformOrigin: "50% 100%",
          translate: "-50% 0",
        }}
        ref={(el) => (refs.current[2] = el)}
      >
        <image
          className="arihovich-inner"
          source={sprite3}
          style={{
            width: "100%",
            height: "100%",
          }}
        />
      </div>
    </div>
  );
}
