﻿using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ACS.Core.Extras
{
    public class CanvasResizer : MonoBehaviour
    {
        private CanvasScaler _scaler;
        private float _baseCanvasHeight;
        private float _baseCanvasWidth;
        private float _baseScreenRatio;

        public void SetupResizer(CanvasScaler scaler, float baseCanvasHeight, float baseCanvasWidth, float baseScreenRatio)
        {
            _scaler = scaler;
            _baseCanvasHeight = baseCanvasHeight;
            _baseCanvasWidth = baseCanvasWidth;
            _baseScreenRatio = baseScreenRatio;
            
            SceneManager.activeSceneChanged += ChangeSize;
        }

        private void ChangeSize(Scene arg0, Scene arg1)
        {
            Resize();
        }

        private void Resize()
        {
            if(_scaler == null)
                return;
            
            _scaler.referenceResolution = new Vector2(
                _baseCanvasWidth,
                _baseCanvasHeight * Mathf.Max(1f, Screen.height / (float)Screen.width / _baseScreenRatio));

            _scaler.transform.DOScale(0.01f, 0f);
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= ChangeSize;
        }
    }
}