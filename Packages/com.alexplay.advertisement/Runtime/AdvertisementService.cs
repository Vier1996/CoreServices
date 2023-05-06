using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

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

        private readonly AdvertisementServiceConfig _config;
        private readonly IntentService.IntentService _intentService;

        private bool _canPlayRewarded = true;
        private bool _canPlayInterstitial = true;

        private AdvertisementImpressionSender _advertisementImpressionSender;
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

            if(_config.HandleImpression)
                _advertisementImpressionSender = new AdvertisementImpressionSender(_config.IsDebug);
        }

        public void ChangeOptions(AdvertisementOptions options) => _options = options;

        public bool IsInterstitialReady()
        {
#if UNITY_EDITOR
            return true;
#endif
            return IronSource.Agent.isInterstitialReady();
        }

        public bool IsRewardedReady()
        {
#if UNITY_EDITOR
            return true;
#endif
            return IronSource.Agent.isRewardedVideoAvailable();
        }

        public void CanPlayInterstitial(bool canPlay) => _canPlayInterstitial = canPlay;
        public void CanPlayRewarded(bool canPlay) => _canPlayRewarded = canPlay;

        public void PlayRewarded(Action shownCallback, Action failedCallback = null, string place = "")
        {
            if (!_canPlayRewarded) return;

            _place = place;
            
#if UNITY_EDITOR
            shownCallback?.Invoke();
#else
            if (IronSource.Agent.isRewardedVideoAvailable() == false) return;
            
            _shownCallback = shownCallback;
            _failedCallback = failedCallback;
            _lastAdPlayingTime = DateTime.UtcNow;
            
            RewardedEventShow?.Invoke(_place);

            IronSource.Agent.showRewardedVideo();
#endif
        }

        public void PlayInterstitial(Action shownCallback = null, Action failedCallback = null, string place = "")
        {
            if (!_canPlayInterstitial) return;
            
            _place = place;

            if (Time.realtimeSinceStartup < _options.FreeInterstitialsAtStart) return;
            if (IronSource.Agent.isInterstitialReady() == false)
            {
                IronSource.Agent.loadInterstitial();
                return;
            }

            if ((DateTime.UtcNow - _lastAdPlayingTime).TotalSeconds > _options.InterstitialsTimeout)
            {
                _shownCallback = shownCallback;
                _failedCallback = failedCallback;
                
                IntersitialEventShow?.Invoke(_place);
                
                IronSource.Agent.showInterstitial();
                IronSource.Agent.loadInterstitial();
                
                _lastAdPlayingTime = DateTime.UtcNow;
            }
        }
        
        public bool HasVideo() => Application.isEditor || IronSource.Agent.isRewardedVideoAvailable();
        
        #region Initialization

        private void SetMetaSettings(bool accepted)
        {
            IronSource.Agent.setConsent(accepted);
            IronSource.Agent.setMetaData("do_not_sell",(!accepted).ToString());
        }

        private void InitIronSource()
        {
            IronSourceEvents.onSdkInitializationCompletedEvent += OnSdkInitializationCompletedEvent;

            List<string> adUnits = new List<string>();
            
            for (int i = 0; i < _config.Options.AdvertisementTypes.Count; i++)
            {
                switch (_config.Options.AdvertisementTypes[i])
                {
                    case AdvertisementType.INTERSTITIAL: 
                        adUnits.Add(IronSourceAdUnits.INTERSTITIAL);
                        break;
                    case AdvertisementType.REWARDED: 
                        adUnits.Add(IronSourceAdUnits.REWARDED_VIDEO);
                        break;
                    case AdvertisementType.BANNER: 
                        adUnits.Add(IronSourceAdUnits.BANNER);
                        break;
                }
            }
            
#if UNITY_ANDROID
            IronSource.Agent.init(_config.AndroidIdentifier, adUnits.ToArray());
#elif UNITY_IPHONE || UNITY_IOS
            IronSource.Agent.init(_config.IosIdentifier, adUnits.ToArray());
#endif
        }

        private void BindIronSourceEvents()
        {
            IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoAvailable;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoUnavailable;
            
            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoAdRewardedEvent; 
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoAdClickedEvent;
            
            IronSourceInterstitialEvents.onAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialAdLoadFailedEvent;        
            IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialAdShowSucceededEvent; 
            IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialAdShowFailedEvent; 
            IronSourceInterstitialEvents.onAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialAdClosedEvent;
            
            IronSourceBannerEvents.onAdLoadFailedEvent += BannerAdLoadFailedEvent;
            IronSourceBannerEvents.onAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
            IronSourceBannerEvents.onAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceBannerEvents.onAdClickedEvent += BannerAdClickedEvent;
            IronSourceBannerEvents.onAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
            IronSourceBannerEvents.onAdLeftApplicationEvent += BannerAdLeftApplicationEvent;
        }

        private void OnSdkInitializationCompletedEvent()
        {
            IronSourceEvents.onSdkInitializationCompletedEvent -= OnSdkInitializationCompletedEvent;
            PreloadAds().Forget(); 
        }

        private async UniTaskVoid PreloadAds()
        {
            await UniTask.Delay(_initDelay);
            
            IronSource.Agent.validateIntegration();
            await UniTask.Delay(_initDelay);

            if (_config.Options.AdvertisementTypes.Contains(AdvertisementType.INTERSTITIAL))
            {
                IronSource.Agent.loadInterstitial();
                await UniTask.Delay(_initDelay);
            }
        }

        #endregion
        
        private void OnVideoShown() => RewardPlayer();

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
            Time.timeScale = 1;
            
            DOTween.To(() => AudioListener.volume, volume =>
            {
                AudioListener.volume = volume;
            }, 1f, 1f);
        }

        private async void RewardPlayer()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            ResumeApplication();
            
            _shownCallback?.Invoke();
            _shownCallback = null;
        }
        
        public void Dispose()
        {
            _intentService.OnFocusChanged -= FocusChanged;
            _intentService.OnPauseChanged -= PauseChanged;
        }
        
        #region REWARDED_EVENTS
        public void RewardedVideoAdOpenedEvent(IronSourceAdInfo ironSourceAdInfo) => PauseApplication();
        public void RewardedVideoAdClosedEvent(IronSourceAdInfo ironSourceAdInfo) => ResumeApplication();

        public void RewardedVideoAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo ironSourceAdInfo)
        {
            OnVideoShown();
            
            RewardedEventShown?.Invoke(_place);
        }

        public void RewardedVideoAvailable(IronSourceAdInfo ironSourceAdInfo) => OnVideoStateChanged?.Invoke(true);
        public void RewardedVideoUnavailable() => OnVideoStateChanged?.Invoke(false);
        
        public void RewardedVideoAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo ironSourceAdInfo)
        {
            OnVideoFailed();
            
            RewardedEventCancel?.Invoke(_place);
        }

        public void RewardedVideoAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo ironSourceAdInfo) { }
        #endregion
        
        #region INTERSTITIAL_EVENTS
        public void InterstitialAdReadyEvent(IronSourceAdInfo ironSourceAdInfo) { }
        public void InterstitialAdLoadFailedEvent(IronSourceError ironSourceError) { }
        public void InterstitialAdShowSucceededEvent(IronSourceAdInfo ironSourceError)
        {
            OnVideoShown();
            
            IntersitialEventShown?.Invoke(_place);
        }

        public void InterstitialAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo ironSourceAdInfo)
        {
            OnVideoFailed();
            
            IntersitialEventCancel?.Invoke(_place);
        }

        public void InterstitialAdClickedEvent(IronSourceAdInfo ironSourceAdInfo) { }
        public void InterstitialAdOpenedEvent(IronSourceAdInfo ironSourceAdInfo) { }
        public void InterstitialAdClosedEvent(IronSourceAdInfo ironSourceAdInfo)
        {
            OnVideoFailed();
        }

        #endregion
        
        #region BANNER_EVENTS
        public void BannerAdLeftApplicationEvent(IronSourceAdInfo ironSourceAdInfo) { }
        public void BannerAdScreenDismissedEvent(IronSourceAdInfo ironSourceAdInfo) { }
        public void BannerAdScreenPresentedEvent(IronSourceAdInfo ironSourceAdInfo) { }
        public void BannerAdClickedEvent(IronSourceAdInfo ironSourceAdInfo) { }
        public void BannerAdLoadFailedEvent(IronSourceError obj) { }
        public void BannerAdLoadedEvent(IronSourceAdInfo ironSourceAdInfo) { }
        #endregion

        public enum AdvertisementType
        {
            NONE, 
            INTERSTITIAL,
            REWARDED,
            BANNER
        }
    }
}