using System;
using System.Collections.Generic;
using System.Linq;
using ACS.Data.DataService.Config;
using ACS.Data.DataService.Security;
using UnityEngine;

namespace ACS.Data.DataService.Tool
{
    public class DataTool
    {
        public readonly DataSecurity Security;
        public readonly IntentService.IntentService IntentService;
        public readonly DataServiceConfig DataServiceConfig;
        public string Path { get; }
        public string DebugPath { get; }
        public string ModelDirectoriesPath { get; }
        public string ModelDebugDirectoriesPath { get; }
        public string Extension => _extension;

        private System.Reflection.Assembly _sharpAssembly;
        
        private const string _sharpAssemblyName = "Assembly-CSharp";
        private const string _modelsPath = "/Data/Models/";
        private const string _modelDebugPath = "/Debug/Models/";
        private const string _modelDirectoriesPath = "/Data/Models";
        private const string _modelDebugDirectoriesPath = "/Debug/Models";
        private const string _extension = ".apd";

        public DataTool(DataServiceConfig dataServiceConfig, IntentService.IntentService intentService)
        {
            IntentService = intentService;
            DataServiceConfig = dataServiceConfig;
            
            Path = Application.persistentDataPath + _modelsPath;
            DebugPath = Application.persistentDataPath + _modelDebugPath;
            ModelDirectoriesPath = Application.persistentDataPath + _modelDirectoriesPath;
            ModelDebugDirectoriesPath = Application.persistentDataPath + _modelDebugDirectoriesPath;
            
            _sharpAssembly = AppDomain.CurrentDomain.GetAssemblies().First(atr => atr.GetName().Name.Equals(_sharpAssemblyName));
            Security = new DataSecurity();
        }
        
        public List<Type> GetTypes<TAttribute, TType>() => GetTypesWithAttribute<TAttribute, TType>(_sharpAssembly);

        private List<Type> GetTypesWithAttribute<TAttribute, TType>(System.Reflection.Assembly assembly)
        {
            List<Type> types = new List<Type>();
 
            foreach(Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TAttribute), true).Length > 0 &&
                    !type.Name.Equals(typeof(TType).Name))
                    types.Add(type);
            }
 
            return types;
        }
    }
}