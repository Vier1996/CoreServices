using System;
using System.IO;
using ACS.Data.DataService.Config;
using ACS.Data.DataService.Container;
using ACS.Data.DataService.Model;
using ACS.Data.DataService.Tool;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace ACS.Data.DataService.Saver
{
    public class BackgroundDataSaver : IDisposable
    {
        private readonly DataTool _dataTool;
        private IDisposable _fixedUpdateDisposable;
        private DataServiceConfig _dataServiceConfig;
        private ProgressModelsContainer _modelsContainer; 
        
        private string _path;
        private string _serializedData;
        private string _normalData;
#if UNITY_EDITOR
        private string _debugData;
        private string _debugPath;
#endif
        private float _cycleSaveTime;
        private bool _savingBusy = false;

        public BackgroundDataSaver(DataTool tool, DataServiceConfig dataServiceConfig)
        {
            _dataTool = tool;
            _dataServiceConfig = dataServiceConfig;
            
#if UNITY_EDITOR
            _dataTool.IntentService.CoreDestroy += OnCoreDestroy;
#else
            _dataTool.IntentService.OnFocusChanged += OnApplicationFocus;
#endif
        }

        public void Initialize(ProgressModelsContainer modelsContainer)
        {
            _modelsContainer = modelsContainer;

            if (_dataTool.DataServiceConfig.EnableAutoSave && _dataTool.DataServiceConfig.AutoSaveDelay > 0)
            {
                BreakTime();
                
                _fixedUpdateDisposable = Observable.EveryUpdate().Subscribe(OnTick);
            }
        }

        public void DemandStorageSaving() => ExecuteStorageSaving();

        private void OnTick(Unit state)
        {
            if (_cycleSaveTime > 0)
            {
                _cycleSaveTime -= Time.deltaTime;
                return;
            }

            BreakTime();
            ExecuteStorageSaving();
        }
        
#if UNITY_EDITOR
        private void OnCoreDestroy()
        {
            _dataTool.IntentService.CoreDestroy -= OnCoreDestroy;
            
            foreach (var modelPair in _modelsContainer.Models) 
                SaveModelInStorage(modelPair.Value);
        }
#else
        private void OnApplicationFocus(bool focusStatus)
        {
            if (!focusStatus) 
                ExecuteStorageSaving();
        }
#endif

        private void ExecuteStorageSaving()
        {
            _savingBusy = true;

            foreach (var modelPair in _modelsContainer.Models)
                SaveModelInStorage(modelPair.Value);

            _savingBusy = false;
        }

        private void SaveModelInStorage(ProgressModel model)
        {
            if(!model.IsDirty) return;

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

        private void BreakTime() => _cycleSaveTime = _dataTool.DataServiceConfig.AutoSaveDelay;

        public void Dispose()
        {
            _fixedUpdateDisposable?.Dispose();
            
#if !UNITY_EDITOR
            _dataTool.IntentService.OnFocusChanged -= OnApplicationFocus;
#endif
        }
    }
}