using ACS.Core;
using ACS.IAP.InAppPurchase.Purchase;
using ACS.IAP.InAppPurchase.Service;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Alexplay.Samples.Purchase.Scripts
{
    public class PurchaseSample : MonoBehaviour
    {
        [SerializeField] private InAppPurchase _purchase;
        
        [SerializeField] private Button _sendPurchaseRequest;
        [SerializeField] private Button _sendRestorePurchaseRequest;
        [SerializeField] private TextMeshProUGUI _info;
        
        private IPurchaseService _purchaseService;

        void Start()
        {
            _purchaseService = Core.Instance.PurchaseService;
            
            _purchaseService.SuccessfulPurchase += OnSuccessfulPurchase;
            _purchaseService.FailedPurchase += OnFailedPurchase;
            _purchaseService.Restore += OnRestore;
            
            _sendPurchaseRequest.onClick.AddListener(() =>
            {
                _purchaseService.SendPurchaseRequest(_purchase.GetIdentifier());
            });
            
            _sendRestorePurchaseRequest.onClick.AddListener(() =>
            {
                _purchaseService.SendRestorePurchasesRequest();
            });
        }

        private void OnRestore(bool obj)
        {
            _info.text = $"Successful restore with status:{obj}";
        }

        private void OnSuccessfulPurchase(string obj)
        {
            _info.text = $"Successful purchase with type:{obj}";
        }
        
        private void OnFailedPurchase(string arg1, PurchaseFailureReason arg2)
        {
            _info.text = $"Failed purchase with type:{arg1}, by reason:{arg2}";
        }

        private void OnDisable()
        {
            _purchaseService.SuccessfulPurchase -= OnSuccessfulPurchase;
            _purchaseService.FailedPurchase -= OnFailedPurchase;
            _purchaseService.Restore -= OnRestore;
        }
    }
}
