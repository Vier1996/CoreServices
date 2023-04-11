using System;
using System.Collections.Generic;
using ACS.Ads.com.alexplay.advertisement.Runtime;
using Cysharp.Threading.Tasks;
using IS.IronSource.Scripts;
using UnityEngine;
using IronS = IS.IronSource.Scripts.IronSource;

namespace ACS.Ads
{
    public class AdvertisementService : IAdvertisementService, IDisposable
    {
        public event Action<bool> OnVideoStateChanged;
        
        public event Action<string> RewardedEventShow;
        public event Action<string> RewardedEventShown;
        public event Action<string> RewardedEventCancel;
        
        public event Action<string> IntersitialEventShow;
        public event Action<string> IntersitialEventShown;
        public event Action<string> IntersitialEventCancel;
        
        public event Action RewardedShown;
        public event Action InterstitialShown;

        private readonly AdvertisementServiceConfig _config;
        private readonly IntentService.IntentService _intentService;

        private bool _canPlayRewarded = true;
        private bool _canPlayInterstitial = true;

        private AdvertisementOptions _options;
        private DateTime _lastAdPlayingTime;
        private Action _shownCallback;
        private Action _failedCallback;

        private string _place = "";
        
        private readonly TimeSpan _initDelay;

        public AdvertisementService(IntentService.IntentService intentService, AdvertisementServiceConfig config)
        {
            _intentService = intentService;
            _initDelay = TimeSpan.FromSeconds(1);
            _lastAdPlayingTime = DateTime.UtcNow;
            _config = config;
            _options = _config.Options;
        }
        
        public void PrepareService()
        {
            _intentService.OnFocusChanged += FocusChanged;
            _intentService.OnPauseChanged += PauseChanged;

            SetMetaSettings(true);
            InitIronSource();
            BindIronSourceEvents();
        }

        public void ChangeOptions(AdvertisementOptions options) => _options = options;

        public bool IsInterstitialReady()
        {
#if UNITY_EDITOR
            return true;
#endif
            return IS.IronSource.Scripts.IronSource.Agent.isInterstitialReady();
        }

        public bool IsRewardedReady()
        {
#if UNITY_EDITOR
            return true;
#endif
            return IS.IronSource.Scripts.IronSource.Agent.isRewardedVideoAvailable();
        }

        public void CanPlayInterstitial(bool canPlay) => _canPlayInterstitial = canPlay;
        public void CanPlayRewarded(bool canPlay) => _canPlayRewarded = canPlay;

        public void PlayRewarded(Action shownCallback, Action failedCallback = null, string place = "")
        {
            if (!_canPlayRewarded) return;

            _place = place;
            
            shownCallback += () => RewardedShown?.Invoke();

#if UNITY_EDITOR
            shownCallback?.Invoke();
#else
            if (IronS.Agent.isRewardedVideoAvailable() == false) return;
            
            _shownCallback = shownCallback;
            _failedCallback = failedCallback;
            _lastAdPlayingTime = DateTime.UtcNow;
            
            RewardedEventShow?.Invoke(_place);

            IronS.Agent.showRewardedVideo();
#endif
        }

        public void PlayInterstitial(Action shownCallback = null, Action failedCallback = null, string place = "")
        {
            if (!_canPlayInterstitial) return;
            
            _place = place;

            if (Time.realtimeSinceStartup < _options.FreeInterstitialsAtStart) return;
            if (IronS.Agent.isInterstitialReady() == false)
            {
                IS.IronSource.Scripts.IronSource.Agent.loadInterstitial();
                return;
            }

            if ((DateTime.UtcNow - _lastAdPlayingTime).TotalSeconds < _options.InterstitialsTimeout)
            {
                shownCallback += () => InterstitialShown?.Invoke();
                
                _shownCallback = shownCallback;
                _failedCallback = failedCallback;
                
                IntersitialEventShow?.Invoke(_place);
                
                IS.IronSource.Scripts.IronSource.Agent.showInterstitial();
                IS.IronSource.Scripts.IronSource.Agent.loadInterstitial();
                
                _lastAdPlayingTime = DateTime.UtcNow;
            }
        }
        
        public bool HasVideo() => Application.isEditor || IronS.Agent.isRewardedVideoAvailable();
        
        #region Initialization

        private void SetMetaSettings(bool accepted)
        {
            IronS.Agent.setConsent(accepted);
            IronS.Agent.setMetaData("do_not_sell",(!accepted).ToString());
        }

        private void InitIronSource()
        {
            IronSourceEvents.onSdkInitializationCompletedEvent += OnSdkInitializationCompletedEvent;
#if UNITY_ANDROID
            IronS.Agent.init(_config.AndroidIdentifier, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER);
#elif UNITY_IOS
            //IronSource.Agent.init(_config.IosIdentifier, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER);
#endif
        }

