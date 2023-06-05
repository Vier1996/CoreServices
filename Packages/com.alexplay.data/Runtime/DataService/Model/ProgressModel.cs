using System;

namespace ACS.Data.DataService.Model
{
    [ProgressModel]
    [UnityEngine.Scripting.Preserve]
    public class ProgressModel
    {
        public bool IsDirty { get; private set; }
        
        private string _serializedData = "";

        public void PutData(string serializedData)
        {
            _serializedData = serializedData;
            IsDirty = false;
        }

        public string GetData() => _serializedData;

        protected void MarkAsDirty() => IsDirty = true;
    }
    
    public class ProgressModelAttribute : Attribute { }
}