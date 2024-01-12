using System;
using Config;
using Constants;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif
using UnityEngine;

namespace ACS.Core
{
    [Serializable]
    public class CoreBootstrapOptions : ServiceConfigBase
    {
        private const string DocumentationURL = "https://docs.google.com/document/d/1PORrcDtGcskwvLADl1r7Vcs8eJlcicsKJyL8jW8-c-k/edit?usp=sharing";
        
        [Title("Alexplay Core", "Made by 'Mr.Vier & Leonovich'", TitleAlignment = TitleAlignments.Centered)]
        [PropertyOrder(-1)]
        [Button(ButtonSizes.Large)]
        public void OpenFcknAwesomeDocs() => Application.OpenURL(DocumentationURL);
        
        [PropertyOrder(-1)]
        [Button] 
        private void UpdatePackage()
        {
#if UNITY_EDITOR
            Client.Add(ACSConst.CorePackageURL);
#endif
        }

        [PropertySpace(SpaceBefore = 20)]
        public TargetFrameRateType FrameRateType;

        [ShowIf("@FrameRateType == TargetFrameRateType.CONSTANT")]
        public int TargetRate = 60;
        public int TweenersCapacity = 500;
        public int SequencesCapacity = 125;
        public bool IgnoreScreenOrientation = false;
        public ScreenOrientation ScreenOrientation = ScreenOrientation.Portrait;

        public void Validate() => _ignoreEnableButton = true;
    }

    public enum TargetFrameRateType { ADAPTIVE, CONSTANT, UNLIMITED, EQUALS }
}