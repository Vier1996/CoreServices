using ACS.Core.UI;
using Constants;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ACS.Core.Internal.AlexplayCoreBootstrap
{
    [DefaultExecutionOrder(-15000)]
    public class CoreBootstrap : MonoBehaviour
    {
        private static CoreBootstrap Instance;

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

                CreateDialogParent();
                ProjectContext.PreInstall += OnProjectContextPreInstall;
                
                DontDestroyOnLoad(this);
            }
        }
        
        private void OnProjectContextPreInstall()
        {
            ProjectContext.PreInstall -= OnProjectContextPreInstall;

            _config = Resources.Load<AlexplayCoreKitConfig>(ACSConst.ConfigName);
            _core = new Core(_config, gameObject, _rectForDialogs);
        }

        private void CreateDialogParent()
        {
            GameObject dialogParentObject = new GameObject("DialogCanvas");
            dialogParentObject.transform.SetParent(transform);

            _rectForDialogs = dialogParentObject.AddComponent<RectTransform>();
            
            Canvas dialogCanvas = dialogParentObject.AddComponent<Canvas>();
            dialogCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            
            CanvasScaler dialogCanvasScaler = dialogParentObject.AddComponent<CanvasScaler>();
            dialogCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            dialogCanvasScaler.referenceResolution = new Vector2(720, 1280);
            dialogCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            dialogCanvasScaler.matchWidthOrHeight = 1f;
            dialogCanvasScaler.referencePixelsPerUnit = 100;
                
            GraphicRaycaster dialogGraphicRaycaster = dialogParentObject.AddComponent<GraphicRaycaster>();
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

            UIResizer dialogUIResizer = dialogParentObject.AddComponent<UIResizer>();
            dialogUIResizer.SetupCanvas();
        }
    }
}
