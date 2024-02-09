import { render } from "@reactunity/renderer";
import React from "react";
import AppContextProvider from "./components/AppContextProvider";
import App from "./components/App";
import LocalizationManager from "./core/LocalizationManager";

Interop.UnityEngine.Application.targetFrameRate = 60;

const spriteStorage = Globals.SpriteStorage;
spriteStorage.Initialize();

const localizationManager = new LocalizationManager();
localizationManager.fromJson(Globals.LocalizationJsonTextAsset.text);
Globals.LocalizationManager = localizationManager;

render(
  <React.StrictMode>
    <AppContextProvider>
      <App />
    </AppContextProvider>
  </React.StrictMode>
);
