import MinesweeperGame from "./MinesweeperGame";
import { MinesweeperState } from "./enums";

export default interface IMinesweeperService {
  Game: MinesweeperGame;
  State: MinesweeperState;
  StartGame(settings: MinesweeperGameSettings, seed: number): void;
  Restart(): void;
  RevealCell(index: number): void;
  FlagCell(index: number): void;
}
