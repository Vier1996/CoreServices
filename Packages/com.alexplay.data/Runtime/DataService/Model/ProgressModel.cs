using System;
using ACS.Data.DataService.Saver;
using ACS.Data.DataService.Tool;

namespace ACS.Data.DataService.Model
{
    [ProgressModel]
    public class ProgressModel
    {
        private DataSaver _saver;

        private string _serializedData = "";

        public void SetupModel(DataTool tool) => _saver = new DataSaver(this, tool);
        public void PutData(string serializedData) => _serializedData = serializedData;
        public string GetData() => _serializedData;
        
        [Obsolete("This API has been deprecated as of ACS Data 1.1.0. All data save immediate by default.", false)]
        public void DemandSaveImmediate()
        {
        }

        [Obsolete("This API has been deprecated as of ACS Data 1.1.0. All data save immediate by default.", false)]
        protected void SetSavingDelay(float savingDelay = 2f)
        {
        }
        
        [Obsolete("This API has been deprecated as of ACS Data 1.1.0. All data save immediate by default.", false)]
        protected void DemandSave()
        {
        }
    }
    
    public class ProgressModelAttribute : Attribute { }
}