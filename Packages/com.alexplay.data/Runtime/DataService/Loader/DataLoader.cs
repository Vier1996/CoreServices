using System;
using System.IO;
using ACS.Data.DataService.Tool;
using Newtonsoft.Json;

namespace ACS.Data.DataService.Loader
{
    public class DataLoader
    {
        private readonly DataTool _dataTool;

        public DataLoader(DataTool tool) => _dataTool = tool;

        public (TModel, string) LoadProgressJson<TModel>(Type modelType) where TModel : new()
        {
            TModel model = default;
            string path = _dataTool.Path + modelType.Name + _dataTool.Extension;

            if (File.Exists(path))
            {
                string cryptData = File.ReadAllText(path);
                string data = _dataTool.Security.Decrypt(cryptData);

                try
                {
                    model = (TModel) JsonConvert.DeserializeObject(data, modelType);
                }
                catch (Exception e)
                {
                    data = "";
                    model = (TModel) Activator.CreateInstance(modelType);
                }
                
                return (model, data);
            }
            
            return ((TModel) Activator.CreateInstance(modelType), "");
        }
    }
}