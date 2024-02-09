import { Menu, Gameplay } from "src/pages";
import { useGameState } from "src/hooks";
import { GameState } from "src/core/enums";

export default function App() {
  const { gameState, setGameState } = useGameState();

  return (
    <div style={{ width: "100%", height: "100%" }}>
      {gameState === GameState.Menu ? (
        <Menu onPlayClicked={() => setGameState(GameState.Gameplay)} />
      ) : (
        <Gameplay onMenuClicked={() => setGameState(GameState.Menu)} />
      )}
    </div>
  );
}
