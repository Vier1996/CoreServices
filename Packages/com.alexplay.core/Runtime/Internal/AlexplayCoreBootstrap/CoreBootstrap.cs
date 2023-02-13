using UnityEngine;
using Zenject;

namespace ACS.Core.Internal.AlexplayCoreBootstrap
{
    [DefaultExecutionOrder(-15000)]
    public class CoreBootstrap : MonoBehaviour
    {
        public bool Configured => _config != null;
        
        private static CoreBootstrap Instance;

        [SerializeField] private RectTransform _dialogRect;
        [SerializeField] private AlexplayCoreKitConfig _config = null;

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
        
        public void ResolveConfig(AlexplayCoreKitConfig config) => _config = config;

        private void OnProjectContextPreInstall()
        {
            ProjectContext.PreInstall -= OnProjectContextPreInstall;

            _core = new Core(_config, _dialogRect, gameObject);
        }
    }
}
