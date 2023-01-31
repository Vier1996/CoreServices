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
using ACS.Analytics.Analytics.Config;
#endif

namespace ACS.Core
{
    [CreateAssetMenu(fileName = "AlexplayCoreKitConfig", menuName = "Core/AlexplayCoreKitConfig")]
    [Serializable]
    public class AlexplayCoreKitConfig : ScriptableObject
    {
#if COM_ALEXPLAY_NET_DATA
        public DataServiceConfig _dataSettings = new DataServiceConfig();
#endif
#if COM_ALEXPLAY_NET_SIGNAL_BUS
        public SignalBusServiceConfig _signalBusSettings = new SignalBusServiceConfig();
#endif
#if COM_ALEXPLAY_NET_DIALOG
        public DialogsServiceConfig _dialogsSettings = new DialogsServiceConfig();
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL
        public ObjectPoolConfig _objectPoolSettings = new ObjectPoolConfig();
#endif
#if COM_ALEXPLAY_NET_ADS
        public AdvertisementServiceConfig _advertisementSettings = new AdvertisementServiceConfig();
#endif
#if COM_ALEXPLAY_NET_GDPR
        public GdprConfig _gdprSettings = new GdprConfig();
#endif
#if COM_ALEXPLAY_NET_AUDIO
        public AudioServiceConfig _audioSettings = new AudioServiceConfig();
#endif
#if COM_ALEXPLAY_NET_PURCHASE
        public PurchaseServiceConfig _purchaseSettings = new PurchaseServiceConfig();
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
        public AnalyticsServicesConfig _analyticsSettings = new AnalyticsServicesConfig();
#endif

        private void OnValidate()
        {
#if COM_ALEXPLAY_NET_DIALOG
            _dialogsSettings.OnValidate();
#endif
        }
    }
}
