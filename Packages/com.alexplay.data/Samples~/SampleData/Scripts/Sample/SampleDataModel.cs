using ACS.Data.DataService.Model;
using Newtonsoft.Json;

namespace Alexplay.Samples.Data.Scripts.Sample
{
    public sealed class SampleDataModel : ProgressModel
    {
        [JsonProperty] private bool sampleBoolean = false;

        private float _savingDelay = 0.76f;

        public SampleDataModel() => SetSavingDelay(_savingDelay);

        public bool GetData() => sampleBoolean;
        
        public void ChangeData()
        {
            sampleBoolean = !sampleBoolean;
            DemandSave();
        }
    }
}
