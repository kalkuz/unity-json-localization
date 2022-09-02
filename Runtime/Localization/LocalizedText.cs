using System;
using UnityEngine;
using UnityEngine.UI;

namespace KalkuzSystems.Localization
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private string localizationKey;

        [Space, SerializeField] private bool listenLocalizationChanges;

        private void OnEnable()
        {
            if (listenLocalizationChanges) LocalizationProvider.OnLocaleChanged += OnLocaleChanged;
        }
        private void OnDisable()
        {
            if (listenLocalizationChanges) LocalizationProvider.OnLocaleChanged -= OnLocaleChanged;
        }

        private void OnLocaleChanged()
        {
            text.text = LocalizationProvider.TryReadLocalizedString(localizationKey);
        }

        private void OnValidate()
        {
            if (text) return;
            text = GetComponent<Text>();
        }
    }
}