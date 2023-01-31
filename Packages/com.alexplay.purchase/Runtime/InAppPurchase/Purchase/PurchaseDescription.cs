using System;
using UnityEngine;

namespace ACS.IAP.InAppPurchase.Purchase
{
    [Serializable]
    public class PurchaseDescription
    {
        [SerializeField] private string _nameKey;
        [SerializeField] private  string _descriptionKey;
        [Range(0.99f, 129.99f)] [SerializeField] private double _priceByDefault = 0.99f;
        
        public string NameKey() => _nameKey;
        public string DescriptionKey() => _descriptionKey;
        public double PriceByDefault() => _priceByDefault;
    }
}