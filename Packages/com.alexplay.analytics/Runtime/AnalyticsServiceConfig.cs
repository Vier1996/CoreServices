using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Analytics
{
    [Serializable]
    public class AnalyticsServiceConfig : ServiceConfigBase
    {
        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.analytics";

        [ShowIf("@IsEnabled == true")]
        public List<AgentInfo> ActiveAgents = new List<AgentInfo>();
        
        [ShowIf("@_isEnabled == true")]
        [GUIColor(0.5f, 1, 1)]
        [PropertyOrder(-1)]
        [Button] private void UpdateAgents()
        {
            List<Type> availableAgents = AnalyticsUtils.GetAllAvailableAgents();
            
            for (int i = 0; i < availableAgents.Count; i++)
            {
                AgentInfo agentInfo = ActiveAgents.FirstOrDefault(agent => agent.TypeName == availableAgents[i].Name);

                if (agentInfo == default) 
                    ActiveAgents.Add(new AgentInfo(availableAgents[i]));
            }
        }

        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
    
    [Serializable]
    public class AgentInfo
    {
        [ReadOnly] public string TypeName;
        [ReadOnly] public string AssemblyName;
     
        public bool CanTrackEvents = true;
        
        public AgentInfo(Type type)
        {
            TypeName = type.Name;
            AssemblyName = type.Assembly.GetName().Name;
        }
    }
}