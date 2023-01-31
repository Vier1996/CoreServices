using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace ACS.IAP.InAppPurchase.Purchases
{
    public static class IAPPurchaseUtils
    {
        private static readonly List<AppStore> SupportedStores;
        
        private static readonly ConfigurationBuilder ConfigurationBuilder;
        private static readonly IAppleConfiguration IappleConfiguration;
        
        static IAPPurchaseUtils()
        {
            SupportedStores = new List<AppStore>()
            {
                AppStore.GooglePlay,
                AppStore.AppleAppStore,
                AppStore.MacAppStore,
            };
            
            ConfigurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
#if UNITY_IOS
            IappleConfiguration = ConfigurationBuilder.Configure<IAppleConfiguration>();
#endif
        }
        
        public static bool CanMakePayment() => 
            Application.platform == RuntimePlatform.Android || IappleConfiguration.canMakePayments;

        public static bool IsCurrentStoreSupportedByValidator()
        {
            AppStore currentAppStore = StandardPurchasingModule.Instance().appStore;
            return SupportedStores.Contains(currentAppStore);
        }
        
        public static void LogReceipt(IEnumerable<IPurchaseReceipt> receipts)
        {
            Debug.Log("Receipt is valid. Contents:");
            
            foreach (var receipt in receipts) 
                LogReceipt(receipt);
        }
        
        private static void LogReceipt(IPurchaseReceipt receipt) =>
            Debug.Log($"Product ID: {receipt.productID}\n" +
                      $"Purchase Date: {receipt.purchaseDate}\n" +
                      $"Transaction ID: {receipt.transactionID}");
    }
}