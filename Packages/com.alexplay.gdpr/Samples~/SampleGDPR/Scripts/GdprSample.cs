using ACS.Core;
using ACS.GDPR.Service;
using UnityEngine;

namespace Alexplay.Samples.GDPR.Scripts
{
    public class GdprSample : MonoBehaviour
    {
        private IGdprService _gdprService;
        
        void Start()
        {
            _gdprService = Core.Instance.GdprService;

            _gdprService.Accepted += OnGDPRAccepted;
            _gdprService.Declined += OnGDPRDeclined;
            
            _gdprService.ShowGdpr(remember: false);
        }

        private void OnGDPRAccepted() => Debug.Log("GDPR: Accepted");

        private void OnGDPRDeclined() => Debug.Log("GDPR: Declined");
    }
}
