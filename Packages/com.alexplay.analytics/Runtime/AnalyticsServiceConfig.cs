using System;
using System.Collections.Generic;
using System.Reflection;
using Config;
using Sirenix.OdinInspector;

namespace ACS.Analytics
{
    [Serializable]
    public class AnalyticsServiceConfig : ServiceConfigBase
    {
        private const string _analyticsAssemblyName = "ACS.Analytics";
        private const string _sharpAssemblyName = "Assembly-CSharp";
        
        [ShowIf("@IsEnabled == true")]
        public List<AgentInfo> ActiveAgents = new List<AgentInfo>();
        
        [ShowIf("@_isEnabled == true")]
        [GUIColor(0.5f, 1, 1)]
        [PropertyOrder(-1)]
        [Button] private void UpdateAgents()
        {
            List<Type> agents = new List<Type>();

            GetAgentsByAssembly(_analyticsAssemblyName, ref agents);
            GetAgentsByAssembly(_sharpAssemblyName, ref agents);

            for (int i = 0; i < agents.Count; i++)
            {
                AgentInfo agentInfo = ActiveAgents.Find(agent => agent.GetType() == agents[i]);

                if (agentInfo == default) 
                    ActiveAgents.Add(new AgentInfo(agents[i]));
            }
        }
        
        private void GetAgentsByAssembly(string assemblyName, ref List<Type> agents)
        {
            foreach(Type type in Assembly.Load(assemblyName).GetTypes())
            {
                if (type.GetCustomAttributes(typeof(AnalyticsAgentAttribute), false).Length > 0 &&
                    !type.Name.Equals(nameof(AnalyticsAgentAttribute)))
                    agents.Add(type);
            }
        }
    }
    
    [Serializable]
    public class AgentInfo
    {
        [ReadOnly] public string TypeName;
        [ReadOnly] public string AssemblyName;
     
        public bool CanTrackEvents = true;

        private Type _agentType;

        public AgentInfo(Type type)
        {
            _agentType = type;
            TypeName = _agentType.Name;
            AssemblyName = type.Assembly.GetName().Name;
        }

        public new Type GetType() => _agentType;
    }
}