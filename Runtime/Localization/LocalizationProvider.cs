using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace KalkuzSystems.Localization
{
    public class LocalizationProvider : MonoBehaviour
    {
        private static string LOCALIZATION_FOLDER_PATH => Path.Combine(Application.dataPath, "Localization");
        private static string PLAYER_PREFS_LAST_LOCALE_KEY => "localization-preference";

        private static LocalizationProvider m_instance;
        
        private Dictionary<string, string> strings;
        
        private Action onLocaleChanged;
        public static Action OnLocaleChanged
        {
            get => m_instance.onLocaleChanged;
            set => m_instance.onLocaleChanged = value;
        }

        [SerializeField] private string defaultLocale = "en";
 
        private void Awake()
        {
            EnsureDirectoryExistence();
            EnsureInstanceExistence();
        }
        
        private IEnumerator Start()
        {
            yield return null;
            
            if (PlayerPrefs.HasKey(PLAYER_PREFS_LAST_LOCALE_KEY))
            {
                ChangeLocale(PlayerPrefs.GetString(PLAYER_PREFS_LAST_LOCALE_KEY, defaultLocale));
            }
            else
            {
                ChangeLocale(defaultLocale);
            }
        }
        
        private void EnsureInstanceExistence()
        {
            if (m_instance)
            {
                Debug.LogWarning($"There is already a {nameof(LocalizationProvider)} declared in scene. Destroying the duplicate one.", m_instance);
                Destroy(gameObject);
            }
            else m_instance = this;
        }
        private void EnsureDirectoryExistence()
        {
            if (Directory.Exists(LOCALIZATION_FOLDER_PATH)) return;
            
            Debug.Log("Assets/Localization directory did not exist. Creating...");
            Directory.CreateDirectory(LOCALIZATION_FOLDER_PATH);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
        
        private string GetJsonPath(string localeID) => Path.Combine(LOCALIZATION_FOLDER_PATH, $"{localeID}.json");

        public static void ChangeLocale(string localeID)
        {
            m_instance.LoadLocalizationAsset(localeID);
            PlayerPrefs.SetString(PLAYER_PREFS_LAST_LOCALE_KEY, localeID);
            
            m_instance.onLocaleChanged?.Invoke();
        }
        private void LoadLocalizationAsset(string localeID)
        {
            var jsonPath = GetJsonPath(localeID);
            if (!File.Exists(jsonPath))
            {
                Debug.Log($"{localeID}.json is not found in localization folder.");
                return;
            }
            
            string json;
            using (StreamReader reader = new StreamReader(jsonPath))
            {
                json = reader.ReadToEnd();
            }
            strings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
        public static string TryReadLocalizedString(string key)
        {
            if (m_instance.strings.TryGetValue(key, out string value))
            {
                return value;
            }
            else
            {
                Debug.LogWarning($"Key '{key}' is not found in the localization asset.");
                return "";
            }
        }
    }
}
