export default class LocalizationManager {
  private readonly _map: Map<string, Map<string, string>>;
  private _currentLanguage: string;
  private _supportedLanguages: string[];

  constructor() {
    this._map = new Map<string, Map<string, string>>();
  }

  fromJson(json: string) {
    const data = JSON.parse(json);
    for (const key in data) {
      const map = new Map<string, string>(Object.entries(data[key]));
      this._map.set(key, map);
    }
    this._supportedLanguages = Array.from(this._map.keys());
    this._currentLanguage = this._supportedLanguages[0];
  }

  supportedLanguages() {
    return this._supportedLanguages;
  }

  currentLanguage() {
    return this._currentLanguage;
  }

  setCurrentLanguage(language: string) {
    if (!this._supportedLanguages.includes(language)) {
      return false;
    }

    if (this._currentLanguage === language) {
      return false;
    }

    this._currentLanguage = language;

    return true;
  }

  getLocalizedValue(key: string) {
    if (!this._map.has(this._currentLanguage)) {
      return key;
    }

    const map = this._map.get(this._currentLanguage);
    if (map.has(key)) {
      return map.get(key);
    }

    return key;
  }
}
