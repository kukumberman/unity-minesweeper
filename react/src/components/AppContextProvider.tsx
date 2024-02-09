import { useState } from "react";
import { AppContext } from "src/hooks/useAppContext";
import { GameState } from "src/core/enums";

type AppContextProviderProps = {
  children: React.ReactNode;
};

export default function AppContextProvider(props: AppContextProviderProps) {
  const [seed, setSeed] = useState("champion");
  const [stageIndex, setStageIndex] = useState(0);
  const [gameState, setGameState] = useState(getInitialState());

  return (
    <AppContext.Provider
      value={{
        seed,
        setSeed,
        stageIndex,
        setStageIndex,
        gameState,
        setGameState,
      }}
    >
      {props.children}
    </AppContext.Provider>
  );
}

function getInitialState(): GameState {
  const start = Globals.StartBehaviour;
  if (start.OverrideInitialState) {
    return start.InitialState;
  }
  return GameState.Menu;
}
