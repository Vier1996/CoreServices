using System;
using System.Collections.Generic;
using System.Linq;
using ACS.Analytics.Agent;
using UnityEditor.Compilation;
using Assembly = System.Reflection.Assembly;

// ReSharper disable RedundantExplicitArrayCreation

namespace ACS.Analytics
{
    public static class AnalyticsUtils
    {
        public static List<Type> GetAllAvailableAgents()
        {
            List<Type> agents = new List<Type>();
            IEnumerable<Assembly> asmList = CompilationPipeline.GetAssemblies().Select(a => Assembly.Load(a.name));

            foreach (Assembly asm in asmList) 
                AddAgentsByAssembly(asm, agents);

            return agents;
        }
        
        private static void AddAgentsByAssembly(Assembly assembly, ICollection<Type> to)
        {
            foreach(Type type in assembly.GetTypes())
                if (type.GetInterface(nameof(IAnalyticsAgent)) != null && type.IsClass)
                    to.Add(type);
        }
    }
}