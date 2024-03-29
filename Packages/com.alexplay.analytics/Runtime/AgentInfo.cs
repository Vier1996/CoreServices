﻿using System;
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
            Assembly assembly = Assembly.Load(AssemblyName);
                if (assembly == null)
                    Debug.Log($"Assembly '{AssemblyName}' not found");
                Type agentType = assembly.GetType(TypeFullName);
                if (agentType == null)    
                    Debug.Log($"Type with full name '{TypeFullName}' doesn't exist within {AssemblyName}");
                return agentType;

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