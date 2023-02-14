using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ACS.Core.UI
{
    public class UIResizer : MonoBehaviour
    {
        private const float BaseCanvasHeight = 1280f;
        private const float BaseCanvasWidth = 720f;
        private const float BaseScreenRatio = 16 / 9f;

        private void Awake() => SetupCanvas();

        private void SetupCanvas()
        {
            GetComponent<CanvasScaler>().referenceResolution = new Vector2(
                BaseCanvasWidth,
                BaseCanvasHeight * Mathf.Max(1f, Screen.height / (float) Screen.width / BaseScreenRatio));

            transform.DOScale(0.01f, 0);
        }
    }
}
