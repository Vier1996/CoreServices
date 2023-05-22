using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ACS.Data.DataService.Model;
using ACS.Data.DataService.Tool;
using Newtonsoft.Json;
using UniRx;

namespace ACS.Data.DataService.Saver
{
    public class DataSaver
    {
        private readonly DataTool _dataTool;

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
            _path = _dataTool.Path + _model.GetType().Name + _dataTool.Extension;
#if UNITY_EDITOR
            _debugPath = _dataTool.DebugPath + _model.GetType().Name + _dataTool.Extension;
#endif

            _dataTool.IntentService.OnPauseChanged += OnApplicationPause;
        }

        /*public void SaveProgressData()
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
        }*/

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) 
                SaveDataInStorage();
        }

        private void SaveDataInStorage()
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
    }
}