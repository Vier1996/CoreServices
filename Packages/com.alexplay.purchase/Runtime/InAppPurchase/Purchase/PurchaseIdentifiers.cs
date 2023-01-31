using System;
using UnityEngine;

namespace ACS.IAP.InAppPurchase.Purchase
{
    [Serializable]
    public class PurchaseIdentifiers
    {
        [SerializeField] private string _androidIdentifier;
        [SerializeField] private string _iosIdentifier;
        
        public string Identifier()
        {
#if UNITY_ANDROID
            return _androidIdentifier;
#elif UNITY_IOS
            return _iosIdentifier;
#endif
            throw new ArgumentException("Hey, seem like you try using non defined store, check your target platform in build settings");
        }
    }
}