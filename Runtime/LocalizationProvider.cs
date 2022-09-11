using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace KalkuzSystems.Localization
{
    public class LocalizationProvider : MonoBehaviour
    {
        private static string LOCALIZATION_FOLDER_PATH => Path.Combine(Application.streamingAssetsPath, "Localization");
        private static string PLAYER_PREFS_LAST_LOCALE_KEY => "localization-preference";

        private static LocalizationProvider m_instance;
        
        private Dictionary<string, string> strings;
        
        private static Action m_onLocaleChanged;
        public static Action OnLocaleChanged
        {
            get => m_onLocaleChanged;
            set => m_onLocaleChanged = value;
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
        private static void EnsureDirectoryExistence()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                if (Directory.Exists(LOCALIZATION_FOLDER_PATH)) return;
            
                Debug.Log("StreamingAssets/Localization directory did not exist. Creating...");
                Directory.CreateDirectory(LOCALIZATION_FOLDER_PATH);
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
        
        private static string GetJsonPath(string localeID) => Path.Combine(LOCALIZATION_FOLDER_PATH, $"{localeID}.json");

        public static void ChangeLocale(string localeID)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                m_instance.StartCoroutine(m_instance.LoadLocalizationAssetWebRequest(localeID));
            }
            else
            {
                m_instance.LoadLocalizationAsset(localeID);
            }
            PlayerPrefs.SetString(PLAYER_PREFS_LAST_LOCALE_KEY, localeID);
        }

        private IEnumerator LoadLocalizationAssetWebRequest(string localeID)
        {
            var jsonPath = GetJsonPath(localeID);
            
            var json = "";
            var uwr = UnityWebRequest.Get(jsonPath);
            yield return uwr.SendWebRequest();
            
            json = uwr.downloadHandler.text;
            
            strings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            m_onLocaleChanged?.Invoke();
        }

        private void LoadLocalizationAsset(string localeID)
        {
            var jsonPath = GetJsonPath(localeID);

            var json = "";
            if (!File.Exists(jsonPath))
            {
                Debug.Log($"StreamingAssets/Localization/{localeID}.json is not found.");
                return;
            }
            
            using (StreamReader reader = new StreamReader(jsonPath))
            {
                json = reader.ReadToEnd();
            }
            
            strings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            m_onLocaleChanged?.Invoke();
        }
        public static string TryReadLocalizedString(string key)
        {
            if (m_instance == null) return "";
            
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
        public static List<string> GetLocales()
        {
            var dirs = Directory.GetFiles(LOCALIZATION_FOLDER_PATH);
            var files = dirs.Select((dir) => dir.Split(Path.DirectorySeparatorChar).Last());
            var locales = files.Where((f) => !f.Contains(".meta")).Select((f) => f.Split('.')[0]).ToList();
            return locales;
        }

        #region Menus

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Kalkuz Systems/Json Localization/Initialize Localization Assets")]
        static void InitializeLocalizationSystem()
        {
            EnsureDirectoryExistence();
            
            var jsonPath = GetJsonPath("en");
            if (!File.Exists(jsonPath))
            {
                Debug.Log("StreamingAssets/Localization/en.json is not found. Creating...");
                using (StreamWriter sw = File.CreateText(jsonPath))
                {
                    var dict = new Dictionary<string, string>()
                    {
                        {"example", "This is an example entry."},
                        {"start", "Start the game"}
                    };
                    var json = JsonConvert.SerializeObject(dict, Formatting.Indented);
                    
                    sw.Write(json);
                }
            }
            UnityEditor.AssetDatabase.Refresh();
        } 
#endif

        #endregion
    }
}
