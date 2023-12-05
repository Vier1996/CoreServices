using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Dialog.Dialogs.Config
{
    [Serializable]
    public struct DialogInfo : IEquatable<DialogInfo>
    {
        [ReadOnly] public string TypeFullName;
        [ReadOnly] public string AssemblyName;
     
        public bool PreBake;
        
        public DialogInfo(Type type)
        {
            TypeFullName = type.FullName;
            AssemblyName = type.Assembly.GetName().Name;
            
            PreBake = false;
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

        public bool Equals(DialogInfo other) => 
            TypeFullName == other.TypeFullName && AssemblyName == other.AssemblyName;

        public override bool Equals(object obj) => obj is DialogInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TypeFullName != null ? TypeFullName.GetHashCode() : 0) * 397) ^ (AssemblyName != null ? AssemblyName.GetHashCode() : 0);
            }
        }
    }
}