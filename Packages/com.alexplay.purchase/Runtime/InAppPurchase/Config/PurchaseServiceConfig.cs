using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ACS.IAP.InAppPurchase.Config
{
    [Serializable]
    public class PurchaseServiceConfig : ServiceConfigBase
    {
        [ShowIf("@IsEnabled == true")]
        public string GooglePlayPublicKey = "Сюда вставить ключ с GooglePlay консоли";
        
        [ShowIf("@IsEnabled == true")]
        [BoxGroup("Purchases")]
        public List<InAppPurchaseInfo> ActualInAppPurchases = new List<InAppPurchaseInfo>();
        
        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.purchase";
        
        [Button] private void DownloadActualPurchases()
        {
            List<Purchase.InAppPurchase> purchases = GetActualPurchases();

            if (purchases != null && purchases.Count > 0)
            {
                for (int i = 0; i < purchases.Count; i++)
                {
                    bool contains =
                        ActualInAppPurchases.FirstOrDefault(purchase => purchase.Purchase.GetHashCode() == purchases[i].GetHashCode()) != default;

                    if (!contains) 
                        ActualInAppPurchases.Add(new InAppPurchaseInfo(purchases[i]));
                }
            }
        }

        [Button] private void ReloadAll()
        {
            ActualInAppPurchases.Clear();
            DownloadActualPurchases();
        }
        
        private List<Purchase.InAppPurchase> GetActualPurchases()
        {
            List<Purchase.InAppPurchase> purchases = new List<Purchase.InAppPurchase>();

#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets("t:InAppPurchase");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Purchase.InAppPurchase purchaseInstance = (Purchase.InAppPurchase) AssetDatabase.LoadAssetAtPath(path, typeof(Purchase.InAppPurchase));
                
                purchases.Add(purchaseInstance);
            }
#endif

            return purchases;
        }
        
        public void Validate()
        {
            for (int i = 0; i < ActualInAppPurchases.Count; i++)
                ActualInAppPurchases[i].Identifier = ActualInAppPurchases[i].Purchase.GetIdentifier();
        }

        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }

    [Serializable]
    public class InAppPurchaseInfo
    {
        [ReadOnly] public string Identifier;
        [ReadOnly] public Purchase.InAppPurchase Purchase;
        public bool AvailableToExecuting;

        public InAppPurchaseInfo(Purchase.InAppPurchase purchase)
        {
            Purchase = purchase;
            Identifier = Purchase.GetIdentifier();
            AvailableToExecuting = true;
        }
    }
}