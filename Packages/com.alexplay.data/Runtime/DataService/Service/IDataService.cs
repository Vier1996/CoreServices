﻿using System;
using ACS.Data.DataService.Container;

namespace ACS.Data.DataService.Service
{
    public interface IDataService
    {
        public event Action UpdateModelData;
        public IProgressModelContainer Models { get; set; }
        public string GetSerializedData();
        public void ApplySerializedData(string serializedData);
    }
}