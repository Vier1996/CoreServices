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

        private IDisposable _saveDisposable;
        private ProgressModel _model;
        private readonly string _path;

        private string _serializedData;
        private string _normalData;

        private float _updateDataTime = 2f;
        private float _updateDataTimer = 1f;

#if UNITY_EDITOR
        private string _debugData;
        private readonly string _debugPath;
#endif

        public DataSaver(ProgressModel model, DataTool tool)
        {
            _dataTool = tool;
            _model = model;
            _path = _dataTool.Path + _model.GetType().Name + _dataTool.Extension;
#if UNITY_EDITOR
            _debugPath = _dataTool.DebugPath + _model.GetType().Name + _dataTool.Extension;
#endif

#if UNITY_EDITOR
            _dataTool.IntentService.CoreDestroy += OnCoreDestroy;
#else
            _dataTool.IntentService.OnPauseChanged += OnApplicationPause;
#endif
            //LaunchSaveTimer();
        }

        private void LaunchSaveTimer()
        {
            _saveDisposable = Observable.EveryFixedUpdate().Subscribe(OnNext);
        }

        private void OnNext(long obj)
        {
            if (_updateDataTimer > 0)
            {
                _updateDataTimer -= Time.fixedDeltaTime;
                return;
            }

            SaveDataInStorage();
        }

        public void SaveDataInStorage()
        {
            _updateDataTimer = _updateDataTime;
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
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) 
                SaveDataInStorage();
        }
#endif
        public void Dispose()
        {
            _saveDisposable?.Dispose();
        }
    }
}