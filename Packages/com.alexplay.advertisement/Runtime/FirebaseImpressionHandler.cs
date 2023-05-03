using System.Collections.Generic;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace ACS.Ads
{
    public class FirebaseImpressionHandler : IImpressionHandler
    {
        private bool _isDebug = false;
        private bool _isReady = false;

        public FirebaseImpressionHandler(bool isDebug)
        {
            _isDebug = isDebug;
            
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available) _isReady = true;
            });
        }

        public void HandleImpression(Dictionary<string, string> impressionParam, double revenue)
        {
            if(!_isReady) return;
            
            Parameter[] adParameters =
            {
                new Parameter(FirebaseAnalytics.ParameterAdPlatform, "ironsource"),
                new Parameter(FirebaseAnalytics.ParameterAdSource, impressionParam["adNetwork"]),
                new Parameter(FirebaseAnalytics.ParameterAdUnitName, impressionParam["adUnit"]),
                new Parameter(FirebaseAnalytics.ParameterCurrency, "USD"),
                new Parameter(FirebaseAnalytics.ParameterValue, revenue)
            };

            string impressionMediationKey = "";
            string impressionKey = "";
            string totalRevenueKey = "";
            string adRevenueKey = "";

            if (_isDebug)
            {
                impressionMediationKey = "ad_impression_mediation_test";
                impressionKey = "ad_impression_test";
                totalRevenueKey = "total_revenue_test";
                adRevenueKey = "ad_revenue_test";

                string info = "";
                info +=
                    $"[AdvertisementImpressionSender] firebase event: |{FirebaseAnalytics.ParameterCurrency}| : USD \n";
                info +=
                    $"[AdvertisementImpressionSender] firebase event: |{FirebaseAnalytics.ParameterValue}| : {revenue}";

                Debug.Log(info);
            }
            else
            {
                impressionMediationKey = "ad_impression_mediation";
                impressionKey = "ad_impression";
                totalRevenueKey = "total_revenue";
                adRevenueKey = "ad_revenue";
            }

            FirebaseAnalytics.LogEvent(impressionMediationKey, adParameters);
            FirebaseAnalytics.LogEvent(impressionKey, adParameters);
            
            FirebaseAnalytics.LogEvent(totalRevenueKey, 
                new Parameter(FirebaseAnalytics.ParameterValue, revenue), 
                new Parameter(FirebaseAnalytics.ParameterCurrency, "USD"));
            FirebaseAnalytics.LogEvent(adRevenueKey, 
                new Parameter(FirebaseAnalytics.ParameterValue, revenue),
                new Parameter(FirebaseAnalytics.ParameterCurrency, "USD"));
        }
    }
}