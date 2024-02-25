using System;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif
using UnityEngine;

namespace Config
{
    [Serializable]
    public abstract class ServiceConfigBase
    {
        [HideInInspector] [SerializeField] private bool _isEnabled;
        
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }
        
        private bool _ignoreEnableButton = false;
        
        private bool _enableCondition => _isEnabled == false && _ignoreEnableButton == false;
        
        private bool _disableCondition => _isEnabled == true && _ignoreEnableButton == false;

        [Button(ButtonSizes.Medium)]
        [ShowIf(nameof(_enableCondition))]
        [GUIColor(0, 1, 0)]
        public void Enable() => _isEnabled = true;

        [PropertySpace(SpaceBefore = 20)]
        [ShowIf(nameof(_disableCondition))]
        [Button(ButtonSizes.Medium)]
        [GUIColor(1, 0, 0)]
        public void Disable() => _isEnabled = false;
        
        protected void UpdatePackage(string packageURL)
        {
#if UNITY_EDITOR
            Client.Add(packageURL);
#endif
        }
    }
}