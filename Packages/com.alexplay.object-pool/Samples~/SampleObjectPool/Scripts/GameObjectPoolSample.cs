using ACS.Core;
using ACS.ObjectPool.ObjectPool.Default;
using UnityEngine;
using UnityEngine.UI;

namespace Alexplay.Samples.GameObjectPool.Scripts
{
    public class GameObjectPoolSample : MonoBehaviour
    {
        [SerializeField] private Button _spawnButton;
        [SerializeField] private Button _returnFirst;
        [SerializeField] private Button _returnLast;
        
        [SerializeField] private GameObject _squarePrefab;
        [SerializeField] private Transform _squareParent;

        private IObjectPool _objectPool;
        private string _poolID;
        
        private void Start()
        {
            _objectPool = Core.Instance.ObjectPoolService.StandardObjectPool;

            _poolID = "poolForSquare";
            _objectPool.Append(_poolID, _squarePrefab);
            
            _spawnButton.onClick.AddListener(SpawnSquare);
            _returnFirst.onClick.AddListener(ReturnFirst);
            _returnLast.onClick.AddListener(ReturnLast);
        }

        private void SpawnSquare()
        {
            GameObject targetSquare = _objectPool.Get(_poolID);
            
            targetSquare.transform.SetParent(_squareParent);
            targetSquare.transform.localPosition = Vector3.zero;
        }

        private void ReturnFirst() => _objectPool.ReturnFirst(_poolID);
        private void ReturnLast() => _objectPool.ReturnLast(_poolID);

        private void OnDisable()
        {
            _objectPool.Remove(_poolID);
        }
    }
}