        private void BindIronSourceEvents()
        {
            IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent; 
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent; 
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
            
            IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;        
            IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent; 
            IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent; 
            IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
            IronSourceEvents.onImpressionSuccessEvent += ImpressionSuccessEvent;

            IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
            IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
            IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
            IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
            IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;
        }

        private void OnSdkInitializationCompletedEvent()
        {
            IronSourceEvents.onSdkInitializationCompletedEvent -= OnSdkInitializationCompletedEvent;
            PreloadAds().Forget();
        }

        private async UniTaskVoid PreloadAds()
        {
            await UniTask.Delay(_initDelay);
            
            IS.IronSource.Scripts.IronSource.Agent.validateIntegration();
            await UniTask.Delay(_initDelay);
            
            IS.IronSource.Scripts.IronSource.Agent.loadInterstitial();
            await UniTask.Delay(_initDelay);
        }

        #endregion
        
        private void OnVideoShown()
        {
            ResumeApplication();
            RewardPlayer();
        }

        private void OnVideoFailed()
        {
            _failedCallback?.Invoke();
            _failedCallback = null;
            ResumeApplication();
        }
        
        private void PauseChanged(bool pauseStatus)
        {
#if !UNITY_EDITOR
            IronSource.Agent.onApplicationPause(pauseStatus);
#endif
        }

        private void FocusChanged(bool focus)
        {
#if !UNITY_EDITOR
            IronSource.Agent.onApplicationPause(!focus);
#endif
        }

        private void PauseApplication()
        {
            AudioListener.volume = 0;
            Time.timeScale = 0;
        }
        
        private void ResumeApplication()
        {
            AudioListener.volume = 1;
            Time.timeScale = 1;
        }

        private async void RewardPlayer()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            
            _shownCallback?.Invoke();
            _shownCallback = null;
        }
        
        public void Dispose()
        {
            _intentService.OnFocusChanged -= FocusChanged;
            _intentService.OnPauseChanged -= PauseChanged;
        }
        
        #region REWARDED_EVENTS
        public void RewardedVideoAdOpenedEvent() { }
        public void RewardedVideoAdClosedEvent()
        {
            ResumeApplication();
            
            RewardedEventCancel?.Invoke(_place);
        }

        public void RewardedVideoAdRewardedEvent(IronSourcePlacement ironSourcePlacement)
        {
            OnVideoShown();
            
            RewardedEventShown?.Invoke(_place);
        }

        public void RewardedVideoAvailabilityChangedEvent(bool b) => OnVideoStateChanged?.Invoke(b);
        public void RewardedVideoAdStartedEvent() => PauseApplication();
        public void RewardedVideoAdEndedEvent() => ResumeApplication();
        public void RewardedVideoAdShowFailedEvent(IronSourceError ironSourceError) => OnVideoFailed();
        public void RewardedVideoAdClickedEvent(IronSourcePlacement ironSourcePlacement) { }
        #endregion
        
        #region INTERSTITIAL_EVENTS
        public void InterstitialAdReadyEvent() { }
        public void InterstitialAdLoadFailedEvent(IronSourceError ironSourceError) { }
        public void InterstitialAdShowSucceededEvent()
        {
            OnVideoShown();
            
            IntersitialEventShown?.Invoke(_place);
        }

        public void InterstitialAdShowFailedEvent(IronSourceError ironSourceError) => OnVideoFailed();
        public void InterstitialAdClickedEvent() { }
        public void InterstitialAdOpenedEvent() { }
        public void InterstitialAdClosedEvent()
        {
            OnVideoFailed();
            
            IntersitialEventCancel?.Invoke(_place);
        }

        #endregion
        
        #region BANNER_EVENTS
        public void BannerAdLeftApplicationEvent() { }
        public void BannerAdScreenDismissedEvent() { }
        public void BannerAdScreenPresentedEvent() { }
        public void BannerAdClickedEvent() { }
        public void BannerAdLoadFailedEvent(IronSourceError obj) { }
        public void BannerAdLoadedEvent() { }
        #endregion
        
        #region IMPRESSION_EVENT
        public void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
        {
            if (impressionData != null)
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["ad_platform"] = "ironSource";
                param["adNetwork"] = impressionData.adNetwork;
                param["adUnit"] = impressionData.adUnit;
                param["instanceName"] = impressionData.instanceName;
                param["currency"] = "USD";
                param["value"] = impressionData.revenue.ToString();
                param["auctionId"] = impressionData.auctionId;
                param["lifetimeRevenue"] = impressionData.lifetimeRevenue.ToString();
                param["country"] = impressionData.country;
                param["ab"] = impressionData.ab;
                param["segmentName"] = impressionData.segmentName;
                param["placement"] = impressionData.placement;
                param["instanceId"] = impressionData.instanceId;
                param["precision"] = impressionData.precision;
                param["encryptedCPM"] = impressionData.encryptedCPM;
                param["conversionValue"] = impressionData.conversionValue.ToString();
            }
        }
        #endregion
    }
}