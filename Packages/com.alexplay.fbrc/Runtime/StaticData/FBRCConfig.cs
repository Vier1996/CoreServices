using System;
using System.Collections.Generic;
using Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.FBRC.StaticData
{
    [Serializable]
    public class FBRCConfig : ServiceConfigBase
    {
        private const string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.fbrc";

        [Title("Firebase Remote Config", TitleAlignment = TitleAlignments.Centered)]
        [Tooltip("Converts  generating property`s name from snake case to pascal"), ShowIf("@IsEnabled")]
        public bool SnakeToPascal;
        [HideInInspector]
        public LoadStrategy LoadingStrategy;
        [ShowIf("@IsEnabled")]
        public float CacheExpirationInSeconds;
        [ShowIf("@IsEnabled")]
        public bool IsLoggingEnabled;
        [ShowIf("@IsEnabled"),
        ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Name")]
        public List<FBRemoteConfigValue> Values;

        #if UNITY_EDITOR
        [ShowIf("@IsEnabled")]
        public string GeneratedFilePath = "";

        [ShowIf("@IsEnabled")]
        [FilePath(AbsolutePath = true), ShowInInspector, FoldoutGroup("Tools")] 
        private string _jsonPath;

        [ShowIf("@IsEnabled"), Button, FoldoutGroup("Tools")]
        private void ParseJson() => 
            Values = FBRCParser.Parse(_jsonPath);
        
        [ShowIf("@IsEnabled"), Button, FoldoutGroup("Tools")]
        private void Regenerate() => 
            ValuesGenerator.Generate(this);
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
#endif
    }
}