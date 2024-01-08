using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ACS.Data.DataService.Config;
using ACS.Data.DataService.Container;
using ACS.Data.DataService.Loader;
using ACS.Data.DataService.Model;
using ACS.Data.DataService.Saver;
using ACS.Data.DataService.Tool;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace ACS.Data.DataService.Service
{
    [UsedImplicitly]
    public class DataService : IDataService, IDisposable
    {
        public event Action ModelsDataChanged;

        public IProgressModelContainer Models
        {
            get => _modelsContainer;
            set
            {
            }
        }

        private readonly DataTool _dataTools;
        private readonly DataLoader _dataLoader;
        private readonly DataCleaner _dataCleaner;
        private readonly BackgroundDataSaver _backgroundDataSaver;
        private ProgressModelsContainer _modelsContainer;
        
        public DataService(DataServiceConfig dataServiceConfig, IntentService.IntentService intentService)
        {
            _dataTools = new DataTool(dataServiceConfig, intentService);
            _dataLoader = new DataLoader(_dataTools);
            _dataCleaner = new DataCleaner(_dataTools);
            _backgroundDataSaver = new BackgroundDataSaver(_dataTools, dataServiceConfig);
        }

        public string GetSerializedData()
        {
            SerializedModelsData serializedModelsDatas = new SerializedModelsData();

            foreach (KeyValuePair<Type,ProgressModel> modelPair in _modelsContainer.Models)
            {
                string modelData = modelPair.Value.GetData();

                if(!modelData.Equals(""))
                    serializedModelsDatas.SerializedModels.Add(new SerializedModel()
                    {
                        ModelName = modelPair.Value.GetType().Name,
                        ModelData = modelData
                    });
            }

            string serializedData = JsonUtility.ToJson(serializedModelsDatas);
            return serializedData;
        }

        public void ApplySerializedData(string serializedData)
        {
            SerializedModelsData deserializedData = JsonUtility.FromJson<SerializedModelsData>(serializedData);
            List<SerializedModel> deserializedModels;

            if (deserializedData is { SerializedModels: { } })
                deserializedModels = deserializedData.SerializedModels;
            else 
                deserializedModels = new List<SerializedModel>();
            
            ClearLocalData(true);
            
            for (int i = 0; i < deserializedModels.Count; i++)
            {
                KeyValuePair<Type,ProgressModel> progressPair = _modelsContainer.Models
                    .FirstOrDefault(md => md.Value.GetType().Name.Equals(deserializedModels[i].ModelName));

                if (progressPair.Value != default)
                {
                    Type targetType = progressPair.Value.GetType();
                    ProgressModel deserializedObject = (ProgressModel) JsonConvert.DeserializeObject(deserializedModels[i].ModelData, targetType);

                    if (deserializedObject != null)
                    {
                        deserializedObject.PutData(deserializedModels[i].ModelData);

                        if (_modelsContainer.Models.ContainsKey(targetType))
                            _modelsContainer.Models[targetType] = deserializedObject;
                    }
                }
            }
            
            _backgroundDataSaver.DemandStorageSaving();
            
            ModelsDataChanged?.Invoke();
        }

        public void ClearLocalData(bool ignoreBroadcastingChangeEvent = false)
        {
            List<Type> actualModelTypes = new List<Type>();

            foreach (KeyValuePair<Type, ProgressModel> modelPair in _modelsContainer.Models)
            {
                _dataCleaner.DeleteModel<ProgressModel>(modelPair.Value.GetType());
                actualModelTypes.Add(modelPair.Key);
            }

            for (int i = 0; i < actualModelTypes.Count(); i++)
                _modelsContainer.Models[actualModelTypes[i]] =
                    (ProgressModel)Activator.CreateInstance(actualModelTypes[i]);

            if(!ignoreBroadcastingChangeEvent)
                ModelsDataChanged?.Invoke();
        }
        

        public void PrepareService()
        {
            TryCreateDirectory();
            LoadData();
            
            _backgroundDataSaver.Initialize(_modelsContainer);
        }

        private void LoadData()
        {
            List<Type> modelTypes = _dataTools.GetTypes<ProgressModelAttribute, ProgressModel>();
            Dictionary<Type, ProgressModel> models = new Dictionary<Type, ProgressModel>();

            for (int i = 0; i < modelTypes.Count; i++)
            {
                Type modelType = modelTypes[i];
                
                if(modelType.ContainsGenericParameters || modelType.IsAbstract)
                    continue;
                
                (ProgressModel, string) modelData = _dataLoader.LoadProgressJson<ProgressModel>(modelType);
                
                modelData.Item1.PutData(modelData.Item2);
                models.Add(modelType, modelData.Item1);
            }

            _modelsContainer = new ProgressModelsContainer(models);
        }

        private void TryCreateDirectory()
        {
            if(!Directory.Exists(_dataTools.ModelDirectoriesPath)) Directory.CreateDirectory(_dataTools.ModelDirectoriesPath);
            if(!Directory.Exists(_dataTools.ModelDebugDirectoriesPath)) Directory.CreateDirectory(_dataTools.ModelDebugDirectoriesPath);
        }

        public void Dispose()
        {
        }
    }

    [System.Serializable]
    public class SerializedModelsData
    {
        public List<SerializedModel> SerializedModels = new List<SerializedModel>();
    }

    [System.Serializable]
    public class SerializedModel
    {
        public string ModelName;
        public string ModelData;
    }
}
