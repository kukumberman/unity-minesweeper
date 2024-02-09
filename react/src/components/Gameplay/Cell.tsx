import { Style, UnityEngine } from "@reactunity/renderer";
import { useEffect, useRef } from "react";
import "src/styles/Cell.scss";

const colors = [
  "",
  "#0000FF", // 1
  "#008000", // 2
  "#FF0000", // 3
  "#000080", // 4
  "#800000", // 5
  "#008080", // 6
  "#000000", // 7
  "#808080", // 8
];

type CellProps = {
  pixelSize: number;
  data: MinesweeperCell;
  interactive: boolean;
  onClick: (index: number) => void;
  onClickAsFlag: (index: number) => void;
};

export function Cell(props: CellProps) {
  const ref = useRef(null);

  useEffect(() => {
    setTimeout(() => {
      // Dima: see Memes.tsx
      ref.current.Style.translate = "-50% 50%";
      ref.current.Element.pickingMode = 1; // PickingMode.Ignore
    }, 0);
  }, []);

  function getBackgroundSprite() {
    const key = props.data.IsRevealed ? "cell_2" : "cell_1";
    return Globals.SpriteStorage.Get(key) as UnityEngine.Sprite;
  }

  function getForegroundSprite(): UnityEngine.Sprite {
    if (props.data.IsBomb && props.data.IsRevealed) {
      return Globals.SpriteStorage.Get("emoji_bomb");
    }
    if (props.data.IsFlag) {
      return Globals.SpriteStorage.Get("emoji_flag");
    }
    return null;
  }

  function getClassName() {
    if (!props.data.IsRevealed && props.interactive) {
      return "cell-hidden cell-interactive";
    }
    if (!props.data.IsRevealed) {
      return "cell-hidden";
    }
    return undefined;
  }

  const pointerUpHandler = (evt) => {
    if (evt.button === 0) {
      props.onClick(props.data.Index);
    } else if (evt.button === 1) {
      props.onClickAsFlag(props.data.Index);
    }
  };

  const colorIndex = props.data.BombNeighborCount;
  const visibleText =
    props.data.IsRevealed &&
    !props.data.IsBomb &&
    props.data.BombNeighborCount > 0;

  const absoluteStyle: Style = {
    position: "absolute",
    width: "100%",
    height: "100%",
  };

  const foregroundImageStyle: Style = {
    position: "absolute",
    width: "69%",
    height: "69%",
    left: "50%",
    top: "50%",
    translate: "-50% 50%",
  };

  const textStyle: Style = {
    position: "absolute",
    width: "100%",
    height: "90%",
    left: 0,
    top: 0,
    fontSize: getFontSize(props.pixelSize),
    textAlign: "center",
    verticalAlign: "middle",
    color: colors[colorIndex],
  };

  return (
    <div
      style={{ width: props.pixelSize, height: props.pixelSize }}
      onPointerUp={pointerUpHandler}
    >
      <div style={absoluteStyle}>
        <image
          style={absoluteStyle}
          source={getBackgroundSprite()}
          className={getClassName()}
        />
        <image
          style={foregroundImageStyle}
          source={getForegroundSprite()}
          ref={ref}
        />
      </div>
      {visibleText ? (
        <text style={textStyle}>{props.data.BombNeighborCount}</text>
      ) : null}
    </div>
  );
}

function getFontSize(pixelSize: number) {
  // Dima: 69 pixels per each 100 pixels in size
  const fontSizePx = (69 * pixelSize) / 100;
  return fontSizePx;
}
