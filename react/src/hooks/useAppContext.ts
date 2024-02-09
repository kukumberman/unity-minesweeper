import { createContext, useContext } from "react";
import { GameState } from "src/core/enums";

type AppCtx = {
  seed: string;
  setSeed: (newValue: string) => void;

  stageIndex: number;
  setStageIndex: (newValue: number) => void;

  gameState: GameState;
  setGameState: (newValue: GameState) => void;
};

export const AppContext = createContext<AppCtx>(null);

export default function useAppContext() {
  const context = useContext(AppContext);
  if (!context) {
    throw new Error("useAppContext must be used within an AppContextProvider");
  }
  return context;
}
