using System;
using ACS.Data.DataService.Saver;
using ACS.Data.DataService.Tool;

namespace ACS.Data.DataService.Model
{
    [ProgressModel]
    [UnityEngine.Scripting.Preserve]
    public class ProgressModel
    {
        public bool IsDirty { get; private set; }
        
        private DataSaver _saver;
        private string _serializedData = "";

        public void SetupModel(DataTool tool) => _saver = new DataSaver(this, tool);
        public void PutData(string serializedData)
        {
            _serializedData = serializedData;
            IsDirty = false;
        }

        public string GetData() => _serializedData;

        protected void MarkAsDirty() => IsDirty = true;
        private void DemandStorageSave() => _saver.SaveDataInStorage(); // This method gets by reflection
    }
    
    public class ProgressModelAttribute : Attribute { }
}