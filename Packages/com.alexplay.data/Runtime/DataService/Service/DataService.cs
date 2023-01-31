using System;
using System.Collections.Generic;
using System.IO;
using ACS.Data.DataService.Config;
using ACS.Data.DataService.Container;
using ACS.Data.DataService.Loader;
using ACS.Data.DataService.Model;
using ACS.Data.DataService.Saver;
using ACS.Data.DataService.Tool;
using JetBrains.Annotations;

namespace ACS.Data.DataService.Service
{
    [UsedImplicitly]
    public class DataService : IDataService, IDisposable
    {
        public IProgressModelContainer Models
        {
            get => _modelsContainer;
            set
            {
            }
        }

        private DataTool _dataTools;
        private DataSaver _dataSaver;
        private DataLoader _dataLoader;
        
        private ProgressModelsContainer _modelsContainer;
        
        public DataService(DataServiceConfig dataServiceConfig)
        {
            _dataTools = new DataTool(dataServiceConfig);
            _dataSaver = new DataSaver(_dataTools);
            _dataLoader = new DataLoader(_dataTools);
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
                ProgressModel model = _dataLoader.LoadProgressJson<ProgressModel>(modelType);
                
                model.Save += SaveDemanded;
                models.Add(model);
            }

            _modelsContainer = new ProgressModelsContainer(models);
        }

        private void SaveDemanded(ProgressModel model) => _dataSaver.SaveProgressData(model);

        private void TryCreateDirectory()
        {
            if(Directory.Exists(_dataTools.ModelDirectoriesPath))
                return;
            Directory.CreateDirectory(_dataTools.ModelDirectoriesPath);
        }

        public void Dispose()
        {
            foreach (var model in _modelsContainer.GetAll()) 
                model.Save -= SaveDemanded;
        }
    }
}
