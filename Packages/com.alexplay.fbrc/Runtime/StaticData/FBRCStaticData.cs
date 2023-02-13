using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ACS.FBRC.StaticData
{
    [CreateAssetMenu(menuName = "Configs/RemoteConfigData")]
    public class FBRCStaticData : ScriptableObject
    {
        [Tooltip("Converts  generating property`s name from snake case to pascal")]
        public bool SnakeToPascal;
        public List<FBRemoteConfigValue> Values;
        public LoadStrategy LoadStrategy;

        #if UNITY_EDITOR
        public string GeneratedFilePath = "";
        [Sirenix.OdinInspector.FilePath(AbsolutePath = true), ShowInInspector, FoldoutGroup("ParseJson")] 
        private string _jsonPath;

        [Button]
        private void Regenerate() => 
            ValuesGenerator.Generate(this);

        [Button, FoldoutGroup("ParseJson")]
        private void ParseJson()
        { 
            Undo.RecordObject(this, "FBRC Parsing parameters");
            Values = FBRCParser.Parse(_jsonPath);
            EditorUtility.SetDirty(this);
        }
        #endif
    }
}