using System;
using System.IO;
using ACS.Data.DataService.Model;
using ACS.Data.DataService.Tool;
using Newtonsoft.Json;
using UniRx;
using Time = UnityEngine.Time;

namespace ACS.Data.DataService.Saver
{
    public class DataSaver : IDisposable
    {
        private readonly DataTool _dataTool;

        private IDisposable _autoSaveDisposable;
        private ProgressModel _model;
        private readonly string _path;

        private string _serializedData;
        private string _normalData;
        private float _autoSaveTimer;
        private bool _savingBusy = false;

#if UNITY_EDITOR
        private string _debugData;
        private readonly string _debugPath;
#endif

        public DataSaver(ProgressModel model, DataTool tool)
        {
            _dataTool = tool;
            Type modelType = (_model = model).GetType();
            
            _autoSaveTimer = _dataTool.DataServiceConfig.AutoSaveDelay;

            _path = _dataTool.Path + modelType.Name + _dataTool.Extension;
#if UNITY_EDITOR
            _debugPath = _dataTool.DebugPath + modelType.Name + _dataTool.Extension;
#endif

#if UNITY_EDITOR
            _dataTool.IntentService.CoreDestroy += OnCoreDestroy;
#else
#if UNITY_ANDROID
            _dataTool.IntentService.OnPauseChanged += OnApplicationPause;
#else
            _dataTool.IntentService.OnFocusChanged += OnApplicationFocus;
#endif
#endif

            if (_dataTool.DataServiceConfig.EnableAutoSave)
                _autoSaveDisposable = Observable.EveryFixedUpdate().Subscribe(OnTick);
        }

        public void SaveDataInStorage()
        {
            if(_savingBusy) return;
            
            _savingBusy = true;
            _autoSaveTimer = _dataTool.DataServiceConfig.AutoSaveDelay;
            _serializedData = JsonConvert.SerializeObject(_model);
            _normalData = _dataTool.Security.Encrypt(_serializedData);

            _model.PutData(_serializedData);
            
#if UNITY_EDITOR
            _debugData = _dataTool.Security.Encrypt(_serializedData, ignoreCrypt: true);
#endif

            try
            {
                File.WriteAllText(_path, _normalData);
                
#if UNITY_EDITOR
                File.WriteAllText(_debugPath, _debugData);
#endif
                
                _savingBusy = false;
            }
            catch (Exception ex)
            {
                _savingBusy = false;
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }

#if UNITY_EDITOR
        private void OnCoreDestroy()
        {
            _dataTool.IntentService.CoreDestroy -= OnCoreDestroy;
            
            SaveDataInStorage();
        }
#else
       
#if UNITY_ANDROID
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) 
                SaveDataInStorage();
        }
#else
        private void OnApplicationFocus(bool focusStatus)
        {
            if (!focusStatus) 
                SaveDataInStorage();
        }
#endif
        
#endif
        
        private void OnTick(long tick)
        {
            if (_autoSaveTimer > 0)
            {
                _autoSaveTimer -= Time.fixedDeltaTime;
                return;
            }

            SaveDataInStorage();
        }
        
        public void Dispose()
        {
            _autoSaveDisposable?.Dispose();
        }
    }
}