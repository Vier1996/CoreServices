using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IS.IronSource.Scripts;
using UniRx;
using UnityEngine;

namespace ACS.Ads
{
    public class AdvertisementImpressionSender : IDisposable
    {
        private int _userImpressions = 0;
        private float _userAdRevenue = 0;
        private float _userEcpm = 0;

        private IDisposable _disposable;
        private float _disposableTimer;
        private const float _disposableDelay = 1f;
        
        private IImpressionHandler _appsFlyerImpressionHandler;
        private IImpressionHandler _firebaseImpressionHandler;
        private List<IronSourceImpressionData> _impressionsQueue = new List<IronSourceImpressionData>();
        private Dictionary<string, string> _impressionParams = new Dictionary<string, string>();
        private List<ImpressionAchievement> _achievements = new List<ImpressionAchievement>
        {
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalRevenue, Value = 0.5f, Achieved = false, NumberOfAchievement = 8},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalRevenue, Value = 1, Achieved = false, NumberOfAchievement = 9},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalRevenue, Value = 2, Achieved = false, NumberOfAchievement = 10},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalRevenue, Value = 4, Achieved = false, NumberOfAchievement = 11},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalRevenue, Value = 6, Achieved = false, NumberOfAchievement = 12},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalRevenue, Value = 7, Achieved = false, NumberOfAchievement = 13},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalRevenue, Value = 10, Achieved = false, NumberOfAchievement = 14},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalRevenue, Value = 15, Achieved = false, NumberOfAchievement = 15},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalEcpm, Value = 5, Achieved = false, NumberOfAchievement = 0},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalEcpm, Value = 15, Achieved = false, NumberOfAchievement = 1},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalEcpm, Value = 25, Achieved = false, NumberOfAchievement = 2},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalEcpm, Value = 35, Achieved = false, NumberOfAchievement = 3},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalEcpm, Value = 45, Achieved = false, NumberOfAchievement = 4},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalEcpm, Value = 65, Achieved = false, NumberOfAchievement = 5},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalEcpm, Value = 85, Achieved = false, NumberOfAchievement = 6},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalEcpm, Value = 100, Achieved = false, NumberOfAchievement = 7},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalImpressions, Value = 1, Achieved = false, NumberOfAchievement = 16},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalImpressions, Value = 5, Achieved = false, NumberOfAchievement = 17},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalImpressions, Value = 10, Achieved = false, NumberOfAchievement = 18},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalImpressions, Value = 20, Achieved = false, NumberOfAchievement = 24},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalImpressions, Value = 25, Achieved = false, NumberOfAchievement = 19},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalImpressions, Value = 45, Achieved = false, NumberOfAchievement = 20},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalImpressions, Value = 100, Achieved = false, NumberOfAchievement = 21},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalImpressions, Value = 200, Achieved = false, NumberOfAchievement = 22},
            new ImpressionAchievement {Type = ImpressionAchievementType.TotalImpressions, Value = 500, Achieved = false, NumberOfAchievement = 23}
        };

        public AdvertisementImpressionSender(bool isDebug)
        {
            IronSourceEvents.onImpressionDataReadyEvent += ImpressionHandle;
            
            _appsFlyerImpressionHandler = new AppsFlyerImpressionHandler(isDebug);
            _firebaseImpressionHandler = new FirebaseImpressionHandler(isDebug);
            _disposableTimer = _disposableDelay;
            
            Setup();
        }
        
        private void Setup()
        {
            if (PlayerPrefs.HasKey("userImpressions") == false)
            {
                PlayerPrefs.SetInt("userImpressions", _userImpressions);
                PlayerPrefs.SetFloat("userAdRevenue", _userAdRevenue);
                PlayerPrefs.SetFloat("userEcpm", _userEcpm);

                foreach (var achivement in _achievements)
                    PlayerPrefs.SetInt("impressionAchivement_" + achivement.NumberOfAchievement, 0);

                PlayerPrefs.Save();
            }
            else
            {
                _userImpressions = PlayerPrefs.GetInt("userImpressions");
                _userAdRevenue = PlayerPrefs.GetFloat("userAdRevenue");
                _userEcpm = PlayerPrefs.GetFloat("userEcpm");

                foreach (var achivement in _achievements)
                    achivement.Achieved =
                        PlayerPrefs.GetInt("impressionAchivement_" + achivement.NumberOfAchievement, 0) != 0;

                PlayerPrefs.Save();
            }

            _disposable = Observable.EveryFixedUpdate().Subscribe(OnTick);
        }

        private void OnTick(long next)
        {
            if (_disposableTimer > 0)
            {
                _disposableTimer -= Time.fixedDeltaTime;
                return;
            }
            
            _disposableTimer = _disposableDelay;

            UpdateInfo();
        }

        private void UpdateInfo()
        {
            for (;;)
            {
                if (_impressionsQueue.Any())
                {
                    if (_impressionsQueue.Count > 0)
                    {
                        foreach (var data in _impressionsQueue)
                        {
                            HandleImpression(data);
                        }
                    }

                    if (_impressionsQueue.Count > 0)
                        _impressionsQueue.RemoveAll(x => true);
                }
            }
        }
        
        private void ImpressionHandle(IronSourceImpressionData imprInfo)
        {
            if (imprInfo.adUnit == "banner")
                return;

            _impressionsQueue.Add(imprInfo);
        }

        private void HandleImpression(IronSourceImpressionData impressionData)
        {
            double revenue = impressionData.revenue.HasValue ? impressionData.revenue.Value : 0;

            SendImpressionMetrics(impressionData, revenue);

            _userImpressions++;
            _userAdRevenue += (float)revenue;
            _userEcpm = _userAdRevenue / _userImpressions * 1000;

            PlayerPrefs.SetInt("userImpressions", _userImpressions);
            PlayerPrefs.SetFloat("userAdRevenue", _userAdRevenue);
            PlayerPrefs.SetFloat("userECPM", _userEcpm);

            PlayerPrefs.Save();

            foreach (var achivement in _achievements)
            {
                if (achivement.Achieved) continue;

                if (achivement.Achieved)
                    PlayerPrefs.SetInt("impressionAchivement_" + achivement.NumberOfAchievement, 1);
            }
        }

        private void SendImpressionMetrics(IronSourceImpressionData impressionData, double revenue)
        {
            if (impressionData != null)
            {
                _impressionParams["ad_platform"] = "ironSource";
                _impressionParams["adNetwork"] = impressionData.adNetwork;
                _impressionParams["adUnit"] = impressionData.adUnit;
                _impressionParams["instanceName"] = impressionData.instanceName;
                _impressionParams["currency"] = "USD";
                _impressionParams["value"] = revenue.ToString(CultureInfo.InvariantCulture);
                _impressionParams["auctionId"] = impressionData.auctionId;
                _impressionParams["lifetimeRevenue"] = impressionData.lifetimeRevenue.ToString();
                _impressionParams["country"] = impressionData.country;
                _impressionParams["ab"] = impressionData.ab;
                _impressionParams["segmentName"] = impressionData.segmentName;
                _impressionParams["placement"] = impressionData.placement;
                _impressionParams["instanceId"] = impressionData.instanceId;
                _impressionParams["precision"] = impressionData.precision;
                _impressionParams["encryptedCPM"] = impressionData.encryptedCPM;
                _impressionParams["conversionValue"] = impressionData.conversionValue.ToString();

                _appsFlyerImpressionHandler.HandleImpression(_impressionParams, revenue);
                _firebaseImpressionHandler.HandleImpression(_impressionParams, revenue);
            }
        }

        private class ImpressionAchievement
        {
            public ImpressionAchievementType Type { get; set; }
            public float Value { get; set; }
            public bool Achieved { get; set; }
            public int NumberOfAchievement { get; set; }

            public string ValueString
            {
                get => Value.ToString();
            }

            public string ValueWithoutComma()
            {
                return ValueString?.Replace(",", string.Empty).Replace(".", string.Empty);
            }
        }

        private enum ImpressionAchievementType
        {
            TotalRevenue = 0,
            TotalEcpm,
            TotalImpressions
        }

        public void Dispose() => _disposable?.Dispose();
    }
}