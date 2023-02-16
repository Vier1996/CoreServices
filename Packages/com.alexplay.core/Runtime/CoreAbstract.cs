#if COM_ALEXPLAY_NET_ADS
using ACS.Ads;
#endif
#if COM_ALEXPLAY_NET_GDPR
using ACS.GDPR.Service;
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
using ACS.Analytics;
using ACS.FBRC;
#endif
#if COM_ALEXPLAY_NET_DIALOG
using ACS.Dialog.Dialogs;
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL
using ACS.ObjectPool.ObjectPool;
#endif
#if COM_ALEXPLAY_NET_PURCHASE
using ACS.IAP.InAppPurchase.Service;
#endif
#if COM_ALEXPLAY_NET_SIGNAL_BUS
using ACS.SignalBus.SignalBus;
#endif
#if COM_ALEXPLAY_NET_DATA
using ACS.Data.DataService.Service;
#endif
#if COM_ALEXPLAY_NET_AUDIO
using ACS.Audio;
#endif

namespace ACS.Core
{
    public abstract class CoreAbstract
    {
#if COM_ALEXPLAY_NET_DATA
        protected DataService _dataService = null;
#endif
#if COM_ALEXPLAY_NET_SIGNAL_BUS
        protected SignalBusService _signalBusService = null;
#endif
#if COM_ALEXPLAY_NET_DIALOG
        protected DialogService _dialogService = null;
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL
        protected ObjectPoolService _objectPoolService = null;
#endif
#if COM_ALEXPLAY_NET_ADS
        protected AdvertisementService _advertisementService = null;
#endif
#if COM_ALEXPLAY_NET_GDPR
        protected GdprService _gdprService = null;
#endif
#if COM_ALEXPLAY_NET_AUDIO
        protected AudioService _audioService = null;
#endif
#if COM_ALEXPLAY_NET_PURCHASE
        protected IAPPurchaseService _iapPurchaseService = null;
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
        protected AnalyticsService _analyticsService = null;
#endif
#if COM_ALEXPLAY_NET_FBRC
        protected FBRCService _fbrcService = null;
#endif
    }
}