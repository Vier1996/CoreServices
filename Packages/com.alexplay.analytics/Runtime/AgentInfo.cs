using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace ACS.Analytics
{
    [Serializable]
    public struct AgentInfo : IEquatable<AgentInfo>
    {
        [ReadOnly] public string TypeFullName;
        [ReadOnly] public string AssemblyName;
     
        public bool CanTrackEvents;
        
        public AgentInfo(Type type)
        {
            TypeFullName = type.FullName;
            AssemblyName = type.Assembly.GetName().Name;
            CanTrackEvents = true;
        }

        public Type GetAgentType()
        {
            try
            {
                Assembly assembly = Assembly.Load(AssemblyName);
                Type agentType = assembly.GetType(TypeFullName);
                return agentType;
            }
            catch (Exception)
            {
                Debug.Log($"Type with full name '{TypeFullName}' doesn't exist");
                throw;
            }
        }

        public bool Equals(AgentInfo other) => 
            TypeFullName == other.TypeFullName && AssemblyName == other.AssemblyName;

        public override bool Equals(object obj) => 
            obj is AgentInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TypeFullName != null ? TypeFullName.GetHashCode() : 0) * 397) ^ (AssemblyName != null ? AssemblyName.GetHashCode() : 0);
            }
        }
    }
}