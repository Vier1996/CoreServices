using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Object = UnityEngine.Object;

namespace ACS.ObjectPool.ObjectPool.Assets.Addressable
{
    public class AddressableAssetsService
    {
        private readonly DiContainer _diContainer;
        private readonly Dictionary<string, AsyncOperationHandle<Object>> _preparedReferences;

        public AddressableAssetsService(DiContainer diContainer)
        {
            _diContainer = diContainer;
            _preparedReferences = new Dictionary<string, AsyncOperationHandle<Object>>();
        }

        public async UniTask<Object> Get(AssetReference reference)
        {
            if (_preparedReferences.ContainsKey(reference.AssetGUID) == false) 
                    await PrepareAsset(reference);
            
            return _diContainer.InstantiatePrefab(_preparedReferences[reference.AssetGUID].Result);
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
