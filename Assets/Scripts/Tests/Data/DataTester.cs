using System;
using System.Collections.Generic;
using ACS.Core;
using ACS.Data.DataService.Model;
using ACS.Data.DataService.Service;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Tests.Data
{
    public class DataTester : MonoBehaviour
    {
        private TestModel model;
        
        [Inject] private void InjectDependencies(IDataService dataService)
        {
            model = dataService.Models.Resolve<TestModel>();
        }

        [Button] private void AddRandomData()
        {
            model.AddStringElement("DASD");
            model.AddIntElement(Random.Range(1, 1000));
            model.AddDoubleElement(Random.Range(1, 1000000));
            model.AddFloatElement(Random.Range(1f, 1000f));
        }
    }

    public class TestModel : ProgressModel
    {
        [JsonProperty] private List<string> _strings = new List<string>();
        [JsonProperty] private List<int> _ints = new List<int>();
        [JsonProperty] private List<double> _doubles = new List<double>();
        [JsonProperty] private List<float> _floats = new List<float>();
        
        public void AddStringElement(string data) => _strings.Add(data);
        public void AddIntElement(int data) => _ints.Add(data);
        public void AddDoubleElement(double data) => _doubles.Add(data);
        public void AddFloatElement(float data) => _floats.Add(data);
    }
}
