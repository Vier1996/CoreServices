using System.Collections.Generic;
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
        
        private List<Canvas> _customCanvases = new List<Canvas>();
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

            SetupRenderMode(dialogCanvas);

            CanvasScaler dialogCanvasScaler = _rectForDialogs.gameObject.AddComponent<CanvasScaler>();
            dialogCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            dialogCanvasScaler.referenceResolution = new Vector2(720, 1280);
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
            
            _customCanvases.Add(dialogCanvas);
        }

        private void SetupRenderMode(Canvas canvas)
        {
            switch (_config._dialogsSettings.RenderMode)
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
                SetupRenderMode(_customCanvases[i]);
        }

        private void OnDestroy()
        {
            ProjectContext.PreInstall -= OnProjectContextPreInstall;
            SceneManager.sceneLoaded -= UpdateCanvasesCamera;
            Core.PostInitialized -= OnProjectContextPostInstall;
        }
    }
}
