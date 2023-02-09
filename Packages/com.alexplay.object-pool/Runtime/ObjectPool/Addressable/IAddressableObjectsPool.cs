using ACS.ObjectPool.ObjectPool.Assets.Reference;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ACS.ObjectPool.ObjectPool.Addressable
{
    public interface IAddressableObjectsPool
    {
        public UniTask<GameObject> Get(AssetReference reference);
        public UniTask<T> Get<T>(ComponentReference<T> reference);
        public UniTask<T> CreateInstance<T>(ComponentReference<T> reference);

        public void Return(string id, GameObject objectInstance);
        public void Return<T>(ComponentReference<T> reference, T obj) where T : Component;
        public void ReturnAll<T>(ComponentReference<T> reference);

        public void ReturnFirst(AssetReference reference);
        public void ReturnLast(AssetReference reference);
        public void Remove(AssetReference reference);
    }
}