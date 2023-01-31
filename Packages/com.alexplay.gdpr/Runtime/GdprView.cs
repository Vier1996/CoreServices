using System;
using System.Linq;
using ACS.GDPR.com.alexplay.gdpr.Runtime.Config;
using DG.Tweening;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace ACS.GDPR.com.alexplay.gdpr.Runtime
{
    public class GdprView : MonoBehaviour
    {
        public GDPR GDPR => _gdpr;
        
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _termsOfUseText;
        [SerializeField] private TextMeshProUGUI _andArticleText;
        [SerializeField] private TextMeshProUGUI _privacyPoliceText;
        [SerializeField] private TextMeshProUGUI _acceptButtonText;
        [SerializeField] private TextMeshProUGUI _declineButtonText;
        
        [Space(10)]
        [Header("Buttons")]
        [SerializeField] private UnityEngine.UI.Button _usingLinkButton;
        [SerializeField] private UnityEngine.UI.Button _privacyLinkButton;
        [SerializeField] private UnityEngine.UI.Button _acceptButton;
        [SerializeField] private UnityEngine.UI.Button _declineButton;
        
        [Space(10)]
        [Header("GdprBackgroundCanvasGroup")]
        [SerializeField] private CanvasGroup _viewCanvasGroup;

        private const string _localizationConfigPath = "Gdpr/GdprTranslationConfig";
        private const string _defaultLanguageCode = "ru";
        private GDPR _gdpr;
        
        private void Awake()
        {
            _gdpr = new GDPR(_usingLinkButton, _privacyLinkButton, _acceptButton, _declineButton);

            BreakState();
            Localize();
            Animate();
        }

        public void CloseGdpr()
        {
            _gdpr.Dispose();
            Destroy(gameObject);
        }
        
        private void BreakState()
        {
            _viewCanvasGroup.transform.localScale = Vector3.zero;
            _viewCanvasGroup.alpha = 0;
        }
        
        private void Animate()
        {
            // _viewCanvasGroup.DOFade(1f, 0.2f);
            _viewCanvasGroup.transform.DOScale(1.1f, 0.1f).OnComplete(() => 
                _viewCanvasGroup.transform.DOScale(1f, 0.1f));
        }

        private void Localize()
        {
            GdprTranslationConfig translationConfig = Resources.Load<GdprTranslationConfig>(_localizationConfigPath);

            if (translationConfig == null)
                throw new Exception(
                    message: "Can't load gdpr translation config with equal name. Check Directory " +
                             "(Alexplay->GDPR->Resources->GdprConfigs). If config contains, check its name. " +
                             "Its name must be (GdprTranslationConfig)");

            string targetLanCode = LocalizationManager.CurrentLanguage == null ? _defaultLanguageCode : LocalizationManager.CurrentLanguage;
            GdprLanguage language = translationConfig.Localization.FirstOrDefault(lan => lan.LanguageCode == targetLanCode);

            if (language == default)
                throw new ArgumentException(
                    "Seem like current language not in container, write to my developer about it.");

            GpdrTranslations translations = language.Translations;
            
            _titleText.text = translations.TitleTranslation;
            _descriptionText.text = translations.DescriptionTranslation;
            _termsOfUseText.text = translations.TermsOfUseTranslation;
            _andArticleText.text = translations.AndArticleTranslation;
            _privacyPoliceText.text = translations.PrivacyPoliceTranslation;
            _acceptButtonText.text = translations.AcceptButtonTranslation;
            _declineButtonText.text = translations.DeclineButtonTranslation;
        }
    }
}
