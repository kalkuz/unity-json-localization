using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace KalkuzSystems.Localization
{
    internal class LocaleEditor : EditorWindow
    {
        private static string LOCALIZATION_FOLDER_PATH => Path.Combine(Application.streamingAssetsPath, "Localization");
        private static string GetJsonPath(string localeID) => Path.Combine(LOCALIZATION_FOLDER_PATH, $"{localeID}.json");

        private int localeIndex;
        private Dictionary<string, string> dict;
        private Vector2 scroll;

        [MenuItem("Kalkuz Systems/Json Localization/Locale Editor")]
        static void Init()
        {
            LocaleEditor window = GetWindow<LocaleEditor>();

            window.minSize = new Vector2(600, 600);
            window.titleContent = new GUIContent("Locale Editor");
            window.Show();
        }

        void OnGUI()
        {
            var locales = LocalizationProvider.GetLocales().ToArray();
            localeIndex = EditorGUILayout.Popup("Locale", localeIndex, locales);
            var localeID = locales[localeIndex];

            if (GUILayout.Button("Load"))
            {
                var json = File.ReadAllText(GetJsonPath(localeID));
                dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            if (dict != null && GUILayout.Button("Save"))
            {
                var json = JsonConvert.SerializeObject(dict, Formatting.Indented);
                using (StreamWriter sw = File.CreateText(GetJsonPath(localeID)))
                {
                    sw.Write(json);
                }
            }
            
            EditorGUILayout.Space(20f);

            if (dict != null)
            {
                scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(480f));
                foreach (var kvp in dict.ToList())
                {
                    EditorGUILayout.BeginHorizontal();

                    var key = kvp.Key;
                    var val = kvp.Value;
                    
                    if (GUILayout.Button("-", GUILayout.MaxWidth(30f), GUILayout.MinHeight(22f)))
                    {
                        dict = dict.Where(x => x.Key != key).ToDictionary(x => x.Key, x => x.Value);
                        break;
                    }
                    
                    key = EditorGUILayout.DelayedTextField(key, GUILayout.MaxWidth(150f), GUILayout.MinHeight(22f));
                    val = EditorGUILayout.TextField(val, GUILayout.MinHeight(22f));

                    if (key != kvp.Key)
                    {
                        dict = dict
                            .Select(x => new KeyValuePair<string, string>(x.Key == kvp.Key ? key : x.Key, x.Value))
                            .ToDictionary(x => x.Key, x => x.Value);
                    }
                    else dict[key] = val;
                    
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
                
                EditorGUILayout.Space(20f);

                GUI.enabled = !dict.ContainsKey("");
                
                if (!GUI.enabled)
                {
                    EditorGUILayout.LabelField("There is empty keys remaining. Fix them before adding new entry.");
                }
                else
                {
                    EditorGUILayout.Space(20f);
                }
                
                if (GUILayout.Button("Add Entry"))
                {
                    dict.Add("", "");
                }
                
                GUI.enabled = true;
            }
        }
    }
}