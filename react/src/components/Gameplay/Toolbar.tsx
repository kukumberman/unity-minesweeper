import { Style } from "@reactunity/renderer";
import { forwardRef } from "react";
import { MinesweeperState } from "src/core/minesweeper/enums";

type ToolbarProps = {
  bombCount: number;
  elapsedTime: number;
  state: MinesweeperState;
  onClick: () => void;
};

export const Toolbar = forwardRef(function Toolbar(
  props: ToolbarProps,
  ref: any
) {
  const spriteBomb = Globals.SpriteStorage.Get("emoji_bomb");
  const spriteState = Globals.SpriteStorage.Get(getImageKey(props.state));
  const spriteStopwatch = Globals.SpriteStorage.Get("emoji_stopwatch");

  const sideImageStyle: Style = {
    width: 80,
    height: 80,
  };

  const centerImageStyle: Style = {
    width: 100,
    height: 100,
  };

  const imageParentStyle: Style = {
    display: "flex",
    flexDirection: "row",
    alignItems: "center",
  };

  const textStyle: Style = {
    width: 300,
    fontSize: 100,
  };

  return (
    <div
      ref={ref}
      style={{
        width: "100%",
        display: "flex",
        flexDirection: "row",
        alignItems: "center",
        justifyContent: "space-between",
      }}
    >
      <div style={imageParentStyle}>
        <image source={spriteBomb} style={sideImageStyle} />
        <text style={{ ...textStyle, textAlign: "left" }}>
          {props.bombCount.toString()}
        </text>
      </div>
      <button onClick={props.onClick} style={{ borderRadius: 5 }}>
        <image source={spriteState} style={centerImageStyle} />
      </button>
      <div style={imageParentStyle}>
        <text style={{ ...textStyle, textAlign: "right" }}>
          {props.elapsedTime.toString()}
        </text>
        <image source={spriteStopwatch} style={sideImageStyle} />
      </div>
    </div>
  );
});

function getImageKey(state: MinesweeperState) {
  if (state === MinesweeperState.Playing) {
    return "emoji_playing";
  }
  if (state === MinesweeperState.Win) {
    return "emoji_win";
  }
  if (state === MinesweeperState.Defeat) {
    return "emoji_defeat";
  }
  return "null";
}
