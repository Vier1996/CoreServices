using System;
using UnityEngine;

#if COM_ALEXPLAY_NET_ADS
using ACS.Ads;
#endif
#if COM_ALEXPLAY_NET_GDPR
using ACS.GDPR.Config;
#endif
#if COM_ALEXPLAY_NET_PURCHASE
using ACS.IAP.InAppPurchase.Config;
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL
using ACS.ObjectPool.ObjectPool.Config;
#endif
#if COM_ALEXPLAY_NET_DIALOG
using ACS.Dialog.Dialogs.Config;
#endif
#if COM_ALEXPLAY_NET_SIGNAL_BUS
using ACS.SignalBus.SignalBus.Config;
#endif
#if COM_ALEXPLAY_NET_DATA
using ACS.Data.DataService.Config;
#endif
#if COM_ALEXPLAY_NET_AUDIO
using ACS.Audio.StaticData;
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
using ACS.Analytics;
#endif
#if COM_ALEXPLAY_NET_FBRC
using ACS.FBRC.StaticData;
#endif
#if COM_ALEXPLAY_NET_CHEAT_TRACKER
using ACS.Cheat.CheatTracker;
#endif

namespace ACS.Core
{
        [Serializable]
        public class AlexplayCoreKitConfig : ScriptableObject
        {
                public CoreBootstrapOptions BootstrapOptions { get; private set; } = new();

#if COM_ALEXPLAY_NET_DATA
                public DataServiceConfig DataSettings { get; private set; } = new();
#endif
#if COM_ALEXPLAY_NET_SIGNAL_BUS
                public SignalBusServiceConfig SignalBusSettings { get; private set; } = new();
#endif
#if COM_ALEXPLAY_NET_DIALOG
                public DialogsServiceConfig DialogsSettings { get; private set; } = new();
#endif
#if COM_ALEXPLAY_NET_ADS
                public AdvertisementServiceConfig AdvertisementSettings { get; private set; } = new();
#endif
#if COM_ALEXPLAY_NET_AUDIO
                public AudioServiceConfig AudioSettings { get; private set; } = new();
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
                public AnalyticsServiceConfig AnalyticsSettings { get; private set; } = new();
#endif
#if COM_ALEXPLAY_NET_FBRC
                public FBRCConfig FbrcSettings { get; private set; } = new();
#endif

                private void OnValidate()
                {
#if COM_ALEXPLAY_NET_DIALOG
                        DialogsSettings.OnValidate();
#endif
                }
        }
}
