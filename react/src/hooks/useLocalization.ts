import { useState } from "react";
import LocalizationManager from "src/core/LocalizationManager";

export default function useLocalization() {
  const localizationManager =
    Globals.LocalizationManager as LocalizationManager;
  const [currentLanguage, setCurrentLanguage] = useState(
    localizationManager.currentLanguage()
  );

  return {
    currentLanguage,
    setCurrentLanguage: (language: string) => {
      if (localizationManager.setCurrentLanguage(language)) {
        setCurrentLanguage(localizationManager.currentLanguage());
      }
    },
    getLocalizedValue: (key: string) => {
      return localizationManager.getLocalizedValue(key);
    },
    supportedLanguages: localizationManager.supportedLanguages(),
  };
}
