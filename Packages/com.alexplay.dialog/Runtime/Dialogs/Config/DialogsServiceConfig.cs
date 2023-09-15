using System;
using Config;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ACS.Dialog.Dialogs.Config
{
    [Serializable]
    public class DialogsServiceConfig : ServiceConfigBase
    {
        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.dialog";
        
        private const string LayerName = "Dialog";
        
        [ShowIf("@IsEnabled == true")] 
        public RenderMode RenderMode = RenderMode.ScreenSpaceCamera;
        
        [ShowIf("@IsEnabled == true")]
        public int DialogSortingOrder;
        
        [ShowIf("@IsEnabled == true")]
        public int ReferenceResolutionX = 720;
        
        [ShowIf("@IsEnabled == true")]
        public int ReferenceResolutionY = 1280;
        
        [ShowIf("@IsEnabled == true")]
        public float BaseScreenRatio = 16 / 9f;

        [ShowIf("@IsEnabled == true")] 
        public string DefaultResources = "Dialogs/";
        public string GetLayerName() => LayerName;

        public void OnValidate() => TryCreateLayer();

        private void TryCreateLayer()
        {
            #if UNITY_EDITOR
            var serializedObject = new SerializedObject(AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset"));
            var sortingLayers = serializedObject.FindProperty("m_SortingLayers");
            
            for (int i = 0; i < sortingLayers.arraySize; i++)
                if (sortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue.Equals(LayerName))
                    return;
            
            sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);
            var newLayer = sortingLayers.GetArrayElementAtIndex(sortingLayers.arraySize - 1);
            newLayer.FindPropertyRelative("name").stringValue = LayerName;
            newLayer.FindPropertyRelative("uniqueID").intValue = LayerName.GetHashCode();
            serializedObject.ApplyModifiedProperties();
            #endif
        }
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}
