using System;
using ACS.Core.ServicesContainer;
using DG.Tweening;
using Intent;
using UnityEngine;

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
#if COM_ALEXPLAY_NET_SIGNAL_BUS
using ACS.SignalBus.SignalBus;
#endif
#if COM_ALEXPLAY_NET_DATA
using ACS.Data.DataService.Service;
#endif
#if COM_ALEXPLAY_NET_AUDIO
using ACS.Audio;
#endif
#if COM_ALEXPLAY_NET_FBRC
using ACS.FBRC;
#endif

namespace ACS.Core
{
    public class Core
    {
        public static event Action PreInitialized;
        public static event Action PostInitialized;

        public static bool Initialized { get; private set; } = false;
        
        public Core(CoreInstallParams installParams)
        {
            PreInitialized?.Invoke();
            
            ServiceContainer container = installParams.Installer.AddComponent<ServiceContainer>();
            IntentService intentService = installParams.Installer.AddComponent<IntentService>();
            
            SetupOptions(installParams.CoreConfig._bootstrapOptions);

#if COM_ALEXPLAY_NET_DATA
            container.Register(typeof(IDataService), 
                new DataService(installParams.CoreConfig._dataSettings, intentService)); 
#endif

#if COM_ALEXPLAY_NET_SIGNAL_BUS
            container.Register(typeof(ISignalBusService), 
                new SignalBusService()); 
#endif

#if COM_ALEXPLAY_NET_DIALOG
            container.Register(typeof(IDialogService),
                new DialogService(installParams.CoreConfig._dialogsSettings, installParams.DialogRect)
                    .AddRenderModeChangeDelegate(installParams.RenderModeChangeDelegate)
            );
#endif

#if COM_ALEXPLAY_NET_ADS
            container.Register(typeof(IAdvertisementService), 
                new AdvertisementService(intentService, installParams.CoreConfig._advertisementSettings)); 
#endif

#if COM_ALEXPLAY_NET_AUDIO
            container.Register(typeof(IAudioService), 
                new AudioService(installParams.CoreConfig._audioSettings)); 
#endif

#if COM_ALEXPLAY_NET_ANALYTICS
            container.Register(typeof(IAnalyticsService),
                new AnalyticsService(installParams.CoreConfig._analyticsSettings)); 
#endif

#if COM_ALEXPLAY_NET_FBRC
                container.Register(typeof(IFBRCService), 
                    new FBRCService(installParams.CoreConfig._fbrcSettings)); 
#endif
            
            container.AsCore();
            
            PostInitialized?.Invoke();
        }

        private void SetupOptions(CoreBootstrapOptions bootstrapOptions)
        {

            switch (bootstrapOptions.FrameRateType)
            {
                case TargetFrameRateType.ADAPTIVE:
#if UNITY_EDITOR
                    Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
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
                    Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
                    break;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            if(bootstrapOptions.IgnoreScreenOrientation == false)
                Screen.orientation = bootstrapOptions.ScreenOrientation;
            
            DOTween.SetTweensCapacity(bootstrapOptions.TweenersCapacity, bootstrapOptions.SequencesCapacity);
        }

        public class CoreInstallParams
        {
            public AlexplayCoreKitConfig CoreConfig;
            public GameObject Installer;
            public RectTransform DialogRect;
            
            public Action<RenderMode> RenderModeChangeDelegate;
        }
    }
}
