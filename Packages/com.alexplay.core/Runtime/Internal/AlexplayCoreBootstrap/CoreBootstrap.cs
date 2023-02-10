using System;
using Constants;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace ACS.Core.Internal.AlexplayCoreBootstrap
{
    [DefaultExecutionOrder(-15000)]
    public class CoreBootstrap : MonoBehaviour
    {
        private static CoreBootstrap Instance;

        [SerializeField] private RectTransform _dialogRect;
        
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

            AlexplayCoreKitConfig config = null;
            
            if ((config = GetConfig()) == null)
                throw new ArgumentException("Seems like you haven't any configuration file, " +
                                            "please restart or recompile UnityEditor");
            
            _core = new Core(config, _dialogRect, gameObject);
        }
        
        public static AlexplayCoreKitConfig GetConfig()
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<AlexplayCoreKitConfig>(ACSConst.SourcePath);
#endif
        }
    }
}
