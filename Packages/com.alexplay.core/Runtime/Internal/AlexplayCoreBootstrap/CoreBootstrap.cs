using System;
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

                CreateDialogParent();
                
                ProjectContext.PreInstall += OnProjectContextPreInstall;
                SceneManager.sceneLoaded += UpdateCanvasesCamera;
                
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
            dialogCanvas.worldCamera = Camera.main;
            dialogCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            dialogCanvas.sortingLayerName = "Dialog";
            dialogCanvas.sortingOrder = 100;
            
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
            
            _customCanvases.Add(dialogCanvas);
        }
        
        private void UpdateCanvasesCamera(Scene arg0, LoadSceneMode arg1)
        {
            for (int i = 0; i < _customCanvases.Count; i++) 
                _customCanvases[i].worldCamera = Camera.main;
        }

        private void OnDestroy()
        {
            ProjectContext.PreInstall -= OnProjectContextPreInstall;
            SceneManager.sceneLoaded -= UpdateCanvasesCamera;
        }
    }
}
