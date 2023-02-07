#if COM_ALEXPLAY_NET_ADS
using ACS.Ads;
using ACS.Ads.com.alexplay.advertisement.Runtime;
#endif
#if COM_ALEXPLAY_NET_GDPR
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
using ACS.Analytics;
#endif
#if COM_ALEXPLAY_NET_DIALOG
using ACS.Dialog.Dialogs;
#endif
#if COM_ALEXPLAY_NET_PURCHASE
using ACS.IAP.InAppPurchase.Service;
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL
using ACS.ObjectPool.ObjectPool;
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
#if COM_ALEXPLAY_NET_GDPR
using ACS.GDPR.Service;
#endif
using System;
using UnityEngine;
using Zenject;

namespace ACS.Core
{
    public class Core : CoreAbstract, IDisposable
    {
        public event Action OnInitialized;
        
        public static Core Instance;
        
        public bool Initialized => _initialized;
        
#if COM_ALEXPLAY_NET_DATA
        public IDataService DataService
        {
            get
            {
                
                if (_dataService == null)
                    throw new NullReferenceException($"Before using {typeof(DataService)} you must -turn (ON) it in config");

                return _dataService;
            }
        }
#endif

#if COM_ALEXPLAY_NET_SIGNAL_BUS
        public ISignalBusService SignalBusService
        {
            get
            {
                if (_signalBusService == null)
                    throw new NullReferenceException($"Before using {typeof(SignalBusService)} you must -turn (ON) it in config");

                return _signalBusService;
            }
        }
#endif
#if COM_ALEXPLAY_NET_DIALOG
        public IDialogService DialogService
        {
            get
            {
                if (_dialogService == null)
                    throw new NullReferenceException($"Before using {typeof(DialogService)} you must -turn (ON) it in config");

                return _dialogService;
            }
        }
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL

        public IObjectPoolService ObjectPoolService
        {
            get
            {
                if (_objectPoolService == null)
                    throw new NullReferenceException($"Before using {typeof(ObjectPoolService)} you must -turn (ON) it in config");

                return _objectPoolService;
            }
        }
#endif
#if COM_ALEXPLAY_NET_ADS
        public IAdvertisementService AdvertisementService
        {
            get
            {
                if (_advertisementService == null)
                    throw new NullReferenceException($"Before using {typeof(AdvertisementService)} you must -turn (ON) it in config");

                return _advertisementService;
            }
        }
#endif
#if COM_ALEXPLAY_NET_GDPR
        public IGdprService GdprService
        {
            get
            {
                if (_gdprService == null)
                    throw new NullReferenceException($"Before using {typeof(GdprService)} you must -turn (ON) it in config");

                return _gdprService;
            }
        }
#endif
#if COM_ALEXPLAY_NET_AUDIO
        public AudioService AudioService
        {
            get
            {
                if (_audioService == null)
                    throw new NullReferenceException($"Before using AudioService you must -turn (ON) it in config");

                return _audioService;
            }
        }
#endif
#if COM_ALEXPLAY_NET_PURCHASE
        public IPurchaseService PurchaseService
        {
            get
            {
                if (_iapPurchaseService == null)
                    throw new NullReferenceException($"Before using {typeof(IAPPurchaseService)} you must -turn (ON) it in config");

                return _iapPurchaseService;
            }
        }
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
        public AnalyticsService AnalyticsService
        {
            get
            {
                if (_analyticsService == null)
                    throw new NullReferenceException($"Before using {typeof(AnalyticsService)} you must -turn (ON) it in config");

                return _analyticsService;
            }
        }
#endif
        private IntentService.IntentService IntentService { get; }
        private DiContainer _diContainer;
        private bool _initialized = false;
        
