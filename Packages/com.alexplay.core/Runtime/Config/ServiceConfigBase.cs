using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Config
{
    [Serializable]
    public abstract class ServiceConfigBase
    {
        public event Action Enabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }
        [HideInInspector] [SerializeField] private bool _isEnabled;

        [Button(ButtonSizes.Medium)]
        [ShowIf("@_isEnabled == false")]
        [GUIColor(0, 1, 0)]
        public void Enable()
        {
            _isEnabled = true;
            
            Enabled?.Invoke();
        }

        [ShowIf("@_isEnabled == true")]
        [Button(ButtonSizes.Medium)]
        [GUIColor(1, 0, 0)]
        public void Disable() => 
            _isEnabled = false;
    }
}