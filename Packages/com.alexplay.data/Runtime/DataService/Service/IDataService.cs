using System;
using ACS.Data.DataService.Container;

namespace ACS.Data.DataService.Service
{
    public interface IDataService
    {
        public event Action ModelsDataChanged;
        public IProgressModelContainer Models { get; set; }
        public string GetSerializedData(bool forceSerialization = false);
        public void ClearLocalData(bool ignoreBroadcastingChangeEvent = false);
        public void ApplySerializedData(string serializedData);
    }
}