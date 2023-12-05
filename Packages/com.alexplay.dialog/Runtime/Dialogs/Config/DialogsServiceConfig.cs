using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ACS.Dialog.Dialogs.View;
using Config;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ACS.Dialog.Dialogs.Config
{
    [Serializable]
    public class DialogsServiceConfig : ServiceConfigBase
    {
        private const string _sharpAssemblyName = "Assembly-CSharp";

        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.dialog";
        
        private const string LayerName = "Dialog";
        
        [ShowIf("@IsEnabled == true")] 
        public RenderMode RenderMode = RenderMode.ScreenSpaceCamera;
        
        [ShowIf("@IsEnabled == true")]
        public int DialogSortingOrder;
        
        [ShowIf("@IsEnabled == true")]
        public bool ResizeOnSceneChanged = true;

        [ShowIf("@IsEnabled == true")]
        public int ReferenceResolutionX = 720;
        
        [ShowIf("@IsEnabled == true")]
        public int ReferenceResolutionY = 1280;
        
        [ShowIf("@IsEnabled == true")]
        public float BaseScreenRatio = 16 / 9f;

        [ShowIf("@IsEnabled == true")] 
        public string DefaultResources = "Dialogs/";
        
        [ShowIf("@IsEnabled == true")]
        public List<DialogInfo> ActiveDialogs = new List<DialogInfo>();
        
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
        
#if UNITY_EDITOR
        [ShowIf("@_isEnabled == true"), GUIColor(0.5f, 1, 1), PropertyOrder(-1)]
        [Button] private void UpdateActiveDialogs()
        {
            Type dialogViewType = typeof(DialogView);
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().First(atr => atr.GetName().Name.Equals(_sharpAssemblyName));

            Type[] allTypes = assembly.GetTypes();
            Type[] subclasses = allTypes.Where(t => t.IsSubclassOf(dialogViewType)).ToArray();
            
            List<Type> availableAgents = subclasses.ToList();

            foreach (var t in availableAgents)
            {
                DialogInfo agentInfo = new DialogInfo(t);

                if (ActiveDialogs.Contains(agentInfo) == false) 
                    ActiveDialogs.Add(new DialogInfo(t));
            }
        }

        [ShowIf("@_isEnabled == true"), GUIColor(0.5f, 1, 1), PropertyOrder(-1)]
        [Button]
        private void Clear()
        {
            ActiveDialogs.Clear();
            ActiveDialogs = new List<DialogInfo>();
        }

        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
#endif
    }
}
