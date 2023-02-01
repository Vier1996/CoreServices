using System;
using Config;
using Sirenix.OdinInspector;
using UnityEditor;

namespace ACS.Dialog.Dialogs.Config
{
    [Serializable]
    public class DialogsServiceConfig : ServiceConfigBase
    {
        private const string LayerName = "Dialog";
        
        [ShowIf("@IsEnabled == true")]
        public int DialogSortingOrder;
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
    }
}
