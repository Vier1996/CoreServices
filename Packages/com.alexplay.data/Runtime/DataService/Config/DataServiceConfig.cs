using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ACS.Data.DataService.Model;
using Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Data.DataService.Config
{
    [Serializable]
    public class DataServiceConfig : ServiceConfigBase
    {
        private const string _sharpAssemblyName = "Assembly-CSharp";

        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.data";

        [ShowIf("@IsEnabled == true")] public bool EnableAutoSave = false;
        [ShowIf("@IsEnabled == true && EnableAutoSave")] public int AutoSaveDelay = 60;
        [ShowIf("@IsEnabled == true")]
        public List<ModelsInfo> ActiveModels = new List<ModelsInfo>();
        
#if UNITY_EDITOR
        [ShowIf("@_isEnabled == true"), GUIColor(0.5f, 1, 1), PropertyOrder(-1)]
        [Button] private void UpdateActiveModels()
        {
            Type dialogViewType = typeof(ProgressModel);
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().First(atr => atr.GetName().Name.Equals(_sharpAssemblyName));

            Type[] allTypes = assembly.GetTypes();
            Type[] subclasses = allTypes.Where(t => t.IsSubclassOf(dialogViewType)).ToArray();
            
            List<Type> availableAgents = subclasses.ToList();

            foreach (var t in availableAgents)
            {
                ModelsInfo agentInfo = new ModelsInfo(t);

                if (ActiveModels.Contains(agentInfo) == false) 
                    ActiveModels.Add(new ModelsInfo(t));
            }
        }

        [ShowIf("@_isEnabled == true"), GUIColor(0.5f, 1, 1), PropertyOrder(-1)]
        [Button]
        private void Clear()
        {
            ActiveModels.Clear();
            ActiveModels = new List<ModelsInfo>();
        }

        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
#endif
    }
}
