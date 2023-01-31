using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ACS.ObjectPool.ObjectPool.Assets.Reference
{
    public class ComponentReference<TComponent> : AssetReference
    {
        private AsyncOperationHandle<TComponent> _operationHandle;

        public ComponentReference(string guid) : base(guid) { }

        public new AsyncOperationHandle<TComponent> InstantiateAsync(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(
                base.InstantiateAsync(position, Quaternion.identity, parent),
                GameObjectReady);
        }

        public new AsyncOperationHandle<TComponent> InstantiateAsync(Transform parent = null, bool instantiateInWorldSpace = false)
        {
            return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(
                base.InstantiateAsync(parent, instantiateInWorldSpace),
                GameObjectReady);
        }

        public AsyncOperationHandle<TComponent> LoadAssetAsync()
        {
            if (!_operationHandle.IsDone) return _operationHandle;

            _operationHandle = Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(
                base.LoadAssetAsync<GameObject>(),
                GameObjectReady);

            return _operationHandle;
        }

        public AsyncOperationHandle<TComponent> TryToLoadAssetAsync()
        {
            if (ValidateAsset(Asset))
            {
                var result = ((GameObject) Asset).GetComponent<TComponent>();
                return Addressables.ResourceManager.CreateCompletedOperation(result, string.Empty);
            }

            return LoadAssetAsync();
        }

        AsyncOperationHandle<TComponent> GameObjectReady(AsyncOperationHandle<GameObject> arg)
        {
            var comp = arg.Result.GetComponent<TComponent>();
            return Addressables.ResourceManager.CreateCompletedOperation<TComponent>(comp, string.Empty);
        }

        public override bool ValidateAsset(Object obj)
        {
            GameObject go = obj as GameObject;
            return go != null && go.GetComponent<TComponent>() != null;
        }

        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return go != null && go.GetComponent<TComponent>() != null;
#else
            return false;
#endif
        }

        public void ReleaseInstance(AsyncOperationHandle<TComponent> op)
        {
            var component = op.Result as Component;

            if (component != null) 
                Addressables.ReleaseInstance(component.gameObject);

            Addressables.Release(op);
        }
    }
}