using System;
using Config;
using Sirenix.OdinInspector;

namespace ACS.GDPR.Config
{
    [Serializable]
    public class GdprConfig : ServiceConfigBase
    {
        [ShowIf("@IsEnabled == true")]
        public int _globalVersionIndex = 0;
        [ShowIf("@IsEnabled == true")]
        public int _globalMajorIndex = 0;
        [ShowIf("@IsEnabled == true")]
        public int _globalMinorIndex = 1;
        
        protected override string PackageName => "com.alexplay.gdpr";
        public string GetVersion() => $"GDPR:{_globalVersionIndex}.{_globalMajorIndex}.{_globalMinorIndex}";
    }
}
