using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ACS.Data.DataService.Model;
using ACS.Data.DataService.Tool;
using Newtonsoft.Json;

namespace ACS.Data.DataService.Saver
{
    public class DataSaver
    {
        private readonly BinaryFormatter _binaryFormatter;
        private readonly DataTool _dataTool;

        private ProgressModel _model;
        private string _path;

#if UNITY_EDITOR
        private string _debugPath;
#endif

        public DataSaver(ProgressModel model, DataTool tool)
        {
            _dataTool = tool;
            _binaryFormatter = new BinaryFormatter();

            _model = model;
            _path = _dataTool.Path + _model.GetType().Name + _dataTool.Extension;
#if UNITY_EDITOR
            _debugPath = _dataTool.DebugPath + _model.GetType().Name + _dataTool.Extension;
#endif
        }

        public void SaveProgressData()
        {
            string serializedData = JsonConvert.SerializeObject(_model);
            string data = _dataTool.Security.Encrypt(serializedData);
            
            _model.PutData(serializedData);
            
#if UNITY_EDITOR
            string debugData = _dataTool.Security.Encrypt(serializedData, ignoreCrypt: true);
#endif
            
            try
            {
                File.WriteAllText(_path, data);
                
#if UNITY_EDITOR
                File.WriteAllText(_debugPath, debugData);
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving data: " + ex.Message);
            }
        }
    }
}