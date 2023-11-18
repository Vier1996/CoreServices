using System.Collections.Generic;
using System.Linq;
using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if COM_ALEXPLAY_ZENJECT_EXTENSION
using Zenject;
#endif

namespace ACS.Core.Internal.AlexplayCoreBootstrap
{
    [DefaultExecutionOrder(-15000)]
    public class CoreBootstrap : MonoBehaviour
    {
        private static CoreBootstrap Instance;

        [SerializeField] private bool _loadFromInitialScene;
        
        private readonly List<CachedCustomCanvas> _customCanvases = new List<CachedCustomCanvas>();
        private RectTransform _rectForDialogs;
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

                _config = Resources.Load<AlexplayCoreKitConfig>(ACSConst.ConfigName);

                CreateDialogParent();
                
                SceneManager.sceneLoaded += UpdateCanvasesCamera;
                Core.PostInitialized += OnProjectContextPostInstall;
                
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                ProjectContext.PreInstall += OnInstalled;
#else
                OnInstalled();
#endif
                
                DontDestroyOnLoad(this);
            }
        }

        private void OnInstalled()
        {
#if COM_ALEXPLAY_ZENJECT_EXTENSION
            ProjectContext.PreInstall -= OnProjectContextPreInstall;
#endif
            
            _core = new Core(_config, gameObject, _rectForDialogs);
            
            SetupDialogParent();
        }

        private void OnProjectContextPostInstall()
        {
            if (_loadFromInitialScene)
            {
                if (SceneManager.GetActiveScene().buildIndex != 0)
                    SceneManager.LoadScene(0);
            }
        }

        private void CreateDialogParent()
        {
            GameObject dialogParentObject = new GameObject("DialogCanvas");
            dialogParentObject.transform.SetParent(transform);
            
            _rectForDialogs = dialogParentObject.AddComponent<RectTransform>();
        }

        private void SetupDialogParent()
        {
            float referenceResolutionLength = Screen.height / (float)Screen.width / _config._dialogsSettings.BaseScreenRatio;
            Canvas dialogCanvas = _rectForDialogs.gameObject.AddComponent<Canvas>();

            SetupRenderMode(dialogCanvas, _config._dialogsSettings.RenderMode);
            
            CanvasScaler dialogCanvasScaler = _rectForDialogs.gameObject.AddComponent<CanvasScaler>();
            dialogCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            dialogCanvasScaler.referenceResolution = new Vector2(
                _config._dialogsSettings.ReferenceResolutionX, 
                _config._dialogsSettings.ReferenceResolutionY * Mathf.Max(1f, referenceResolutionLength));
            dialogCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            dialogCanvasScaler.matchWidthOrHeight = 1f;
            dialogCanvasScaler.referencePixelsPerUnit = 100;
            
            dialogCanvas.transform.localScale *= 0.01f;
                
            GraphicRaycaster dialogGraphicRaycaster = _rectForDialogs.gameObject.AddComponent<GraphicRaycaster>();
            dialogGraphicRaycaster.ignoreReversedGraphics = true;
            dialogGraphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            dialogGraphicRaycaster.blockingMask = LayerMask.GetMask(new []
            {
                "Default",
                "TransparentFX",
                "IgnoreRaycast",
                "Water",
                "UI"
            });
            
            _customCanvases.Add(new CachedCustomCanvas()
            {
                Canvas = dialogCanvas,
                CustomCanvasType = CustomCanvasType.DIALOG_CANVAS,
            });
            
#if COM_ALEXPLAY_NET_DIALOG
            _core.DialogService.RenderModeChanged += OnRenderModeChanged;
#endif
        }

        private void OnRenderModeChanged(RenderMode renderMode)
        {
            CachedCustomCanvas cachedCanvas = _customCanvases.FirstOrDefault(canvas => canvas.CustomCanvasType == CustomCanvasType.DIALOG_CANVAS);
            
            if(cachedCanvas != default)
                SetupRenderMode(cachedCanvas.Canvas, renderMode);
        }

        private void SetupRenderMode(Canvas canvas, RenderMode renderMode)
        {
            switch (renderMode)
            {
                case RenderMode.WorldSpace:
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.worldCamera = Camera.main;
                    canvas.sortingLayerName = _config._dialogsSettings.GetLayerName();
                    canvas.sortingOrder = _config._dialogsSettings.DialogSortingOrder;
                    break;
                case RenderMode.ScreenSpaceCamera:
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = Camera.main;
                    canvas.sortingLayerName = _config._dialogsSettings.GetLayerName();
                    canvas.sortingOrder = _config._dialogsSettings.DialogSortingOrder;
                    break;
                case RenderMode.ScreenSpaceOverlay:
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.sortingOrder = _config._dialogsSettings.DialogSortingOrder;
                    break;
            }
        }
        
        private void UpdateCanvasesCamera(Scene arg0, LoadSceneMode arg1)
        {
            for (int i = 0; i < _customCanvases.Count; i++) 
                SetupRenderMode(_customCanvases[i].Canvas, _config._dialogsSettings.RenderMode);
        }

        private void OnDestroy()
        {
#if COM_ALEXPLAY_NET_DIALOG
            if(_core != null)
                _core.DialogService.RenderModeChanged -= OnRenderModeChanged;
#endif            
            
#if COM_ALEXPLAY_ZENJECT_EXTENSION
            ProjectContext.PreInstall -= OnInstalled;
#endif
            SceneManager.sceneLoaded -= UpdateCanvasesCamera;
            Core.PostInitialized -= OnProjectContextPostInstall;
        }
    }
}
