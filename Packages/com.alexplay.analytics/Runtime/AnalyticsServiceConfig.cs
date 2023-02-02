using System;
using Config;
using Sirenix.OdinInspector;

namespace ACS.Analytics
{
    [Serializable]
    public class AnalyticsServiceConfig : ServiceConfigBase
    {
        [ShowIf("@IsEnabled == true")]
        public AgentInfo[] Agents;
        
        [Serializable]
        public class AgentInfo
        {
            public string TypeName;
            public string AssemblyName = "Assembly-CSharp";
        }
    }
}