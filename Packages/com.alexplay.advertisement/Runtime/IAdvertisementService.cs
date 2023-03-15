using System;

namespace ACS.Ads.com.alexplay.advertisement.Runtime
{
    public interface IAdvertisementService
    {
        public event Action<bool> OnVideoStateChanged;
        public void ChangeOptions(AdvertisementOptions options);
        public bool IsInterstitialReady();
        public bool IsRewardedReady();
        public void CanPlayInterstitial(bool canPlay);
        public void CanPlayRewarded(bool canPlay);
        public void PlayRewarded(Action shownCallback, Action failedCallback = null);
        public void PlayInterstitial(Action shownCallback = null, Action failedCallback = null);
        public bool HasVideo();
    }
}