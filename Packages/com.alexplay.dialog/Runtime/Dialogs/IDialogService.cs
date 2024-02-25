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
        
        public bool HasActiveDialog { get; set; }
        
        public void CallDialog(Type dialogType);
        public void CallDialog<TArgs>(Type dialogType, TArgs args) where TArgs : DialogArgs;
        public void CloseAllDialogs();
        public DialogView GetDialog(Type dialogType);
        public bool TryGetDialog<T>(out T dialog) where T : DialogView;
        public void ChangeRenderMode(RenderMode mode);
    }
}