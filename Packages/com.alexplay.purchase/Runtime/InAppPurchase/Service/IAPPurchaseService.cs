using System;
using ACS.IAP.InAppPurchase.Config;
using ACS.IAP.InAppPurchase.Purchases;
using ACS.IAP.InAppPurchase.Starter;
using ACS.IAP.InAppPurchase.Tangles;
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

        private InAppPurchaseServiceStarter _inAppPurchaseServiceStarter;
        private CrossPlatformValidator _validator;
        private PurchaseServiceConfig _serviceConfig;
        private IExtensionProvider _extensionProvider;
        private IStoreController _storeController;
        
        private ACSGooglePlayTangle _googlePlayTangle;
        private ACSAppleTangle _appleTangle;
        
        private bool _restoring = false;
        
        public IAPPurchaseService(PurchaseServiceConfig serviceConfig)
        {
            _validator = null;
            _serviceConfig = serviceConfig;
            _inAppPurchaseServiceStarter = null;

            _googlePlayTangle = new ACSGooglePlayTangle(serviceConfig.GooglePlayPublicKey);
            _appleTangle = new ACSAppleTangle();
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

        public string GetPurchasePrice(string sku)
        {
            string localizedPrice = "";
            if (IAPPurchaseUtils.IsCurrentStoreSupportedByValidator())
            {
#if !UNITY_EDITOR
                localizedPrice = _storeController.products.WithID(sku).metadata.localizedPrice.ToString();
#endif
            }
            else
                localizedPrice = _inAppPurchaseServiceStarter.GetProduct(sku).GetDefaultPrice() + "$";
            
            return localizedPrice;
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
                _validator = new CrossPlatformValidator(_googlePlayTangle.Data(), _appleTangle.Data(), Application.identifier);
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