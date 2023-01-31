using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace ACS.IAP.InAppPurchase.Purchase
{
    [CreateAssetMenu(fileName = "InAppPurchase", menuName = "Core/Purchase/InAppPurchase")]
    [Serializable]
    public class InAppPurchase : ScriptableObject
    {
        [SerializeField] private PurchaseIdentifiers _purchaseIdentifiers;
        [SerializeField] private PurchaseProductType _productType;
        [SerializeField] private PurchaseDescription _purchaseDescription;

        public string GetIdentifier() => _purchaseIdentifiers.Identifier();
        public ProductType GetProductType() => _productType.ProductType();
        public string GetNameKey() => _purchaseDescription.NameKey();
        public string GetDescriptionKey() => _purchaseDescription.DescriptionKey();
        public double GetDefaultPrice() => _purchaseDescription.PriceByDefault();
    }
}