using System;
using Sirenix.OdinInspector;
using UnityEngine;

#if COM_ALEXPLAY_NET_ADS
using ACS.Ads;
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

namespace ACS.Core
{
    [Serializable]
    public class AlexplayCoreKitConfig : SerializedScriptableObject
    {
        public CoreBootstrapOptions _bootstrapOptions = new();
        
#if COM_ALEXPLAY_NET_DATA
        public DataServiceConfig _dataSettings = new();
#endif
#if COM_ALEXPLAY_NET_SIGNAL_BUS
        public SignalBusServiceConfig _signalBusSettings = new();
#endif
#if COM_ALEXPLAY_NET_DIALOG
        public DialogsServiceConfig _dialogsSettings = new();
#endif
#if COM_ALEXPLAY_NET_ADS
        public AdvertisementServiceConfig _advertisementSettings = new();
#endif
#if COM_ALEXPLAY_NET_AUDIO
        public AudioServiceConfig _audioSettings = new();
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
        public AnalyticsServiceConfig _analyticsSettings = new();
#endif
#if COM_ALEXPLAY_NET_FBRC
        public FBRCConfig _fbrcSettings = new();
#endif

        private void OnValidate()
        {
#if COM_ALEXPLAY_NET_DIALOG
            _dialogsSettings.OnValidate();
#endif
        }
    }
}
