using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ject
{
    public class vJect
    {
        private const string _sharpAssemblyName = "Assembly-CSharp";
        private readonly System.Reflection.Assembly _sharpAssembly;

        private List<Type> _services;
        private List<Type> _getters;
        
        public vJect()
        {
            _sharpAssembly = AppDomain.CurrentDomain.GetAssemblies().First(atr => atr.GetName().Name.Equals(_sharpAssemblyName));
           
            _services = GetTypesWithAttribute<DiService>(_sharpAssembly);
            _getters = GetTypesWithAttribute<DiGetter>(_sharpAssembly);

            foreach (Type getterType in _getters)
            {
                DiGetter getter = GetAttribute<DiGetter>(getterType);
                
                getter.Created += instance => Debug.Log("Created");
            }
            
            Debug.Log("1");
        }
        
        private List<Type> GetTypesWithAttribute<TAttribute>(System.Reflection.Assembly assembly)
        {
            List<Type> types = new List<Type>();
 
            foreach(Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TAttribute), true).Length > 0)
                    types.Add(type);
            }
 
            return types;
        }
        
        public static T GetAttribute<T>(Type type) where T : Attribute =>
                type
                .GetField(type.Name)
                .GetCustomAttributes(false)
                .OfType<T>()
                .SingleOrDefault();

        private List<Type> GetTypes(System.Reflection.Assembly assembly, Type type)
        {
            assembly.GetTypes().Where(type => type)
        }
    }
}