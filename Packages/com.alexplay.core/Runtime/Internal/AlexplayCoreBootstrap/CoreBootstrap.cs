using UnityEngine;
using Zenject;

namespace ACS.Core.Internal.AlexplayCoreBootstrap
{
    [DefaultExecutionOrder(-15000)]
    public class CoreBootstrap : MonoBehaviour
    {
        private static CoreBootstrap Instance;

        [SerializeField] private RectTransform _dialogRect;
        [SerializeField] private AlexplayCoreKitConfig _config;
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

            _core = new Core(_config, _dialogRect, gameObject);
        }
    }
}
