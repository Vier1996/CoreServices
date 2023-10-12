using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if COM_ALEXPLAY_ZENJECT_EXTENSION
using Zenject;
#endif
using Object = UnityEngine.Object;

namespace ACS.ObjectPool.ObjectPool.Assets.Addressable
{
    public class AddressableAssetsService
    {
#if COM_ALEXPLAY_ZENJECT_EXTENSION
        private readonly DiContainer _diContainer;
#endif
        private readonly Dictionary<string, AsyncOperationHandle<Object>> _preparedReferences;

#if COM_ALEXPLAY_ZENJECT_EXTENSION
        public AddressableAssetsService(DiContainer diContainer)
        {
            _diContainer = diContainer;
            _preparedReferences = new Dictionary<string, AsyncOperationHandle<Object>>();
        }
#else
        public AddressableAssetsService()
        {
            _preparedReferences = new Dictionary<string, AsyncOperationHandle<Object>>();
        }
#endif
        
        public async UniTask<Object> Get(AssetReference reference)
        {
            if (_preparedReferences.ContainsKey(reference.AssetGUID) == false) 
                    await PrepareAsset(reference);
            
#if COM_ALEXPLAY_ZENJECT_EXTENSION
            return _diContainer.InstantiatePrefab(_preparedReferences[reference.AssetGUID].Result);
#else
            return Object.Instantiate((_preparedReferences[reference.AssetGUID].Result));
#endif
        }
        
        private async UniTask PrepareAsset(AssetReference reference)
        {
            AsyncOperationHandle<Object> handle = GetHandle(reference);
            await handle.Task;
        }
        
        private AsyncOperationHandle<Object> GetHandle(AssetReference reference)
        {
            if (_preparedReferences.ContainsKey(reference.AssetGUID))
                return _preparedReferences[reference.AssetGUID];

            AsyncOperationHandle<Object> handle = reference.LoadAssetAsync<Object>();

            _preparedReferences.Add(reference.AssetGUID, handle);

            return handle;
        }
    }
}
