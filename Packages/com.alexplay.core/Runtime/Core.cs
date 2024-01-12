using System;
using ACS.Core.Internal.AlexplayCoreBootstrap;
using DG.Tweening;
using UnityEngine;

#if COM_ALEXPLAY_ZENJECT_EXTENSION
using Zenject;
#endif

#if COM_ALEXPLAY_NET_ADS
using ACS.Ads;
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
#if COM_ALEXPLAY_NET_FBRC
using ACS.FBRC;
#endif

namespace ACS.Core
{
    public class Core : CoreAbstract, IDisposable
    {
        public static event Action PreInitialized;
        public static event Action PostInitialized;
        
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
#if COM_ALEXPLAY_NET_FBRC
        public FBRCService FbrcService
        {
            get
            {
                if (_fbrcService == null)
                    throw new NullReferenceException($"Before using {typeof(FBRCService)} you must -turn (ON) it in config");

                return _fbrcService;
            }
        }
#endif
 
        private IntentService.IntentService IntentService { get; }
        
#if COM_ALEXPLAY_ZENJECT_EXTENSION
        private DiContainer _diContainer;
#endif
        private bool _initialized = false;

        public Core(AlexplayCoreKitConfig coreConfig, GameObject parentMonoBehavior, RectTransform rectForDialogs)
        {
            if (Instance == null)
            {
                Instance = this;
                
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                _diContainer = ProjectContext.Instance.Container;
#endif
                IntentService = parentMonoBehavior.AddComponent<IntentService.IntentService>();
                
                SetupOptions(coreConfig._bootstrapOptions);
                
#if COM_ALEXPLAY_NET_DATA
                if(coreConfig._dataSettings.IsEnabled)
                    _dataService = new DataService(coreConfig._dataSettings, IntentService);
#endif

#if COM_ALEXPLAY_NET_SIGNAL_BUS
                if(coreConfig._signalBusSettings.IsEnabled)
                    _signalBusService = 
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                        new SignalBusService(_diContainer);
#else
                        new SignalBusService();
#endif
#endif
#if COM_ALEXPLAY_NET_DIALOG
                if(coreConfig._dialogsSettings.IsEnabled)
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                    _dialogService = new DialogService(_diContainer, coreConfig._dialogsSettings, rectForDialogs);
#else
                    _dialogService = new DialogService(coreConfig._dialogsSettings, rectForDialogs);
#endif
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL
                if (coreConfig._objectPoolSettings.IsEnabled)
                    _objectPoolService = 
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                        new ObjectPoolService(parentMonoBehavior.transform, _diContainer, coreConfig._objectPoolSettings);
#else
                        new ObjectPoolService(parentMonoBehavior.transform, coreConfig._objectPoolSettings);
#endif
#endif
#if COM_ALEXPLAY_NET_ADS
                if (coreConfig._advertisementSettings.IsEnabled)
                    _advertisementService = new AdvertisementService(IntentService, coreConfig._advertisementSettings);
#endif
#if COM_ALEXPLAY_NET_GDPR
                if (coreConfig._gdprSettings.IsEnabled)
                    _gdprService = new GdprService(coreConfig._gdprSettings, rectForDialogs);
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
#if COM_ALEXPLAY_NET_FBRC
                if (coreConfig._fbrcSettings.IsEnabled)
                    _fbrcService = new FBRCService(coreConfig._fbrcSettings);
#endif
                
                PreInitialized?.Invoke();

                PrepareServices(coreConfig);
            }
            else Dispose();
        }

