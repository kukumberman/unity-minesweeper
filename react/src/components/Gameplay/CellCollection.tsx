import { MinesweeperState } from "src/core/minesweeper/enums";
import { Cell } from "./Cell";

type CellCollectionProps = {
  state: MinesweeperState;
  cellSize: number;
  width: number;
  height: number;
  cells: MinesweeperCell[];
  onClick: (index: number) => void;
  onClickAsFlag: (index: number) => void;
};

export function CellCollection(props: CellCollectionProps) {
  const interactive = props.state === MinesweeperState.Playing;

  const cells = props.cells.map((cell, index) => (
    <Cell
      key={index}
      pixelSize={props.cellSize}
      data={cell}
      interactive={interactive}
      onClick={props.onClick}
      onClickAsFlag={props.onClickAsFlag}
    />
  ));

  return (
    <div
      style={{
        width: props.cellSize * props.width,
        height: props.cellSize * props.height,
        display: "flex",
        flexDirection: "row",
        flexWrap: "wrap",
      }}
    >
      {cells}
    </div>
  );
}
