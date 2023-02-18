using System;
using System.Collections.Generic;
using System.Linq;
using ACS.IAP.InAppPurchase.Config;
using UnityEngine.Purchasing;

namespace ACS.IAP.InAppPurchase.Starter
{
    public class InAppPurchaseServiceStarter
    {
        private readonly List<InAppPurchaseInfo> _products;
        
        public InAppPurchaseServiceStarter(IStoreListener storeListener, List<InAppPurchaseInfo> currentAvailablePurchases)
        {
            _products = currentAvailablePurchases;
            InitializePurchasing(storeListener, currentAvailablePurchases);
        }

        private void InitializePurchasing(IStoreListener storeListener, List<InAppPurchaseInfo> currentAvailablePurchases)
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            for (int i = 0; i < currentAvailablePurchases.Count; i++)
                builder.AddProduct(currentAvailablePurchases[i].Purchase.GetIdentifier(), currentAvailablePurchases[i].Purchase.GetProductType());
            
            UnityPurchasing.Initialize(storeListener, builder);
        }

        public Purchase.InAppPurchase GetProduct(string sku)
        {
            InAppPurchaseInfo productInfo = _products.FirstOrDefault(prd => prd.Purchase.GetIdentifier() == sku);

            if (productInfo == default)
                throw new ArgumentNullException($"Can not find product with sku:{sku}");

            return productInfo.Purchase;
        }
    }
}