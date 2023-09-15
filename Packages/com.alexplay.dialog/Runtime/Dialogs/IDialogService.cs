using System;
using ACS.Dialog.Dialogs.Arguments;
using ACS.Dialog.Dialogs.View;
using UnityEngine;

namespace ACS.Dialog.Dialogs
{
    public interface IDialogService
    {
        public event Action<Type> DialogShown;
        public event Action<Type> DialogHide;
        public event Action<int> OnCountChanged;
        public event Action<RenderMode> RenderModeChanged;
        
        public bool HasActiveDialog { get; set; }
        
        public void CallDialog(Type dialogType, SourceContext context = SourceContext.PROJECT_CONTEXT);
        public void CallDialog<TArgs>(Type dialogType, TArgs args, SourceContext context = SourceContext.PROJECT_CONTEXT) where TArgs : DialogArgs;
        public void CloseAllDialogs();
        public bool TryGetDialog<T>(out T dialog) where T : DialogView;
        public void ChangeRenderMode(RenderMode mode);
    }
}