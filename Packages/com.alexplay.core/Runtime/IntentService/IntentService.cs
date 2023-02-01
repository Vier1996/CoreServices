using System;
using UnityEngine;

namespace IntentService
{
    public class IntentService : MonoBehaviour
    {
        public event Action<bool> OnFocusChanged;
        public event Action<bool> OnPauseChanged;
        
        public void PrepareService() { }

        private void OnApplicationFocus(bool hasFocus) => OnFocusChanged?.Invoke(hasFocus);

        private void OnApplicationPause(bool pauseStatus) => OnPauseChanged?.Invoke(pauseStatus);
    }
}