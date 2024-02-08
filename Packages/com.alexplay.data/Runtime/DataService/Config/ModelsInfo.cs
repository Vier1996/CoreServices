using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Data.DataService.Config
{
    [Serializable]
    public struct ModelsInfo : IEquatable<ModelsInfo>
    {
        [ReadOnly] public string Name;
        [ReadOnly] public string TypeFullName;
        [ReadOnly] public string AssemblyName;
     
        public bool LockErrasing;
        
        public ModelsInfo(Type type)
        {
            Name = type.Name;
            TypeFullName = type.FullName;
            AssemblyName = type.Assembly.GetName().Name;
            
            LockErrasing = false;
        }

        public Type GetModelType()
        {
            Assembly assembly = Assembly.Load(AssemblyName);
            
            if (assembly == null)
                Debug.Log($"Assembly '{AssemblyName}' not found");
            
            Type agentType = assembly.GetType(TypeFullName);
            
            if (agentType == null)    
                Debug.Log($"Type with full name '{TypeFullName}' doesn't exist within {AssemblyName}");
            
            return agentType;

        }

        public bool Equals(ModelsInfo other) => 
            TypeFullName == other.TypeFullName && AssemblyName == other.AssemblyName;

        public override bool Equals(object obj) => obj is ModelsInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TypeFullName != null ? TypeFullName.GetHashCode() : 0) * 397) ^ (AssemblyName != null ? AssemblyName.GetHashCode() : 0);
            }
        }
    }
}