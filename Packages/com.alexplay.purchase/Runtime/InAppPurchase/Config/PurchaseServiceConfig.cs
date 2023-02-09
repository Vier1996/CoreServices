using System;
using System.Collections.Generic;
using Config;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ACS.IAP.InAppPurchase.Config
{
    [Serializable]
    public class PurchaseServiceConfig : ServiceConfigBase
    {
        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.purchase";
        
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
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}