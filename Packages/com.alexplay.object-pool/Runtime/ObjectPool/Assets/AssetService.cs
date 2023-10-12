using System;
using ACS.ObjectPool.ObjectPool.Assets.Addressable;
using ACS.ObjectPool.ObjectPool.Assets.Resources;
#if COM_ALEXPLAY_ZENJECT_EXTENSION
using Zenject;
#endif

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
#if COM_ALEXPLAY_ZENJECT_EXTENSION
        private DiContainer _diContainer;
        
        public AssetService(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
#else
        public AssetService() { }
#endif
        

        public void CreateAddressableAssetService()
        {
            _addressable = 
#if COM_ALEXPLAY_ZENJECT_EXTENSION
                new AddressableAssetsService(_diContainer);
#else
                new AddressableAssetsService();
#endif
        }

        public void CreateResourceAssetService()
        {
            _resource = new ResourceAssetService();
        }
    }
}