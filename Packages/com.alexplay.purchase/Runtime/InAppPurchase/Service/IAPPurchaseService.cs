using System;
using ACS.IAP.InAppPurchase.Config;
using ACS.IAP.InAppPurchase.Purchases;
using ACS.IAP.InAppPurchase.Starter;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace ACS.IAP.InAppPurchase.Service
{
    public class IAPPurchaseService : IPurchaseService, IStoreListener
    {
        public event Action<string> SuccessfulPurchase; 
        public event Action<string, PurchaseFailureReason> FailedPurchase;
        public event Action<bool> Restore;

        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;
        private PurchaseServiceConfig _serviceConfig;
        private InAppPurchaseServiceStarter _inAppPurchaseServiceStarter;
        
        private readonly CrossPlatformValidator _validator;
        private bool _restoring = false;
        
        public IAPPurchaseService(PurchaseServiceConfig serviceConfig)
        {
            _validator = null;
            _serviceConfig = serviceConfig;
            _inAppPurchaseServiceStarter = null;
        }
        
        public void PrepareService()
        {
            _inAppPurchaseServiceStarter = new InAppPurchaseServiceStarter(this, _serviceConfig.GetActualPurchases());
            InitializeValidator();
        }

        public void SendPurchaseRequest(string purchaseID) => _storeController.InitiatePurchase(purchaseID);
        
        public void SendRestorePurchasesRequest()
        {
            _restoring = true;
            
#if UNITY_EDITOR
            OnRestoreComplete(true);
            return;
#endif

            switch (Application.platform)
            {
                case RuntimePlatform.Android: 
                    _extensionProvider.GetExtension<IGooglePlayStoreExtensions>().RestoreTransactions(OnRestoreComplete);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    _extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(OnRestoreComplete);
                    break;
            }
        }
        
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;
            
            Debug.Log("Purchase manager successfully initialized");
        }

        public void OnInitializeFailed(InitializationFailureReason error) => Debug.Log($"Purchase manager initialize failed: {error}");

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Product product = purchaseEvent.purchasedProduct;
            bool isPurchaseValid = IsPurchaseValid(product);

            if (isPurchaseValid)
            {
                OnPurchaseSuccess(product);
                Debug.Log("Valid receipt, unlocking content.");
            }
            else
            {
                Debug.Log("Invalid receipt, not unlocking content.");
            }

            return PurchaseProcessingResult.Complete;
        }
        
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) => 
            FailedPurchase?.Invoke(product.definition.id, failureReason);

        private void OnPurchaseSuccess(Product product) => 
            SuccessfulPurchase?.Invoke(product.definition.id);
        
        private void OnRestoreComplete(bool success)
        {
            var restoreMessage = "";

            if (success) restoreMessage = "Restore Successful";
            else restoreMessage = "Restore Failed";
            
            Debug.Log(restoreMessage);
            
            Restore?.Invoke(success);
            _restoring = false;
        }

        private void InitializeValidator()
        {
            if (IAPPurchaseUtils.IsCurrentStoreSupportedByValidator())
            {
#if !UNITY_EDITOR
                _validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
#endif
            }
            else
                Debug.Log(
                    $"PurchaseService: can't initialize validator! Reason: invalid store ({StandardPurchasingModule.Instance().appStore})");
        }
        
        private bool IsPurchaseValid(Product product)
        {
            if (IAPPurchaseUtils.IsCurrentStoreSupportedByValidator())
            {
                try
                {
                    IPurchaseReceipt[] result = _validator.Validate(product.receipt);
                    IAPPurchaseUtils.LogReceipt(result);
                }
                catch (IAPSecurityException reason)
                {
                    Debug.Log($"Invalid receipt: {reason}");
                    return false;
                }
            }

            return true;
        }
    }
}