        private void SetupOptions(CoreBootstrapOptions bootstrapOptions)
        {

            switch (bootstrapOptions.FrameRateType)
            {
                case TargetFrameRateType.ADAPTIVE:
#if UNITY_EDITOR
                    Application.targetFrameRate = Screen.currentResolution.refreshRate;
#else
                    if (SystemInfo.systemMemorySize < 3000) Application.targetFrameRate = 30;
                    else if (SystemInfo.systemMemorySize < 4000) Application.targetFrameRate = 45;
                    else Application.targetFrameRate = Screen.currentResolution.refreshRate;
#endif
                    break;

                case TargetFrameRateType.CONSTANT:
                    Application.targetFrameRate = bootstrapOptions.TargetRate;
                    break;
                
                case TargetFrameRateType.UNLIMITED:
                    Application.targetFrameRate = -1;
                    break;
                
                case TargetFrameRateType.EQUALS:
                    Application.targetFrameRate = Screen.currentResolution.refreshRate;
                    break;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            if(bootstrapOptions.IgnoreScreenOrientation == false)
                Screen.orientation = bootstrapOptions.ScreenOrientation;
            
            DOTween.SetTweensCapacity(bootstrapOptions.TweenersCapacity, bootstrapOptions.SequencesCapacity);
        }

        private void PrepareServices(AlexplayCoreKitConfig coreConfig)
        {
#if COM_ALEXPLAY_NET_DATA
            if (coreConfig._dataSettings.IsEnabled)
            {
                _dataService.PrepareService();
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                _diContainer.Bind<IDataService>().FromInstance(_dataService);
#endif
            }
#endif
            
#if COM_ALEXPLAY_NET_AUDIO

            if (coreConfig._audioSettings.IsEnabled)
            {
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                _diContainer.Bind<AudioService>().FromInstance(_audioService);
#endif
            }
#endif
            
#if COM_ALEXPLAY_NET_SIGNAL_BUS
            if (coreConfig._signalBusSettings.IsEnabled)
            {
                _signalBusService.PrepareService();
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                _diContainer.Bind<ISignalBusService>().FromInstance(_signalBusService);
#endif
            }
#endif
            
#if COM_ALEXPLAY_NET_DIALOG
            if (coreConfig._dialogsSettings.IsEnabled)
            {
                _dialogService.PrepareService();
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                _diContainer.Bind<IDialogService>().FromInstance(_dialogService);
#endif
            }
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL
            if (coreConfig._objectPoolSettings.IsEnabled)
            {
                _objectPoolService.PrepareService();
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                _diContainer.Bind<IObjectPoolService>().FromInstance(_objectPoolService);
#endif
            }
#endif
#if COM_ALEXPLAY_NET_ADS
            if (coreConfig._advertisementSettings.IsEnabled)
            {
                _advertisementService.PrepareService();
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                _diContainer.Bind<IAdvertisementService>().FromInstance(_advertisementService);
#endif
            }
#endif
#if COM_ALEXPLAY_NET_GDPR
            if (coreConfig._gdprSettings.IsEnabled)
            {
                _gdprService.PrepareService();
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                _diContainer.Bind<IGdprService>().FromInstance(_gdprService);
#endif
            }
#endif
#if COM_ALEXPLAY_NET_PURCHASE
            if (coreConfig._purchaseSettings.IsEnabled)
            {
                _iapPurchaseService.PrepareService();
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                _diContainer.Bind<IPurchaseService>().FromInstance(_iapPurchaseService);
#endif
            }
#endif
#if COM_ALEXPLAY_NET_ANALYTICS && COM_ALEXPLAY_ZENJECT_EXTENSION
            if (coreConfig._analyticsSettings.IsEnabled)
            {
                _diContainer.Bind<IAnalyticsService>().FromInstance(_analyticsService).AsSingle();
            }
#endif
#if COM_ALEXPLAY_NET_FBRC && COM_ALEXPLAY_ZENJECT_EXTENSION

            if (coreConfig._fbrcSettings.IsEnabled)
            {
                _diContainer.BindInstance(_fbrcService).AsSingle();
            }
#endif
            OnServicesPrepared();
        }

        private void OnServicesPrepared()
        {
            _initialized = true;
            
            PostInitialized?.Invoke();
        }

        public void Dispose()
        {
            Instance = null;
#if COM_ALEXPLAY_NET_DATA
            _dataService?.Dispose();
#endif
        }
    }
}
