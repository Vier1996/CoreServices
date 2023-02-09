using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Purchasing;

namespace ACS.IAP.InAppPurchase.Starter
{
    public class InAppPurchaseServiceStarter
    {
        private List<global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase> _products;
        
        public InAppPurchaseServiceStarter(IStoreListener storeListener, List<global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase> currentAvailablePurchases)
        {
            _products = currentAvailablePurchases;
            InitializePurchasing(storeListener, currentAvailablePurchases);
        }

        private void InitializePurchasing(IStoreListener storeListener, List<global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase> currentAvailablePurchases)
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            for (int i = 0; i < currentAvailablePurchases.Count; i++)
                builder.AddProduct(currentAvailablePurchases[i].GetIdentifier(), currentAvailablePurchases[i].GetProductType());
            
            UnityPurchasing.Initialize(storeListener, builder);
        }

        public global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase GetProduct(string sku)
        {
            global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase product =
                _products.FirstOrDefault(prd => prd.GetIdentifier() == sku);

            if (product == default)
                throw new ArgumentNullException($"Can not find product with sku:{sku}");

            return product;
        }
    }
}