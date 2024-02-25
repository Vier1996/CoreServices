using System.Collections.Generic;
using Constants;
using UnityEngine;
#if COM_ALEXPLAY_NET_DIALOG
using System.Linq;
using ACS.Core.Extras;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#endif

namespace ACS.Core.Internal.AlexplayCoreBootstrap
{
    [DefaultExecutionOrder(-15000)]
    public class CoreBootstrap : MonoBehaviour
    {
        private readonly List<CachedCustomCanvas> _customCanvases = new();
        private AlexplayCoreKitConfig _config;
        private Core _core;
        
        private RectTransform _rectForDialogs;
        
        private void Awake()
        {
            if (Core.Initialized)
                Destroy(gameObject);
            else
            {
                _config = Resources.Load<AlexplayCoreKitConfig>(ACSConst.ConfigName);

#if COM_ALEXPLAY_NET_DIALOG
                CreateDialogParent();
                
                SceneManager.sceneLoaded += UpdateCanvasesCamera;
#endif         
                Install();
                DontDestroyOnLoad(this);
            }
        }

        private void Install()
        {
            Core.CoreInstallParams installParams = new Core.CoreInstallParams()
            {
                CoreConfig = _config,
                Installer = gameObject,
                DialogRect = _rectForDialogs,
                
                RenderModeChangeDelegate =
#if COM_ALEXPLAY_NET_DIALOG
                    OnRenderModeChanged
#else
                    null
#endif
            };
            
            _core = new Core(installParams);
            
#if COM_ALEXPLAY_NET_DIALOG
            SetupDialogParent();
#endif
        }

#if COM_ALEXPLAY_NET_DIALOG
        private void CreateDialogParent()
        {
            GameObject dialogParentObject = new GameObject("DialogCanvas");
            dialogParentObject.transform.SetParent(transform);
            
            _rectForDialogs = dialogParentObject.AddComponent<RectTransform>();
        }

        private void SetupDialogParent()
        {
            float referenceResolutionLength = _config._dialogsSettings.ResizeOnSceneChanged
                ? Screen.height / (float)Screen.width / _config._dialogsSettings.BaseScreenRatio
                : 1f;

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
            dialogGraphicRaycaster.blockingMask = LayerMask.GetMask(new[]
            {
                "Default",
                "TransparentFX",
                "IgnoreRaycast",
                "Water",
                "UI"
            });

            if (_config._dialogsSettings.ResizeOnSceneChanged)
            {
                CanvasResizer dialogCanvasResizer = _rectForDialogs.gameObject.AddComponent<CanvasResizer>();
                dialogCanvasResizer.SetupResizer(
                    dialogCanvasScaler,
                    _config._dialogsSettings.ReferenceResolutionY,
                    _config._dialogsSettings.ReferenceResolutionX,
                    _config._dialogsSettings.BaseScreenRatio
                );
            }

            _customCanvases.Add(new CachedCustomCanvas()
            {
                Canvas = dialogCanvas,
                CustomCanvasType = CustomCanvasType.DIALOG_CANVAS,
            });
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
                    canvas.worldCamera = FindObjectOfType<Camera>();
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
#endif

        private void OnDestroy()
        {
#if COM_ALEXPLAY_NET_DIALOG
            SceneManager.sceneLoaded -= UpdateCanvasesCamera;
#endif
        }
    }
}
