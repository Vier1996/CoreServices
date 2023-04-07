﻿using System;
using ACS.Data.DataService.Saver;
using ACS.Data.DataService.Tool;

namespace ACS.Data.DataService.Model
{
    [ProgressModel]
    public class ProgressModel
    {
        private DataSaver _saver;

        private string _serializedData = "";
        private long _lastSaveTicks = 0;
        private long _savingDelay = TimeSpan.FromSeconds(2).Ticks;
        
        public void SetupModel(DataTool tool) => _saver = new DataSaver(this, tool);
        public void PutData(string serializedData) => _serializedData = serializedData;
        public string GetData() => _serializedData;
        public void DemandSaveImmediate() => SaveData();

        protected void SetSavingDelay(float savingDelay = 2f) => _savingDelay = TimeSpan.FromSeconds(savingDelay).Ticks;

        protected void DemandSave()
        {
            if (SaveAllowed()) 
                SaveData();
        }
        
        private void SaveData()
        {
            _lastSaveTicks = DateTime.UtcNow.Ticks;
            _saver.SaveProgressData();
        }

        private bool SaveAllowed() => DateTime.UtcNow.Ticks - _lastSaveTicks >= _savingDelay;
    }
    
    public class ProgressModelAttribute : Attribute { }
}