import { EventEmitter } from "events";
import IMinesweeperService from "./IMinesweeperService";
import MinesweeperGame from "./MinesweeperGame";
import { MinesweeperState } from "./enums";

const EVENT_CHANGED = "EVENT_CHANGED";

export default class MinesweeperService implements IMinesweeperService {
  public Game: MinesweeperGame;
  public State: MinesweeperState;

  private readonly eventEmitter: any;

  private settings: MinesweeperGameSettings;
  private seed: number;

  constructor() {
    this.eventEmitter = new EventEmitter();
    this.State = MinesweeperState.None;
  }

  public StartGame(settings: MinesweeperGameSettings, seed: number) {
    this.settings = settings;
    this.seed = seed;
    this.Game = new MinesweeperGame(settings);
    this.Game.Play(seed);
    this.State = MinesweeperState.Playing;
    this.DispatchStateChangedEvent();
  }

  public Restart() {
    this.StartGame(this.settings, this.seed);
  }

  public RevealCell(index: number) {
    if (this.State !== MinesweeperState.Playing) {
      return;
    }

    const cell = this.Game.CellsRef[index];

    if (cell.IsFlag) {
      return;
    }

    cell.IsRevealed = true;

    if (cell.IsBomb) {
      this.State = MinesweeperState.Defeat;
      this.RevealBombs();
      this.DispatchStateChangedEvent();
      return;
    } else if (cell.BombNeighborCount === 0) {
      this.FloodFill(cell);
    }

    this.CheckIfWin();

    this.DispatchStateChangedEvent();
  }

  public FlagCell(index: number) {
    if (this.State !== MinesweeperState.Playing) {
      return;
    }

    const cell = this.Game.CellsRef[index];

    if (!cell.IsRevealed) {
      cell.IsFlag = !cell.IsFlag;
    }

    this.CheckIfWin();

    this.DispatchStateChangedEvent();
  }

  private FloodFill(cell: MinesweeperCell) {
    const grid = this.Game.Grid;

    const visited = new Set<number>();
    const queue: number[] = [];
    queue.push(grid.ConvertTo1D(cell.X, cell.Y));

    const indexes: number[] = [];

    while (queue.length > 0) {
      const cellIndex = queue.shift();
      if (visited.has(cellIndex)) {
        continue;
      }
      visited.add(cellIndex);
      const nextCell = this.Game.CellsRef[cellIndex];
      if (nextCell.BombNeighborCount === 0) {
        grid.GetNeighboursNonAlloc(nextCell.X, nextCell.Y, indexes);
      }
      for (const index of indexes) {
        const neighborCell = this.Game.CellsRef[index];
        queue.push(neighborCell.Index);
        neighborCell.IsRevealed = true;
        neighborCell.IsFlag = false;
      }
      indexes.length = 0;
    }
  }

  private RevealBombs() {
    for (let i = 0, length = this.Game.CellsRef.length; i < length; i++) {
      const cell = this.Game.CellsRef[i];

      if (cell.IsBomb && !cell.IsFlag) {
        cell.IsRevealed = true;
      }
    }
  }

  private CheckIfWin() {
    if (this.IsAboutToWin()) {
      this.State = MinesweeperState.Win;
    }
  }

  private IsAboutToWin(): boolean {
    const length = this.Game.CellsRef.length;

    let revealedCount = 0;

    let bombFlagCount = 0;
    let totalFlagCount = 0;

    for (let i = 0; i < length; i++) {
      const cell = this.Game.CellsRef[i];

      if (cell.IsRevealed && !cell.IsBomb) {
        revealedCount += 1;
      } else if (!cell.IsRevealed && cell.IsBomb && cell.IsFlag) {
        bombFlagCount += 1;
      }

      if (cell.IsFlag) {
        totalFlagCount += 1;
      }
    }

    if (length - revealedCount === this.Game.BombCount) {
      return true;
    }

    if (
      bombFlagCount === this.Game.BombCount &&
      totalFlagCount === this.Game.BombCount
    ) {
      return true;
    }

    return length === revealedCount + bombFlagCount;
  }

  private DispatchStateChangedEvent() {
    this.eventEmitter.emit(EVENT_CHANGED);
  }

  public AddListener(callback: () => void) {
    this.eventEmitter.on(EVENT_CHANGED, callback);
  }

  public RemoveListener(callback: () => void) {
    this.eventEmitter.off(EVENT_CHANGED, callback);
  }
}
