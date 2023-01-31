using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACS.GDPR.com.alexplay.gdpr.Runtime.Config
{
    [CreateAssetMenu(fileName = "GdprTranslationConfig", menuName = "GDPR/Translation/Config")]
    [Serializable]
    public class GdprTranslationConfig : ScriptableObject
    {
        public List<GdprLanguage> Localization = new List<GdprLanguage>();
    }

    [Serializable]
    public class GdprLanguage
    {
        public string LanguageCode;
        public GpdrTranslations Translations = new GpdrTranslations();
    }
    
    [Serializable]
    public class GpdrTranslations
    {
        public string TitleTranslation;
        public string DescriptionTranslation;
        public string TermsOfUseTranslation;
        public string AndArticleTranslation;
        public string PrivacyPoliceTranslation;
        public string AcceptButtonTranslation;
        public string DeclineButtonTranslation;
    }
}
