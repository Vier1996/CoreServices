using Constants;
using UnityEngine;
using Zenject;

namespace ACS.Core.Internal.AlexplayCoreBootstrap
{
    [DefaultExecutionOrder(-15000)]
    public class CoreBootstrap : MonoBehaviour
    {
        private static CoreBootstrap Instance;

        [SerializeField] private RectTransform _dialogRect;
        
        private AlexplayCoreKitConfig _config = null;
        private Core _core;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                
                ProjectContext.PreInstall += OnProjectContextPreInstall;
                
                DontDestroyOnLoad(this);
            }
        }
        
        private void OnProjectContextPreInstall()
        {
            ProjectContext.PreInstall -= OnProjectContextPreInstall;

            _config = Resources.Load<AlexplayCoreKitConfig>(ACSConst.AssetMenuRootName);
            _core = new Core(_config, _dialogRect, gameObject);
        }
    }
}
