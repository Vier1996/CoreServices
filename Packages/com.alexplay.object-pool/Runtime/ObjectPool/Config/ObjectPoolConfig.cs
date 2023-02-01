using System;
using Config;
using Sirenix.OdinInspector;

namespace ACS.ObjectPool.ObjectPool.Config
{
    [Serializable]
    public class ObjectPoolConfig : ServiceConfigBase
    {
        [ShowIf("@IsEnabled == true")]
        public bool AddressablePool = false;
        [ShowIf("@IsEnabled == true")]
        public bool StandardPool = false;
    }
}
