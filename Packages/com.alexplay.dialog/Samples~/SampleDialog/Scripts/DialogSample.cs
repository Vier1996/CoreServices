using ACS.Core;
using ACS.Dialog.Dialogs;
using Alexplay.Samples.Dialogs.Scripts.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace Alexplay.Samples.Dialogs.Scripts
{
    public class DialogSample : MonoBehaviour
    {
        [SerializeField] private Button _callDialogButton;
        
        private IDialogService _dialogService; 
        
        private void Start()
        {
            _dialogService = Core.Instance.DialogService;
            _callDialogButton.onClick.AddListener(CallDialog);
        }

        private void CallDialog()
        {
            //_dialogService.CallDialog(typeof(SampleDialog));
            
            _dialogService.CallDialog(typeof(SampleDialog), new SampleDialogArgs()
            {
                SampleInt = 312,
                SampleDouble = 22d,
                SampleString = "Sample string"
            });
        }
    }
}
