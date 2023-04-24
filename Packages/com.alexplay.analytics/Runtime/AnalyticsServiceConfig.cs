using System;
using System.Collections.Generic;
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

            foreach (var t in availableAgents)
            {
                AgentInfo agentInfo = new AgentInfo(t);

                if (ActiveAgents.Contains(agentInfo) == false) 
                    ActiveAgents.Add(new AgentInfo(t));
            }
        }

        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}