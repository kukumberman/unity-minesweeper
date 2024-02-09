import { useEffect, useRef, useState } from "react";
import { useLocalization, useSeed, useStages, useMinesweeper } from "src/hooks";
import { useUnityUpdateStateless } from "src/hooks/useUnity";
import { MenuButton } from "src/components/Gameplay/MenuButton";
import { Toolbar } from "src/components/Gameplay/Toolbar";
import { TutorialTipProps } from "src/components/Gameplay/TutorialTip";
import { TutorialPanel } from "src/components/Gameplay/TutorialPanel";
import { CellCollection } from "src/components/Gameplay/CellCollection";
import Mathf from "src/core/Mathf";

import "src/styles/buttons.scss";

const tutorialTips = ["tutorial_1", "tutorial_2", "tutorial_3", "tutorial_4"];

const kCellSize = 100;
const kZoom = {
  initial: 1,
  min: 0.5,
  max: 2,
  sensitivity: 0.2,
};

type GameplayProps = {
  onMenuClicked: () => void;
};

// Dima:
// UIToolkit only (because of events handlers and VisualElement instance properties)
export default function Gameplay(props: GameplayProps) {
  const { getLocalizedValue } = useLocalization();
  const { seed } = useSeed();
  const { currentStage } = useStages();

  const [panelVisible, setPanelVisible] = useState(false);
  const {
    elapsedTime,
    remainingBombCount,
    state,
    cells,
    revealCell,
    flagCell,
    restart,
  } = useMinesweeper(seed, currentStage);

  const variables = useRef({
    toolbar: null,
    dragReceiver: null,
    draggableTarget: null,
    zoomScale: kZoom.initial,
    isDragging: false,
    clickRelativeOffset: {
      x: 0,
      y: 0,
    },
  });

  useUnityUpdateStateless(() => {
    handleScroll();
  });

  useEffect(() => {
    const { draggableTarget, zoomScale } = variables.current;
    draggableTarget.Style.scale = zoomScale;
  }, []);

  const onPointerDown = (evt) => {
    if (evt.button === 2) {
      variables.current.isDragging = true;

      const { draggableTarget, clickRelativeOffset } = variables.current;
      const worldRect = draggableTarget.Element.layout;
      clickRelativeOffset.x = worldRect.x - evt.position.x;
      clickRelativeOffset.y = worldRect.y - evt.position.y;
    }
  };

  const onPointerUp = (evt) => {
    if (evt.button === 2) {
      variables.current.isDragging = false;
    }
  };

  const onPointerMove = (evt) => {
    if (variables.current.isDragging) {
      handleDrag(evt.position);
    }
  };

  const onToolbarClicked = () => {
    restart();
  };

  const onCellClicked = (index: number) => {
    revealCell(index);
  };

  const onCellClickedAsFlag = (index: number) => {
    flagCell(index);
  };

  function handleDrag(position) {
    const { draggableTarget, clickRelativeOffset } = variables.current;
    position.x += clickRelativeOffset.x;
    position.y += clickRelativeOffset.y;

    position = clampPosition(position);

    draggableTarget.Style.left = position.x;
    draggableTarget.Style.top = position.y;
  }

  function clampPosition(position) {
    const { draggableTarget, zoomScale, dragReceiver } = variables.current;

    const elementScale = zoomScale;

    const elementSize = {
      x: draggableTarget.ClientWidth,
      y: draggableTarget.ClientHeight,
    };

    const elementHalfSize = {
      x: elementSize.x * 0.5,
      y: elementSize.y * 0.5,
    };

    const cellPadding = {
      x: kCellSize * elementScale,
      y: kCellSize * elementScale,
    };

    const scaleOffset = {
      x: elementHalfSize.x * (1 - elementScale),
      y: elementHalfSize.y * (1 - elementScale),
    };

    const min = {
      x: 0 - elementSize.x,
      y: 0 - elementSize.y,
    };
    min.x += scaleOffset.x;
    min.y += scaleOffset.y;
    min.x += cellPadding.x;
    min.y += cellPadding.y;

    const max = {
      x: dragReceiver.ClientWidth,
      y: dragReceiver.ClientHeight,
    };

    max.x -= scaleOffset.x;
    max.y -= scaleOffset.y;
    max.x -= cellPadding.x;
    max.y -= cellPadding.y;

    const toolbarHeight = variables.current.toolbar.Element.layout.height;
    max.y -= toolbarHeight * elementScale;

    position.x = Mathf.clamp(position.x, min.x, max.x);
    position.y = Mathf.clamp(position.y, min.y, max.y);

    return position;
  }

  function handleScroll() {
    const scrollDirection = Interop.UnityEngine.Input.mouseScrollDelta.y;

    if (scrollDirection !== 0) {
      let zoomScale = variables.current.zoomScale;
      zoomScale += scrollDirection * kZoom.sensitivity;
      zoomScale = Mathf.clamp(zoomScale, kZoom.min, kZoom.max);
      variables.current.zoomScale = zoomScale;
      variables.current.draggableTarget.Style.scale = zoomScale;
    }
  }

  function convertCurrentMousePositionLegacyInput() {
    const Screen = Interop.UnityEngine.Screen;
    const Mathf = Interop.UnityEngine.Mathf;
    const Input = Interop.UnityEngine.Input;

    const rect = variables.current.dragReceiver.Element.layout;
    const screenSize = rect.size;

    const mousePosition = Input.mousePosition;

    mousePosition.x = Mathf.InverseLerp(0, Screen.width, mousePosition.x);
    mousePosition.y = Mathf.InverseLerp(0, Screen.height, mousePosition.y);

    mousePosition.x = Mathf.Lerp(0, screenSize.x, mousePosition.x);
    mousePosition.y = Mathf.Lerp(0, screenSize.y, mousePosition.y);

    mousePosition.y = screenSize.y - mousePosition.y;

    return mousePosition;
  }

  const tips: TutorialTipProps[] = tutorialTips.map((id) => {
    return {
      sprite: Globals.SpriteStorage.Get(id),
      text: getLocalizedValue(id),
    };
  });

  return (
    <div
      style={{ position: "absolute", width: "100%", height: "100%" }}
      ref={(el) => (variables.current.dragReceiver = el)}
      onPointerDown={onPointerDown}
      onPointerUp={onPointerUp}
      onPointerMove={onPointerMove}
    >
      <div
        style={{
          width: "100%",
          height: "100%",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
        }}
      >
        <div
          style={{
            position: "absolute",
            top: 25,
            transition: "scale 0.1s linear",
          }}
          ref={(el) => (variables.current.draggableTarget = el)}
          name="draggable"
        >
          <Toolbar
            ref={(el) => (variables.current.toolbar = el)}
            onClick={onToolbarClicked}
            state={state}
            bombCount={remainingBombCount}
            elapsedTime={elapsedTime}
          />
          {/* Dima: todo - dont re-render CellCollection when elapsedTime is updated. */}
          <CellCollection
            state={state}
            cellSize={kCellSize}
            width={currentStage.Settings.Width}
            height={currentStage.Settings.Height}
            onClick={onCellClicked}
            onClickAsFlag={onCellClickedAsFlag}
            cells={cells}
          />
        </div>
      </div>
      <TutorialPanel
        visible={panelVisible}
        tips={tips}
        onClick={() => setPanelVisible(!panelVisible)}
      />
      <MenuButton
        onClick={props.onMenuClicked}
        text={getLocalizedValue("button_menu")}
      />
    </div>
  );
}
