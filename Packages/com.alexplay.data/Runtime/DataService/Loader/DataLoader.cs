using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ACS.Data.DataService.Tool;
using Newtonsoft.Json;
using UnityEngine;

namespace ACS.Data.DataService.Loader
{
    public class DataLoader
    {
        private readonly BinaryFormatter _binaryFormatter;
        private readonly DataTool _dataTool;

        public DataLoader(DataTool tool)
        {
            _binaryFormatter = new BinaryFormatter();
            _dataTool = tool;
        }

        public TModel LoadProgressJson<TModel>(Type modelType) where TModel : new()
        {
            TModel model = default;
            string path = _dataTool.Path + modelType.Name + _dataTool.Extension;

            if (File.Exists(path))
            {
                FileStream stream = File.Open(path, FileMode.Open);
                string cryptData = (string) _binaryFormatter.Deserialize(stream);
                stream.Close();

                string data = _dataTool.Security.Decrypt(cryptData);

                try
                {
                    model = (TModel) JsonConvert.DeserializeObject(data, modelType);
                    Debug.Log($"File for [{modelType.Name}] found, data applied");
                }
                catch (Exception e)
                {
                    Debug.Log($"Corrupted data for [{modelType.Name}], set default data");
                    model = (TModel) Activator.CreateInstance(modelType);
                }
                
                return model;
            }
            
            Debug.Log($"File for [{modelType.Name}] not found, applied default data");
            
            return (TModel) Activator.CreateInstance(modelType);
        }
    }
}