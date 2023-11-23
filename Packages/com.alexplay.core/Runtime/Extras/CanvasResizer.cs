using UnityEngine;
using UnityEngine.UI;

namespace ACS.Core.Extras
{
    public class CanvasResizer : MonoBehaviour
    {
        public void SetupCanvas(CanvasScaler scaler, float baseCanvasHeight, float baseCanvasWidth, float baseScreenRatio)
        {
            scaler.referenceResolution = new Vector2(
                baseCanvasWidth,
                baseCanvasHeight * Mathf.Max(1f, Screen.height / (float) Screen.width / baseScreenRatio));

            scaler.transform.localScale = Vector3.one * 0.01f;
        }
    }
}