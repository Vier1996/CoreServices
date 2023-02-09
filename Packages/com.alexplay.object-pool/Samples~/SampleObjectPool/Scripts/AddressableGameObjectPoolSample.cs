using ACS.Core;
using ACS.ObjectPool.ObjectPool.Addressable;
using UnityEngine;
using UnityEngine.UI;

namespace Alexplay.Samples.GameObjectPool.Scripts
{
    public class AddressableGameObjectPoolSample : MonoBehaviour
    {
        [SerializeField] private Button _spawnButton;
        [SerializeField] private Button _returnFirst;
        [SerializeField] private Button _returnLast;

        [SerializeField] private SquareReference _squareReference;
        [SerializeField] private Transform _squareParent;

        private IAddressableObjectsPool _objectPool;
        
        private void Start()
        {
            _objectPool = Core.Instance.ObjectPoolService.AddressableObjectPool;
            
            _spawnButton.onClick.AddListener(SpawnSquare);
            _returnFirst.onClick.AddListener(ReturnFirst);
            _returnLast.onClick.AddListener(ReturnLast);
        }

        private async void SpawnSquare()
        {
            Square targetSquare = await _objectPool.Get(_squareReference);
            
            targetSquare.transform.SetParent(_squareParent);
            targetSquare.transform.localPosition = Vector3.zero;
        }
        
        private void ReturnFirst() => _objectPool.ReturnFirst(_squareReference);
        private void ReturnLast() => _objectPool.ReturnLast(_squareReference);

        private void OnDisable()
        {
            _objectPool.Remove(_squareReference);
        }
    }
}