        public Core(AlexplayCoreKitConfig coreConfig, RectTransform dialogRect, GameObject parentMonoBehavior)
        {
            if (Instance == null)
            {
                Instance = this;
                _diContainer = ProjectContext.Instance.Container;
                IntentService = parentMonoBehavior.AddComponent<IntentService.IntentService>();
                
                Application.targetFrameRate = 60;
                
#if COM_ALEXPLAY_NET_DATA
                if(coreConfig._dataSettings.IsEnabled)
                    _dataService = new DataService(coreConfig._dataSettings);
#endif

#if COM_ALEXPLAY_NET_SIGNAL_BUS
                if(coreConfig._signalBusSettings.IsEnabled)
                    _signalBusService = new SignalBusService(_diContainer);
#endif
#if COM_ALEXPLAY_NET_DIALOG
                if(coreConfig._dialogsSettings.IsEnabled)
                    _dialogService = new DialogService(_diContainer, coreConfig._dialogsSettings, dialogRect);
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL
                if (coreConfig._objectPoolSettings.IsEnabled)
                    _objectPoolService = new ObjectPoolService(parentMonoBehavior.transform, _diContainer, coreConfig._objectPoolSettings);
#endif
#if COM_ALEXPLAY_NET_ADS
                if (coreConfig._advertisementSettings.IsEnabled)
                    _advertisementService = new AdvertisementService(IntentService, coreConfig._advertisementSettings);
#endif
#if COM_ALEXPLAY_NET_GDPR
                if (coreConfig._gdprSettings.IsEnabled)
                    _gdprService = new GdprService(coreConfig._gdprSettings, dialogRect);
#endif
#if COM_ALEXPLAY_NET_AUDIO
                if (coreConfig._audioSettings.IsEnabled)
                    _audioService = new AudioService(coreConfig._audioSettings);
#endif
#if COM_ALEXPLAY_NET_PURCHASE
                if (coreConfig._purchaseSettings.IsEnabled)
                    _iapPurchaseService = new IAPPurchaseService(coreConfig._purchaseSettings);
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
                if (coreConfig._analyticsSettings.IsEnabled)
                    _analyticsService = new AnalyticsService(coreConfig._analyticsSettings);
#endif
                PrepareServices(coreConfig);
            }
            else Dispose();
        }

        private void PrepareServices(AlexplayCoreKitConfig coreConfig)
        {
#if COM_ALEXPLAY_NET_DATA
            if (coreConfig._dataSettings.IsEnabled)
            {
                _dataService.PrepareService();
                _diContainer.Bind<IDataService>().FromInstance(_dataService);
            }
#endif
            
#if COM_ALEXPLAY_NET_AUDIO

            if (coreConfig._audioSettings.IsEnabled)
            {
                _dataService.PrepareService();
                _diContainer.Bind<AudioService>().FromInstance(_audioService);
            }
#endif
            
#if COM_ALEXPLAY_NET_SIGNAL_BUS
            if (coreConfig._signalBusSettings.IsEnabled)
            {
                _signalBusService.PrepareService();
                _diContainer.Bind<ISignalBusService>().FromInstance(_signalBusService);
            }
#endif
            
#if COM_ALEXPLAY_NET_DIALOG
            if (coreConfig._dialogsSettings.IsEnabled)
            {
                _dialogService.PrepareService();
                _diContainer.Bind<IDialogService>().FromInstance(_dialogService);
            }
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL
            if (coreConfig._objectPoolSettings.IsEnabled)
            {
                _objectPoolService.PrepareService();
                _diContainer.Bind<IObjectPoolService>().FromInstance(_objectPoolService);
            }
#endif
#if COM_ALEXPLAY_NET_ADS
            if (coreConfig._advertisementSettings.IsEnabled)
            {
                _advertisementService.PrepareService();
                _diContainer.Bind<IAdvertisementService>().FromInstance(_advertisementService);
            }
#endif
#if COM_ALEXPLAY_NET_GDPR
            if (coreConfig._gdprSettings.IsEnabled)
            {
                _gdprService.PrepareService();
                _diContainer.Bind<IGdprService>().FromInstance(_gdprService);
            }
#endif
#if COM_ALEXPLAY_NET_PURCHASE
            if (coreConfig._purchaseSettings.IsEnabled)
            {
                _iapPurchaseService.PrepareService();
                _diContainer.Bind<IPurchaseService>().FromInstance(_iapPurchaseService);
            }
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
            if (coreConfig._analyticsSettings.IsEnabled)
            {
                _diContainer.BindInstance(_analyticsService).AsSingle();
            }
#endif
            OnServicesPrepared();
        }

        private void OnServicesPrepared()
        {
            _initialized = true;
            OnInitialized?.Invoke();
        }

        public void Dispose()
        {
            Instance = null;
#if COM_ALEXPLAY_NET_DATA
            _dataService?.Dispose();
#endif
            
#if COM_ALEXPLAY_NET_SIGNAL_BUS
            _signalBusService?.Dispose();
#endif
        }
    }
}
