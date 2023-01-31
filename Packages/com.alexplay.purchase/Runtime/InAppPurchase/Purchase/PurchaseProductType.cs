using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace ACS.IAP.InAppPurchase.Purchase
{
    [Serializable]
    public class PurchaseProductType
    {
        [SerializeField] private ProductType _productType;

        public ProductType ProductType() => _productType;
    }
}