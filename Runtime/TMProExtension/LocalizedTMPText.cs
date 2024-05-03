using TMPro;
using UnityEngine;

namespace Kalkuz.Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTMPText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private string localizationKey;

        [Space, SerializeField] private bool listenLocalizationChanges = true;

        private void OnEnable()
        {
            if (listenLocalizationChanges) LocalizationProvider.OnLocaleChanged += OnLocaleChanged;
            
            SetLocalizedText();
        }
        private void OnDisable()
        {
            if (listenLocalizationChanges) LocalizationProvider.OnLocaleChanged -= OnLocaleChanged;
        }

        private void OnLocaleChanged()
        {
            SetLocalizedText();
        }

        public void SetLocalizedText()
        {
            text.text = LocalizationProvider.TryReadLocalizedString(localizationKey);
        }

        private void OnValidate()
        {
            if (text) return;
            text = GetComponent<TMP_Text>();
        }
    }
}