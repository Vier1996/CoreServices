using System;
using System.Collections.Generic;
using System.Reflection;
using ACS.Analytics.Agent;
// ReSharper disable RedundantExplicitArrayCreation

namespace ACS.Analytics
{
    public static class AnalyticsUtils
    {
        private const string AnalyticsAssemblyName = "ACS.Analytics";
        private const string CSharpAssemblyName = "Assembly-CSharp";
        
        public static List<Type> GetAllAvailableAgents()
        {
            List<Type> agents = new List<Type>();

            var mainAsm = Assembly.GetEntryAssembly();
            AssemblyName[] asmList;
            if (mainAsm is { })
                asmList = mainAsm.GetReferencedAssemblies();
            else
                asmList = new AssemblyName[]
                {
                    Assembly.Load(AnalyticsAssemblyName).GetName(),
                    Assembly.Load(CSharpAssemblyName).GetName(),
                };

            foreach (AssemblyName name in asmList) 
                AddAgentsByAssembly(name, agents);

            return agents;
        }
        
        private static void AddAgentsByAssembly(AssemblyName assemblyName, ICollection<Type> to)
        {
            foreach(Type type in Assembly.Load(assemblyName).GetTypes())
                if (type.GetInterface(nameof(IAnalyticsAgent)) != null && type.IsClass)
                    to.Add(type);
        }
    }
}