using System;
using ACS.Data.DataService.Model;
using Newtonsoft.Json;

namespace Alexplay.Samples.Data.Scripts.Sample
{
    public sealed class SampleDataModelSecond : ProgressModel
    {
        [JsonProperty] private long sampleLong = 0;
        
        public long GetData() => sampleLong;

        public void ChangeData()
        {
            sampleLong = DateTime.UtcNow.Ticks;
            DemandSave();
        }
    }
}