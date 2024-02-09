import { useReactiveValue } from "@reactunity/renderer";
import { useEffect } from "react";

export function useUnityUpdate(callback: () => void) {
  // eslint-disable-next-line
  const counter: number = useReactiveValue(
    Globals.UnityLifecycleBehaviour.OnUpdateEvent
  );
  callback();
}

export function useUnityUpdateStateless(callback: () => void) {
  useEffect(() => {
    const removeListener =
      Globals.UnityLifecycleBehaviour.OnUpdateEvent.AddListener(callback);
    return () => {
      removeListener();
    };
  }, [callback]);
}
