using System;
using ACS.ObjectPool.ObjectPool.Assets.Addressable;
using ACS.ObjectPool.ObjectPool.Assets.Resources;
using Zenject;

namespace ACS.ObjectPool.ObjectPool.Assets
{
    public class AssetService
    {
        public AddressableAssetsService Addressable
        {
            get
            {
                if (_addressable == null)
                    throw new NullReferenceException($"Before using {typeof(AddressableAssetsService)} you must -turn (ON) it in asset config");

                return _addressable;
            }
            set
            {
                
            }
        }

        public ResourceAssetService Resource
        {
            get
            {
                if (_resource == null)
                    throw new NullReferenceException($"Before using {typeof(ResourceAssetService)} you must -turn (ON) it in asset config");

                return _resource;
            }
            set
            {
                
            }
        }
        
        private AddressableAssetsService _addressable;
        private ResourceAssetService _resource;
        private DiContainer _diContainer;
        
        public AssetService(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public void CreateAddressableAssetService()
        {
            _addressable = new AddressableAssetsService(_diContainer);
        }

        public void CreateResourceAssetService()
        {
            _resource = new ResourceAssetService();
        }
    }
}