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

        private Type _modelType;
        private IDisposable _saveDisposable;
        private ProgressModel _model;
        private readonly string _path;

        private string _serializedData;
        private string _normalData;

#if UNITY_EDITOR
        private string _debugData;
        private readonly string _debugPath;
#endif

        public DataSaver(ProgressModel model, DataTool tool)
        {
            _dataTool = tool;
            _model = model;
            _modelType = model.GetType();
            
            _path = _dataTool.Path + _modelType.Name + _dataTool.Extension;
#if UNITY_EDITOR
            _debugPath = _dataTool.DebugPath + _modelType.Name + _dataTool.Extension;
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
        }

        public void SaveDataInStorage()
        {
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
            }
            catch (Exception ex)
            {
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
        public void Dispose()
        {
            _saveDisposable?.Dispose();
        }
    }
}