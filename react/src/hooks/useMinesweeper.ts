import { useEffect, useRef, useState } from "react";
import Random from "src/core/Random";
import MinesweeperService from "src/core/minesweeper/MinesweeperService";
import { MinesweeperState } from "src/core/minesweeper/enums";

function GetRemainingBombCount(service: MinesweeperService) {
  const baseCount = service.Game.BombCount;
  let flagCount = 0;

  for (let i = 0, length = service.Game.CellsRef.length; i < length; i++) {
    var cell = service.Game.CellsRef[i];

    if (cell.IsFlag) {
      flagCount += 1;
    }
  }

  return baseCount - flagCount;
}

export default function useMinesweeper(seed: string, stage: MinesweeperStage) {
  const ref = useRef(new MinesweeperService());
  const [counter, setCounter] = useState(0);
  const [elapsedTime, setElapsedTime] = useState(0);

  const service = ref.current;
  if (service.State === MinesweeperState.None) {
    service.StartGame(stage.Settings, Random.getHashCode(seed));
  }

  const tick = () => {
    if (service.State === MinesweeperState.Playing) {
      setElapsedTime((value) => value + 1);
    }
  };

  const restart = () => {
    setElapsedTime(0);
    service.Restart();
  };

  useEffect(() => {
    const id = setInterval(() => {
      tick();
    }, 1 * 1000);

    const callback = () => {
      setCounter((c) => c + 1);
    };

    service.AddListener(callback);

    return () => {
      clearInterval(id);
      service.RemoveListener(callback);
    };
  }, []);

  return {
    elapsedTime,
    remainingBombCount: GetRemainingBombCount(service),
    state: service.State,
    cells: service.Game.CellsRef,
    revealCell: service.RevealCell.bind(service),
    flagCell: service.FlagCell.bind(service),
    restart,
    refreshId: counter,
  };
}
