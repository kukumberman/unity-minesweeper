import { ReactUnity } from "@reactunity/renderer";
import { View } from "@reactunity/renderer/ugui";

export interface RadioButtonGroup extends View<ReactUnity.UGUI.UGUIComponent> {
  choices: string[];
  value: number;
  onChange?: (evt: any) => void;
}

declare global {
  interface ReactUnityCustomElements {
    radioButtonGroup: RadioButtonGroup;
  }
}

declare global {
  type MinesweeperGameSettings = {
    Width: number;
    Height: number;
    BombCount: number;
  };
  type MinesweeperStage = {
    Name: string;
    Settings: MinesweeperGameSettings;
  };
  type MinesweeperCell = {
    Index: number;
    X: number;
    Y: number;
    BombNeighborCount: number;
    IsBomb: boolean;
    IsFlag: boolean;
    IsRevealed: boolean;
  };
}
