using ACS.Data.DataService.Service;
using Alexplay.Samples.Data.Scripts.Sample;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Alexplay.Samples.Data.Scripts
{
    public class DataSample : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI sampleText_1;
        [SerializeField] private TextMeshProUGUI sampleText_2;
        [SerializeField] private TextMeshProUGUI sampleText_3;

        [SerializeField] private Button sampleButton_1;
        [SerializeField] private Button sampleButton_2;
        [SerializeField] private Button sampleButton_3;

        private IDataService _dataService;

        private SampleDataModel _sampleDataModel;
        private SampleDataModelSecond _sampleDataModelSecond;
        private SampleDataModelThird _sampleDataModelThird;
        
        [Inject] private void InjectDependencies(IDataService dataService)
        { 
            _dataService = dataService;
            
            SetupSample_1();
            SetupSample_2();
            SetupSample_3();
        }

        private void SetupSample_1()
        {
            _sampleDataModel = _dataService.Models.Resolve<SampleDataModel>();
            
            sampleButton_1.onClick.AddListener(() =>
            {
                _sampleDataModel.ChangeData();
                UpdateSample_1();
            });
            
            UpdateSample_1();
        }
        
        private void SetupSample_2()
        {
            _sampleDataModelSecond = _dataService.Models.Resolve<SampleDataModelSecond>();
            
            sampleButton_2.onClick.AddListener(() =>
            {
                _sampleDataModelSecond.ChangeData();
                UpdateSample_2();
            });
            
            UpdateSample_2();
        }
        
        private void SetupSample_3()
        {
            _sampleDataModelThird = _dataService.Models.Resolve<SampleDataModelThird>();
            
            sampleButton_3.onClick.AddListener(() =>
            {
                _sampleDataModelThird.ChangeData();
                UpdateSample_3();
            });
            
            UpdateSample_3();
        }
        
        private void UpdateSample_1()
        {
            sampleText_1.text = _sampleDataModel.GetData().ToString();
        }
        
        private void UpdateSample_2()
        {
            sampleText_2.text = _sampleDataModelSecond.GetData().ToString();
        }
        
        private void UpdateSample_3()
        {
            sampleText_3.text = _sampleDataModelThird.GetData().ToString();
        }
    }
}
