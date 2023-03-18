using System;
using System.Collections.Generic;
using System.Reflection;

namespace ACS.Analytics
{
    public static class AnalyticsUtils
    {
        public static List<Type> GetAllAvailableAgents()
        {
            List<Type> agents = new List<Type>();
            
            agents.AddRange(GetAgentsByAssembly(AnalyticsConstants.AnalyticsAssemblyName));
            agents.AddRange(GetAgentsByAssembly(AnalyticsConstants.SharpAssemblyName));

            return agents;
        }
        
        private static List<Type> GetAgentsByAssembly(string assemblyName)
        {
            List<Type> agents = new List<Type>();
            
            foreach(Type type in Assembly.Load(assemblyName).GetTypes())
            {
                if (type.GetCustomAttributes(typeof(AnalyticsAgentAttribute), false).Length > 0 &&
                    !type.Name.Equals(nameof(AnalyticsAgentAttribute)))
                    agents.Add(type);
            }

            return agents;
        }
    }
}