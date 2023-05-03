using System.Collections.Generic;
using AppsFlyerSDK;
using UnityEngine;

namespace ACS.Ads
{
    public class AppsFlyerImpressionHandler : IImpressionHandler
    {
        private bool _isDebug = false;
        
        public AppsFlyerImpressionHandler(bool isDebug) => _isDebug = isDebug;

        public void HandleImpression(Dictionary<string, string> impressionParam, double revenue)
        {
            if(!(AppsFlyer.instance is { isInit: true })) return;
            
            string impressionKey = "";
            string adRevenueKey = "";

            if (_isDebug)
            {
                impressionKey = "ad_impression_test";
                adRevenueKey = "ad_revenue_test";

                string info = "";
                        
                foreach (var adsParam in impressionParam)
                    info += $"[AdvertisementImpressionSender] appsflyer event: |{adsParam.Key}| : {adsParam.Value} \n";
                        
                Debug.Log(info);
            }
            else
            {
                impressionKey = "ad_impression";
                adRevenueKey = "ad_revenue";
            }

            AppsFlyer.sendEvent(impressionKey, impressionParam);
            AppsFlyer.sendEvent(adRevenueKey, impressionParam);
        }
    }
}