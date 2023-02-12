using System;
using System.Collections.Generic;
using ACS.Core;
using ACS.Data.DataService.Model;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tests.Data
{
    public class DataTester : MonoBehaviour
    {
        private TestModel model;
        
        private void Awake() => model = Core.Instance.DataService.Models.Resolve<TestModel>();

        [Button] private void AddRandomData()
        {
            model.AddStringElement("DASD");
            model.AddIntElement(Random.Range(1, 1000));
            model.AddDoubleElement(Random.Range(1, 1000000));
            model.AddFloatElement(Random.Range(1f, 1000f));
        }

        [Button] private void Save()
        {
            model.SaveToDB(); 
        }
    }

    public class TestModel : ProgressModel
    {
        [JsonProperty] private List<string> _strings = new List<string>();
        [JsonProperty] private List<int> _ints = new List<int>();
        [JsonProperty] private List<double> _doubles = new List<double>();
        [JsonProperty] private List<float> _floats = new List<float>();

        public TestModel() => SetSavingDelay(0);

        public void AddStringElement(string data) => _strings.Add(data);
        public void AddIntElement(int data) => _ints.Add(data);
        public void AddDoubleElement(double data) => _doubles.Add(data);
        public void AddFloatElement(float data) => _floats.Add(data);
        
        public void SaveToDB() => DemandSave();
    }
}
