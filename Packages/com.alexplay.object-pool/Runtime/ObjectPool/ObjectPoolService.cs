using System;
using ACS.ObjectPool.ObjectPool.Addressable;
using ACS.ObjectPool.ObjectPool.Assets;
using ACS.ObjectPool.ObjectPool.Config;
using ACS.ObjectPool.ObjectPool.Default;
using UnityEngine;
using Zenject;

namespace ACS.ObjectPool.ObjectPool
{
    public class ObjectPoolService : IObjectPoolService
    {
        public IAddressableObjectsPool AddressableObjectPool
        {
            get
            {
                if (_addressable == null)
                    throw new NullReferenceException($"Before using {typeof(AddressableObjectsPool)} you must -turn (ON) it in object pool config");

                return _addressable;
            }
            set
            {
                
            }
        }

        public IObjectPool StandardObjectPool
        {
            get
            {
                if (_objectPool == null)
                    throw new NullReferenceException($"Before using {typeof(Default.ObjectPool)} you must -turn (ON) it in object pool config");

                return _objectPool;
            }
            set
            {
                
            }
        }
        
        private readonly ObjectPoolConfig _objectPoolConfig;
        private readonly AssetService _assetService;
        
        private AddressableObjectsPool _addressable;
        private Default.ObjectPool _objectPool;

        private readonly Transform _coreParent;
        private Transform _defaultPoolParent;

        public ObjectPoolService(Transform coreParent, DiContainer diContainer, ObjectPoolConfig objectPoolConfig)
        {
            _assetService = new AssetService(diContainer);
            _coreParent = coreParent;
            _objectPoolConfig = objectPoolConfig;
        }
        
        public void PrepareService()
        {
            GameObject defaultParent = new GameObject("DefaultPoolParent");
            defaultParent.transform.SetParent(_coreParent.transform);
            _defaultPoolParent = defaultParent.transform;

            if (_objectPoolConfig.AddressablePool)
            {
                _assetService.CreateAddressableAssetService();
                
                _addressable = new AddressableObjectsPool(_assetService.Addressable);
                _addressable.SetDefaultParent(_defaultPoolParent);
            }
            
            if (_objectPoolConfig.StandardPool)
            {
                _assetService.CreateResourceAssetService();
                
                _objectPool = new Default.ObjectPool();
                _objectPool.SetDefaultParent(_defaultPoolParent);
            }
        }
    }

    public interface IObjectPoolService
    {
        public IAddressableObjectsPool AddressableObjectPool { get; set; }
        public IObjectPool StandardObjectPool { get; set; }
    }
}
