import { ReactUnity, UnityEngine } from "@reactunity/renderer";
import { useEffect, useRef } from "react";
import { useUnityUpdateStateless } from "src/hooks/useUnity";
import "src/styles/FlagButton.scss";

type FlagButtonProps = {
  index: number;
  isSelected: boolean;
  sprite: UnityEngine.Sprite;
  outlineSprite: UnityEngine.Sprite;
  onClick: (index: number) => void;
};

export function FlagButton(props: FlagButtonProps) {
  const ref = useRef<ReactUnity.UGUI.ButtonComponent>(null);
  const imageRef = useRef<ReactUnity.UGUI.ImageComponent>(null);

  useEffect(() => {
    const desiredHeight = 87;

    const rectSize = props.sprite.rect.size;
    const aspect = rectSize.x / rectSize.y;

    const component = ref.current;
    component.Style.height = desiredHeight;
    component.Style.width = desiredHeight * aspect;
  }, []);

  useUnityUpdateStateless(() => {
    if (imageRef.current !== null && props.isSelected) {
      const image = imageRef.current;
      image.Style.color = Globals.GradientColorBehaviour.CurrentColor;
    }
  });

  const onClick = () => {
    props.onClick(props.index);
  };

  return (
    <button
      onClick={onClick}
      ref={ref}
      style={{
        margin: "0 10",
        backgroundColor: "none",
        border: "none",
        padding: 0,
      }}
      className="flag-animated"
    >
      <image
        ref={imageRef}
        source={props.outlineSprite}
        style={{
          color: "red",
          position: "absolute",
          opacity: props.isSelected ? 1 : 0,
          width: "100%",
          height: "100%",
        }}
      />
      <image source={props.sprite} style={{ width: "100%", height: "100%" }} />
    </button>
  );
}
