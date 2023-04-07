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
        public event Action UpdateModelData;
        
        public IProgressModelContainer Models
        {
            get => _modelsContainer;
            set
            {
            }
        }

        private DataTool _dataTools;
        private DataLoader _dataLoader;
        private DataCleaner _dataCleaner;
        
        private ProgressModelsContainer _modelsContainer;
        
        public DataService(DataServiceConfig dataServiceConfig)
        {
            _dataTools = new DataTool(dataServiceConfig);
            _dataLoader = new DataLoader(_dataTools);
            _dataCleaner = new DataCleaner(_dataTools);
        }

        public string GetSerializedData()
        {
            SerializedModelsData serializedModelsDatas = new SerializedModelsData();
            List<ProgressModel> allModels = _modelsContainer.GetAll();
            
            for (int i = 0; i < allModels.Count; i++)
            {
                string modelData = allModels[i].GetData();

                if(!modelData.Equals(""))
                    serializedModelsDatas.SerializedModels.Add(new SerializedModel()
                    {
                        ModelName = allModels[i].GetType().Name,
                        ModelData = modelData
                    });
            }

            string serializedData = JsonUtility.ToJson(serializedModelsDatas);
            return serializedData;
        }

        public void ApplySerializedData(string serializedData)
        {
            bool hasChanges = false;
            SerializedModelsData deserializedData = JsonUtility.FromJson<SerializedModelsData>(serializedData);
            List<ProgressModel> allModels = _modelsContainer.GetAll();
            List<SerializedModel> deserializedModels = deserializedData.SerializedModels;
         
            ClearLocalData(allModels);
            
            for (int i = 0; i < deserializedModels.Count; i++)
            {
                ProgressModel progressModel = allModels.FirstOrDefault(md => md.GetType().Name.Equals(deserializedModels[i].ModelName));

                if (progressModel != default)
                {
                    var deserializeObject = (ProgressModel) JsonConvert.DeserializeObject(deserializedModels[i].ModelData, progressModel.GetType());

                    if (deserializeObject != null)
                    {
                        progressModel = deserializeObject;
                        progressModel.SetupModel(_dataTools);
                        progressModel.PutData(deserializedModels[i].ModelData);
                        progressModel.DemandSaveImmediate();
                        hasChanges = true;
                    }
                }
            }
            
            if(hasChanges)
                UpdateModelData?.Invoke();
        }

        public void ClearLocalData(List<ProgressModel> localModels)
        {
            for (int i = 0; i < localModels.Count; i++)
                _dataCleaner.DeleteModel<ProgressModel>(localModels[i].GetType());
        }
        

        public void PrepareService()
        {
            TryCreateDirectory();
            LoadData();
        }

        private void LoadData()
        {
            List<Type> modelTypes = _dataTools.GetTypes<ProgressModelAttribute, ProgressModel>();
            List<ProgressModel> models = new List<ProgressModel>();

            for (int i = 0; i < modelTypes.Count; i++)
            {
                Type modelType = modelTypes[i];
                
                if(modelType.ContainsGenericParameters || modelType.IsAbstract)
                    continue;
                
                (ProgressModel, string) modelData = _dataLoader.LoadProgressJson<ProgressModel>(modelType);
                
                modelData.Item1.SetupModel(_dataTools);
                modelData.Item1.PutData(modelData.Item2);
                models.Add(modelData.Item1);
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
