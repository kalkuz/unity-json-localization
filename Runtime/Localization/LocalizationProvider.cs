using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace KalkuzSystems.Localization
{
    public class LocalizationProvider : MonoBehaviour
    {
        private static string LOCALIZATION_FOLDER_PATH => Path.Combine(Application.dataPath, "Localization");

        private static LocalizationProvider m_instance;
        private Action onLocaleChanged;

        public static Action OnLocaleChanged => m_instance.onLocaleChanged;
        
        private void Awake()
        {
            EnsureDirectoryExistence();
            EnsureInstanceExistence();
        }
        
        private IEnumerator Start()
        {
            yield return null;
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

        #region Test Area

        [ContextMenu("Test")]
        public void Test()
        {
            EnsureDirectoryExistence();

            var englishPath = GetJsonPath("en");
            if (!File.Exists(englishPath))
            {
                Debug.Log("en.json is not found. Creating one...");
                File.CreateText(englishPath); 
                
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
        }

        #endregion
    }
}
