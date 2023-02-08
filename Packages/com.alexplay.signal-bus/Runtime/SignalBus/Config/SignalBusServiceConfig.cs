using System;
using Config;
using Sirenix.OdinInspector;
using UnityEditor;

namespace ACS.SignalBus.SignalBus.Config
{
    [Serializable]
    public class SignalBusServiceConfig : ServiceConfigBase
    {

        [Button] private void UpdatePackage()
        {
#if UNITY_EDITOR
            UnityEditor.PackageManager.Client.Resolve();
#endif
        }
    }
}
