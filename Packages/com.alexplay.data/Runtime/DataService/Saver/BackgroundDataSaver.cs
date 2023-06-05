using System;
using System.IO;
using System.Threading;
using ACS.Data.DataService.Config;
using ACS.Data.DataService.Container;
using ACS.Data.DataService.Model;
using ACS.Data.DataService.Tool;
using Newtonsoft.Json;

namespace ACS.Data.DataService.Saver
{
    public class BackgroundDataSaver : IDisposable
    {
        private readonly DataTool _dataTool;
        private DataServiceConfig _dataServiceConfig;
        private ProgressModelsContainer _modelsContainer; 
        private Thread _cycleSavingThread;
        
        private readonly object _lockObject = new object();
        
        private string _path;
        private string _serializedData;
        private string _normalData;
#if UNITY_EDITOR
        private string _debugData;
        private string _debugPath;
#endif
        private int _cycleSaveTime;
        private bool _savingBusy = false;
        private bool _isCanceled = false;

        public BackgroundDataSaver(DataTool tool, DataServiceConfig dataServiceConfig)
        {
            _dataTool = tool;
            _dataServiceConfig = dataServiceConfig;
            
#if UNITY_EDITOR
            _dataTool.IntentService.CoreDestroy += OnCoreDestroy;
#else
#if UNITY_ANDROID
            _dataTool.IntentService.OnPauseChanged += OnApplicationPause;
#else
            _dataTool.IntentService.OnFocusChanged += OnApplicationFocus;
#endif
#endif
        }

        public void Initialize(ProgressModelsContainer modelsContainer)
        {
            _modelsContainer = modelsContainer;

            if (_dataTool.DataServiceConfig.EnableAutoSave && _dataTool.DataServiceConfig.AutoSaveDelay > 0)
            {
                _cycleSaveTime = _dataTool.DataServiceConfig.AutoSaveDelay * 1000;

                _cycleSavingThread = new Thread(ExecuteStorageSaving)
                {
                    Name = "SavingDataThread",
                    IsBackground = true
                };

                _cycleSavingThread.Start();
            }
        }

        public void ForceSaveInStorage() => ExecuteStorageSaving();

#if UNITY_EDITOR
        private void OnCoreDestroy()
        {
            _dataTool.IntentService.CoreDestroy -= OnCoreDestroy;

            _isCanceled = true;
            
            foreach (var modelPair in _modelsContainer.Models) 
                SaveModelInStorage(modelPair.Value);
        }
#else
       
#if UNITY_ANDROID
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) 
                ForceSaveInStorage();
        }
#else
        private void OnApplicationFocus(bool focusStatus)
        {
            if (!focusStatus) 
                ForceSaveInStorage();
        }
#endif
        
#endif
        
        private void ExecuteStorageSaving()
        {
            while (!_isCanceled)
            {
                lock (_lockObject)
                {
                    _savingBusy = true;

                    foreach (var modelPair in _modelsContainer.Models) 
                        SaveModelInStorage(modelPair.Value);
                }

                if (!_isCanceled)
                {
                    _savingBusy = false;
                    Thread.Sleep(_cycleSaveTime);
                }
            }
        }

        private void SaveModelInStorage(ProgressModel model)
        {
            if(_savingBusy || !model.IsDirty) return;

            Type modelType = model.GetType();
            
            _path = _dataTool.Path + modelType.Name + _dataTool.Extension;
#if UNITY_EDITOR
            _debugPath = _dataTool.DebugPath + modelType.Name + _dataTool.Extension;
#endif
            _serializedData = JsonConvert.SerializeObject(model);
            _normalData = _dataTool.Security.Encrypt(_serializedData);

            model.PutData(_serializedData);
            
#if UNITY_EDITOR
            _debugData = _dataTool.Security.Encrypt(_serializedData, ignoreCrypt: true);
#endif

            try
            {
                File.WriteAllText(_path, _normalData);
                
#if UNITY_EDITOR
                File.WriteAllText(_debugPath, _debugData);
#endif
            }
            catch (Exception ex)
            {
                _savingBusy = false;
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }

        public void Dispose() { }
    }
}