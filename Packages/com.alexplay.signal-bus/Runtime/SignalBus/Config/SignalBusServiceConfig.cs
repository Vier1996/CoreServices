using System;
using Config;

namespace ACS.SignalBus.SignalBus.Config
{
    [Serializable]
    public class SignalBusServiceConfig : ServiceConfigBase
    {
        protected override string PackageName => "com.alexplay.signal_bus";
    }
}
