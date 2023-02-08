using System;
using System.Globalization;
using ACS.Dialog.Dialogs.Arguments;
using ACS.Dialog.Dialogs.View;
using TMPro;
using UnityEngine;

namespace Alexplay.Samples.Dialogs.Scripts.Dialog
{
    public class SampleDialog : DialogView
    {
        [SerializeField] private TextMeshProUGUI _textToSampleInt;
        [SerializeField] private TextMeshProUGUI _textToSampleDouble;
        [SerializeField] private TextMeshProUGUI _textToSampleString;

        private SampleDialogArgs _sampleDialogArgs;
        
        public override void Show()
        {
            base.Show();
            
            _sampleDialogArgs = GetArgs<SampleDialogArgs>();
            
            Initialize();
        }

        private void Initialize()
        {
            if (_sampleDialogArgs == null)
            {
                _textToSampleInt.gameObject.SetActive(false);
                _textToSampleDouble.gameObject.SetActive(false);
                _textToSampleString.gameObject.SetActive(false);
                
                return;
            }

            _textToSampleInt.text = _sampleDialogArgs.SampleInt.ToString();
            _textToSampleDouble.text = _sampleDialogArgs.SampleDouble.ToString(CultureInfo.InvariantCulture);
            _textToSampleString.text = _sampleDialogArgs.SampleString;
        }
    }

    [Serializable]
    public class SampleDialogArgs : DialogArgs
    {
        public int SampleInt;
        public double SampleDouble;
        public string SampleString;
    }
}
