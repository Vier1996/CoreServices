using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Core
{
    [System.Serializable]
    public abstract class CoreServiceConfig
    {
        public bool IsEnabled => _isEnabled;
        [HideInInspector, SerializeField] private bool _isEnabled;
        
        [ShowIf("@_isEnabled == false"), Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
        public void Enable() => 
            _isEnabled = true;
        
        [ShowIf("@_isEnabled == true"), Button(ButtonSizes.Medium), GUIColor(1, 0, 0)]
        public void Disable() => 
            _isEnabled = false;
    }
}