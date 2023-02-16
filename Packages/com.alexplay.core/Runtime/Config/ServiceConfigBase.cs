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
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }
        [HideInInspector] [SerializeField] private bool _isEnabled;

        [Button(ButtonSizes.Medium)]
        [ShowIf("@_isEnabled == false")]
        [GUIColor(0, 1, 0)]
        public void Enable() => _isEnabled = true;

        [PropertySpace(SpaceBefore = 20)]
        [ShowIf("@_isEnabled == true")]
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