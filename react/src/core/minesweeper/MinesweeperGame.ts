import Grid2D from "src/core/Grid2D";
import Random from "src/core/Random";

export default class MinesweeperGame {
  public readonly CellsRef: MinesweeperCell[];
  public readonly BombCount: number;
  public readonly Grid: Grid2D;
  private random: Random;

  public constructor(settings: MinesweeperGameSettings) {
    this.Grid = new Grid2D(settings.Width, settings.Height);
    this.CellsRef = new Array(this.Width * this.Height);
    this.BombCount = settings.BombCount;

    this.FillEmptyCells();
  }

  get Height() {
    return this.Grid.Height;
  }

  get Width() {
    return this.Grid.Width;
  }

  public Play(seed: number) {
    this.random = new Random(seed);
    this.FillRandomBombs();
  }

  private FillEmptyCells() {
    for (let i = 0; i < this.CellsRef.length; i++) {
      const { x, y } = this.Grid.ConvertTo2D(i);

      this.CellsRef[i] = {
        Index: i,
        X: x,
        Y: y,
        BombNeighborCount: 0,
        IsBomb: false,
        IsFlag: false,
        IsRevealed: false,
      };
    }
  }

  private FillRandomBombs() {
    Random.shuffle(this.CellsRef, this.random);

    for (let i = 0; i < this.BombCount; i++) {
      this.CellsRef[i].IsBomb = true;
    }

    this.CellsRef.sort(MinesweeperGame.CellComparisonByIndex);

    const indexes = [];

    for (let i = 0; i < this.CellsRef.length; i++) {
      indexes.length = 0;
      const cell = this.CellsRef[i];
      this.Grid.GetNeighboursNonAlloc(cell.X, cell.Y, indexes);

      for (let j = 0; j < indexes.length; j++) {
        const idx = indexes[j];
        if (this.CellsRef[idx].IsBomb) {
          cell.BombNeighborCount += 1;
        }
      }
    }
  }

  private static CellComparisonByIndex(
    lhs: MinesweeperCell,
    rhs: MinesweeperCell
  ) {
    return lhs.Index - rhs.Index;
  }
}
