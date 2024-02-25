using System;

namespace ACS.Ads
{
    public interface IAdvertisementService
    {
        public event Action<bool> OnVideoStateChanged;
        public event Action<string> RewardedEventShow;
        public event Action<string> RewardedEventShown;
        public event Action<string> RewardedEventCancel;
        
        public event Action<string> InterstitialEventShow;
        public event Action<string> InterstitialEventShown;
        public event Action<string> InterstitialEventCancel;
        
        public void ChangeOptions(AdvertisementOptions options);
        public bool IsInterstitialReady();
        public bool IsRewardedReady();
        public void CanPlayInterstitial(bool canPlay);
        public void CanPlayRewarded(bool canPlay);
        public void PlayRewarded(Action shownCallback, Action failedCallback = null, string place = "");
        public void PlayInterstitial(Action shownCallback = null, Action failedCallback = null, string place = "");
        public bool HasVideo();
        public void OverridePauseApplicationDelegate(PauseApplicationDelegate pauseDelegate);
        public void OverrideResumeApplicationDelegate(ResumeApplicationDelegate resumeDelegate);
        
        public delegate void PauseApplicationDelegate();
        public delegate void ResumeApplicationDelegate();
    }
}