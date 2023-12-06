using ACS.Dialog.Dialogs.Arguments;
using UnityEngine;

namespace ACS.Dialog.Dialogs.View
{
    public class Dialog : MonoBehaviour
    {
        public delegate void DialogHiddenHandler(DialogView dialogView);
        public delegate void DialogShownHandler(DialogView dialogView);
        
        protected bool _visible { get; set; }
        protected CanvasGroup _canvasGroup;
        protected DialogArgs _dialogArgs;
        
        public Dialog AddShownHandler(DialogShownHandler dialogShownHandler)
        {
            _onDialogShown += dialogShownHandler;
            return this;
        }
        public Dialog RemoveShownHandler(DialogShownHandler dialogShownHandler)
        {
            _onDialogShown -= dialogShownHandler;
            return this;
        }
        public virtual Dialog AddHiddenHandler(DialogHiddenHandler dialogHiddenHandler)
        {
            _onDialogHidden += dialogHiddenHandler;
            return this;
        }
        public virtual Dialog RemoveHiddenHandler(DialogHiddenHandler dialogHiddenHandler)
        {
            _onDialogHidden -= dialogHiddenHandler;
            return this;
        }
        
        protected void NotifyListeners()
        {
            if (_visible)
            {
                if (_onDialogShown != null)
                {
                    _onDialogShown((DialogView)this);
                    _onDialogShown = null;
                }
            }
            else
            {
                if (_onDialogHidden != null)
                {
                    _onDialogHidden((DialogView)this);
                    _onDialogShown = null;
                }
            }
        }
        
        protected event DialogShownHandler _onDialogShown;
        protected event DialogHiddenHandler _onDialogHidden;
    }
}