using System;
using ACS.Dialog.Dialogs.Arguments;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ACS.Dialog.Dialogs.View
{
    public abstract class DialogView : Dialog, IReceiveArgs<DialogArgs>
    {
        public event Action ShowingAnimationFinished;
        
        [BoxGroup("Parameters"), SerializeField] private bool _ignoreAnimation = false;
        [BoxGroup("Parameters"), SerializeField] private bool _customDialogSize = false;
        [BoxGroup("Parameters"), ShowIf(nameof(_customDialogSize), true), SerializeField] private Vector3 _customPosition;
        [BoxGroup("Parameters"), ShowIf(nameof(_customDialogSize), true), SerializeField] private Vector3 _customScale;
        [BoxGroup("Close"), SerializeField] private Button _closeButton;
        
        protected virtual void Awake()
        {
            transform.DOLocalMove(_customDialogSize ? _customPosition : Vector3.zero, 0);
            
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            
            if(_closeButton != null)
                _closeButton.onClick.AddListener(Hide);
        }
        
        public DialogView SetArgs(DialogArgs args)
        { 
            _dialogArgs = args;

            return this;
        }

        public void SetParent(RectTransform parentTransform)
        {
            RectTransform dialogRect = GetComponent<RectTransform>();
                
            dialogRect.SetParent(parentTransform);
            dialogRect.position = Vector3.zero;
            dialogRect.localScale = _customDialogSize ? _customScale : Vector3.one;
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
                    }, 1f, 0.3f).OnComplete(() => ShowingAnimationFinished?.Invoke());
                }

                NotifyListeners();
            }   
        }

        public virtual void Hide()
        {
            _visible = false;
            
            DOTween.To(() => _canvasGroup.alpha, alpha =>
            {
                _canvasGroup.alpha = alpha;
            }, 0f, 0.3f).OnComplete(NotifyListeners);
        }

        public void TemporaryEnable() => gameObject.SetActive(true);

        public void TemporaryDisable() => gameObject.SetActive(false);
        
        public void ShowCloseButton() => _closeButton.gameObject.SetActive(true);
        
        public void HideCloseButton() => _closeButton.gameObject.SetActive(false);

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
    
    [Serializable]
    public class DialogReference : ComponentReference<DialogView>
    {
        public DialogReference(string guid) : base(guid) { }
    }
}
