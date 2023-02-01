using System;
using ACS.GDPR.Config;
using UnityEngine;

namespace ACS.GDPR.Service
{
    public class GdprService : IGdprService
    {
        public event Action Accepted;
        public event Action Declined;
        
        private GdprState GetGdprState
        {
            get => (GdprState)PlayerPrefs.GetInt($"GdprService.{_gdprConfig.GetVersion()}");
            set => PlayerPrefs.SetInt($"GdprService.{_gdprConfig.GetVersion()}", (int)value);
        }

        private GdprView _gdprInstance;
        private GdprConfig _gdprConfig;
        private bool _rememberChoose = true;
        private readonly RectTransform _dialogsParent;
        
        public GdprService(GdprConfig gdprConfig, RectTransform dialogsParent)
        {
            _gdprConfig = gdprConfig;
            _dialogsParent = dialogsParent;
        }

        public void PrepareService() { }

        public void ShowGdpr(bool remember = true)
        {
            _rememberChoose = remember;
            
            RectTransform rect = _dialogsParent.GetComponent<RectTransform>();
            if (rect == null)
                throw new Exception("GDPR parent can contains a RectTransform component, check it.");
            
            GdprState currentState = GetGdprState;

            switch (currentState)
            {
                case GdprState.WAITING_TO_ACCEPT:
                case GdprState.DECLINED:
                    CallGdpr(rect);
                    break;
                case GdprState.ACCEPTED: 
                    Accepted?.Invoke(); 
                    break;
            }
        }

        private void CallGdpr(RectTransform parent)
        {
            _gdprInstance = UnityEngine.Object.Instantiate(Resources.Load<GdprView>("Gdpr/GDPR"), parent);

            _gdprInstance.GDPR.Accepted += OnAccept;
            _gdprInstance.GDPR.Decline += OnDecline;
        }

        private void OnAccept()
        {
            _gdprInstance.GDPR.Accepted -= OnAccept;
            _gdprInstance.CloseGdpr();
            
            Resources.UnloadUnusedAssets();
            
            if(_rememberChoose)
                GetGdprState = GdprState.ACCEPTED;
            
            Accepted?.Invoke();
        }
        
        private void OnDecline()
        {
            _gdprInstance.GDPR.Decline -= OnDecline;
            _gdprInstance.CloseGdpr();
            
            Resources.UnloadUnusedAssets();

            GetGdprState = GdprState.DECLINED;
            Declined?.Invoke();
        }
    }
}