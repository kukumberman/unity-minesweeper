import useAppContext from "./useAppContext";

export default function useGameState() {
  const { gameState, setGameState } = useAppContext();

  return {
    gameState,
    setGameState,
  };
}
