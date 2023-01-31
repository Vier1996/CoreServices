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
        
        public DataSaver(DataTool tool)
        {
            _dataTool = tool;
            _binaryFormatter = new BinaryFormatter();
        }

        public void SaveProgressData(ProgressModel model)
        {
            string path = _dataTool.Path + model.GetType().Name + _dataTool.Extension;
            string serializedData = JsonConvert.SerializeObject(model);
            string data = _dataTool.Security.Encrypt(serializedData);

            FileStream stream = File.OpenWrite(path);
            _binaryFormatter.Serialize(stream, data);
            stream.Close();
        }
    }
}