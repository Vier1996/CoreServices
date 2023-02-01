using System;
using System.Collections.Generic;
using Config;
using UnityEditor;

namespace ACS.IAP.InAppPurchase.Config
{
    [Serializable]
    public class PurchaseServiceConfig : ServiceConfigBase
    {
        public List<global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase> GetActualPurchases()
        {
            List<global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase> purchases = new List<global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase>();
            string[] guids = AssetDatabase.FindAssets("t:InAppPurchase");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase purchaseInstance = (global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase) AssetDatabase.LoadAssetAtPath(path, typeof(global::ACS.IAP.InAppPurchase.Purchase.InAppPurchase));
                
                purchases.Add(purchaseInstance);
            }

            return purchases;
        }
    }
}