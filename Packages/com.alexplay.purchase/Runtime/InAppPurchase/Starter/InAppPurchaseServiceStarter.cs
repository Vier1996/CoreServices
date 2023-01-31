using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace ACS.IAP.InAppPurchase.Starter
{
    public class InAppPurchaseServiceStarter
    {
        public InAppPurchaseServiceStarter(IStoreListener storeListener, List<global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase> currentAvailablePurchases) => 
            InitializePurchasing(storeListener, currentAvailablePurchases);

        private void InitializePurchasing(IStoreListener storeListener, List<global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase> currentAvailablePurchases)
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            for (int i = 0; i < currentAvailablePurchases.Count; i++)
                builder.AddProduct(currentAvailablePurchases[i].GetIdentifier(), currentAvailablePurchases[i].GetProductType());
            
            UnityPurchasing.Initialize(storeListener, builder);
        }
    }
}