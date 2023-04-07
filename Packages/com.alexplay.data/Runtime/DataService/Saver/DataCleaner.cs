using System;
using System.IO;
using ACS.Data.DataService.Tool;
using Newtonsoft.Json;

namespace ACS.Data.DataService.Saver
{
    public class DataCleaner
    {
        private readonly DataTool _dataTool;

        public DataCleaner(DataTool tool) => _dataTool = tool;

        public void DeleteModel<TModel>(Type modelType) where TModel : new()
        {
            TModel model = default;
            string path = _dataTool.Path + modelType.Name + _dataTool.Extension;

            if (!File.Exists(path)) return;
            
            File.Delete(path);
        }
    }
}