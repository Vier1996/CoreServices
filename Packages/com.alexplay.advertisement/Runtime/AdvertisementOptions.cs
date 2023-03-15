using System;
using UnityEngine;

namespace ACS.Ads
{
    [Serializable]
    public class AdvertisementOptions
    {
        [Tooltip("In seconds")] public float InterstitialsTimeout;
        [Tooltip("In seconds")] public float FreeInterstitialsAtStart;
    }
}