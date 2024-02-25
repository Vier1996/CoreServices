using System;
using System.Reflection;
using ACS.Dialog.Dialogs.View;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Dialog.Dialogs.Config
{
    [Serializable]
    public struct DialogInfo : IEquatable<DialogInfo>
    {
        [ReadOnly] public string Name;
        [ReadOnly] public string TypeFullName;
        [ReadOnly] public string AssemblyName;
        
        public DialogReference AddressableReference;
        
        public DialogInfo(Type type)
        {
            Name = type.Name;
            TypeFullName = type.FullName;
            AssemblyName = type.Assembly.GetName().Name;
            AddressableReference = null;
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