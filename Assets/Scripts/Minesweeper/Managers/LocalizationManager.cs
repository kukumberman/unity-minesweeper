using System;
using System.Collections.Generic;
using System.Linq;
using Injection;
using UnityEngine;
using Newtonsoft.Json;

namespace Kukumberman.Minesweeper.Managers
{
    public sealed class LocalizationManager
    {
        public event Action OnLanguageChanged;

        [Inject]
        private MinesweeperGameModel _gameModel;

        private Dictionary<string, Dictionary<string, string>> _languagesMap = new();

        public string CurrentLanguage => _gameModel.PreferredLanguage;

        public List<string> SupportedLanguages => _languagesMap.Keys.ToList();

        public void CreateFromJson(string json)
        {
            _languagesMap = JsonConvert.DeserializeObject<
                Dictionary<string, Dictionary<string, string>>
            >(json);

            PreValidateMap();

            SetInitialLanguage();
        }

        public bool TrySetLanguage(string language)
        {
            if (_gameModel.PreferredLanguage == language)
            {
                return false;
            }

            if (_languagesMap.ContainsKey(language))
            {
                _gameModel.PreferredLanguage = language;
                _gameModel.Save();

                OnLanguageChanged.SafeInvoke();

                return true;
            }

            return false;
        }

        public string GetValue(string key)
        {
            if (
                _languagesMap.TryGetValue(CurrentLanguage, out var map)
                && map.TryGetValue(key, out var value)
            )
            {
                if (value.Length == 0)
                {
                    return key;
                }

                return value;
            }

            return key;
        }

        private void SetInitialLanguage()
        {
            var systemLanguage = Application.systemLanguage.ToString();

            if (_gameModel.PreferredLanguage == null)
            {
                _gameModel.PreferredLanguage = systemLanguage;
            }

            if (SupportedLanguages.Contains(_gameModel.PreferredLanguage))
            {
                OnLanguageChanged.SafeInvoke();
                return;
            }

            if (!TrySetLanguage(_gameModel.PreferredLanguage) && !TrySetLanguage(systemLanguage))
            {
                if (!TrySetLanguage(SupportedLanguages[0]))
                {
                    Debug.LogError("Failed to set intiial language");
                }
            }
        }

        private void PreValidateMap()
        {
            var tempKeys = new List<string>();

            foreach (var langAsKey in _languagesMap.Keys)
            {
                var languageMap = _languagesMap[langAsKey];

                tempKeys.AddRange(languageMap.Keys);

                foreach (var key in tempKeys)
                {
                    var value = languageMap[key];
                    if (
                        string.IsNullOrEmpty(value)
                        || string.IsNullOrWhiteSpace(value)
                        || value.Trim().Length == 0
                    )
                    {
                        languageMap[key] = string.Empty;
                    }
                }

                tempKeys.Clear();
            }
        }
    }
}
