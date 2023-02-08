using ACS.Ads.com.alexplay.advertisement.Runtime;
using ACS.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alexplay.Samples.Advertisement.Scripts
{
    public class AdvertisementSample : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _info; 
        
        [SerializeField] private Button _hasVideoButton; 
        [SerializeField] private Button _playInterstitialButton; 
        [SerializeField] private Button _playRewardedButton; 
        
        private IAdvertisementService _advertisementService;
        
        private void Start()
        {
            _advertisementService = Core.Instance.AdvertisementService;
            
            _hasVideoButton.onClick.AddListener(CheckHasVideo);
            _playInterstitialButton.onClick.AddListener(PlayInterstitial);
            _playRewardedButton.onClick.AddListener(PlayRewarded);
        }

        private void CheckHasVideo() => _info.text = "Is has video: " + _advertisementService.HasVideo();

        private void PlayInterstitial()
        {
            _advertisementService.PlayInterstitial();
            _info.text = "Played interstitial";
        }

        private void PlayRewarded() => _advertisementService.PlayRewarded(OnPlayCallback);

        private void OnPlayCallback() => _info.text = "Played rewarded";
    }
}
