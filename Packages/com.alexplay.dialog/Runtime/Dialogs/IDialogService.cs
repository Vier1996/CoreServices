using System;
using ACS.Dialog.Dialogs.Arguments;

namespace ACS.Dialog.Dialogs
{
    public interface IDialogService
    {
        public event Action<int> OnCountChanged;
        public bool HasActiveDialog { get; set; }

        public void CallDialog(Type dialogType);
        public void CallDialog<TArgs>(Type dialogType, TArgs args) where TArgs : DialogArgs;
    }
}