using ACS.Dialog.Dialogs.Arguments;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ACS.Dialog.Dialogs.View
{
    public class DialogView : Dialog, IReceiveArgs<DialogArgs>
    {
        [SerializeField] private bool _ignoreAnimation = false;
        [SerializeField] private Button _closeButton;
        
        private void Awake()
        {
            transform.DOLocalMove(Vector3.zero, 0);
            
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            
            _closeButton.onClick.AddListener(Hide);
        }
        
        public void SetArgs(DialogArgs args) => _dialogArgs = args;

        public void SetParent(RectTransform parentTransform)
        {
            RectTransform dialogRect = GetComponent<RectTransform>();
                
            dialogRect.SetParent(parentTransform);
            dialogRect.position = Vector3.zero;
            dialogRect.localScale = Vector3.one;
            dialogRect.rotation = new Quaternion(0, 0, 0, 0);
        }
        
        public virtual void Show()
        {
            if (!_visible)
            {
                gameObject.SetActive(_visible = true);
                
                if(_ignoreAnimation)
                    _canvasGroup.alpha = 1f;
                else
                {
                    DOTween.To(() => _canvasGroup.alpha, alpha =>
                    {
                        _canvasGroup.alpha = alpha;
                    }, 1f, 0.3f);
                }

                NotifyListeners();
            }   
        }

        protected virtual void Hide()
        {
            _visible = false;

            NotifyListeners();

            gameObject.SetActive(false);
            
            Resources.UnloadUnusedAssets();

            if (gameObject != null) 
                Destroy(gameObject);
        }
        
        protected TArgs GetArgs<TArgs>() where TArgs : DialogArgs
        {
            if (_dialogArgs == null)
            {
                Debug.LogError($"Trying to receive {typeof(TArgs)} - NULL args");
                return null;
            }
            
            return (TArgs) _dialogArgs;
        }
    }
}
