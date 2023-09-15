using System.Collections.Generic;
using System.Linq;
using ACS.Core.UI;
using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

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
                
                ProjectContext.PreInstall += OnProjectContextPreInstall;
                SceneManager.sceneLoaded += UpdateCanvasesCamera;
                Core.PostInitialized += OnProjectContextPostInstall;
                
                DontDestroyOnLoad(this);
            }
        }

        private void OnProjectContextPreInstall()
        {
            ProjectContext.PreInstall -= OnProjectContextPreInstall;
            
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
            Canvas dialogCanvas = _rectForDialogs.gameObject.AddComponent<Canvas>();

            SetupRenderMode(dialogCanvas, _config._dialogsSettings.RenderMode);

            CanvasScaler dialogCanvasScaler = _rectForDialogs.gameObject.AddComponent<CanvasScaler>();
            dialogCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            dialogCanvasScaler.referenceResolution = new Vector2(_config._dialogsSettings.ReferenceResolutionX, _config._dialogsSettings.ReferenceResolutionY);
            dialogCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            dialogCanvasScaler.matchWidthOrHeight = 1f;
            dialogCanvasScaler.referencePixelsPerUnit = 100;
                
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

            UIResizer dialogUIResizer = _rectForDialogs.gameObject.AddComponent<UIResizer>();
            dialogUIResizer.SetupCanvas();

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
            _core.DialogService.RenderModeChanged -= OnRenderModeChanged;
#endif            
            ProjectContext.PreInstall -= OnProjectContextPreInstall;
            SceneManager.sceneLoaded -= UpdateCanvasesCamera;
            Core.PostInitialized -= OnProjectContextPostInstall;
        }
    }
}
