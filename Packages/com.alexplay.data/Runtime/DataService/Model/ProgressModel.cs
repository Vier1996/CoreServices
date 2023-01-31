using System;

namespace ACS.Data.DataService.Model
{
    [ProgressModel]
    public class ProgressModel
    {
        public event Action<ProgressModel> Save;
        
        private long _lastSaveTicks = 0;
        private long _savingDelay = TimeSpan.FromSeconds(2).Ticks;

        protected void SetSavingDelay(float savingDelay = 2f) => 
            _savingDelay = TimeSpan.FromSeconds(savingDelay).Ticks;

        protected void DemandSave()
        {
            if (SaveAllowed())
            {
                _lastSaveTicks = DateTime.UtcNow.Ticks;
                Save?.Invoke(this);
            }
        }

        private bool SaveAllowed() => DateTime.UtcNow.Ticks - _lastSaveTicks >= _savingDelay;
    }
    
    public class ProgressModelAttribute : Attribute { }
}