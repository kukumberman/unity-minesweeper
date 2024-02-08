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
