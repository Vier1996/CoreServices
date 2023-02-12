using System;
using ACS.Data.DataService.Model;
using Newtonsoft.Json;

namespace Alexplay.Samples.Data.Scripts.Sample
{
    public sealed class SampleDataModelThird : ProgressModel
    {
        [JsonProperty] private string sampleString = "";
        
        public string GetData() => sampleString;

        public void ChangeData()
        {
            sampleString = DateTime.UtcNow.Ticks.ToString();
            DemandSave();
        }
    }
}