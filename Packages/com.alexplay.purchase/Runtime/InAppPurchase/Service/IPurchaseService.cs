using System;
using UnityEngine.Purchasing;

namespace ACS.IAP.InAppPurchase.Service
{
    public interface IPurchaseService
    {
        public event Action<string> SuccessfulPurchase; 
        public event Action<string, PurchaseFailureReason> FailedPurchase;
        public event Action<bool> Restore;

        public void SendPurchaseRequest(string purchaseID);
        public void SendRestorePurchasesRequest();
        public string GetPurchasePrice(string id);
    }
}