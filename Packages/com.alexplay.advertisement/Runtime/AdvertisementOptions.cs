using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACS.Ads
{
    [Serializable]
    public class AdvertisementOptions
    {
        [Tooltip("In seconds")] public float InterstitialsTimeout;
        [Tooltip("In seconds")] public float FreeInterstitialsAtStart;
        [Tooltip("AdvertisementType")] public List<AdvertisementService.AdvertisementType> AdvertisementTypes;
    }
}