using System;
using System.Collections.Generic;
using ACS.ObjectPool.ObjectPool.Assets.Addressable;
using ACS.ObjectPool.ObjectPool.Assets.Reference;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace ACS.ObjectPool.ObjectPool.Addressable
{
    public class AddressableObjectsPool : IAddressableObjectsPool, IPool
    {
        public Dictionary<string, List<GameObject>> Pool { get; set; }
        
        private readonly AddressableAssetsService _assets;

        private Transform _defaultParent = null;

        public AddressableObjectsPool(AddressableAssetsService service)
        {
            Pool = new Dictionary<string, List<GameObject>>();
            _assets = service;
        }

        public void SetDefaultParent(Transform defaultParent) => _defaultParent = defaultParent;

        public async UniTask<GameObject> Get(AssetReference reference)
        {
            if (Pool.ContainsKey(reference.AssetGUID) == false)
                Pool.Add(reference.AssetGUID, new List<GameObject>());
            
            if (Pool[reference.AssetGUID].Count > 0)
            {
                GameObject element = FindAvailable(reference);

                if (element != null)
                {
                    element.gameObject.SetActive(true);
                    return element;
                }
            }
            return await AddNewInstance(reference);
        }

        public async UniTask<T> Get<T>(ComponentReference<T> reference)
        {
            GameObject gObject = await Get((AssetReference) reference);
            return gObject.GetComponent<T>();
        }

        public void Return(string id, GameObject objectInstance)
        {
            if (Pool.ContainsKey(id) == false)
                throw new Exception("Trying to return asset with not prepared reference");
            
            objectInstance.gameObject.SetActive(false);
            objectInstance.transform.SetParent(_defaultParent);
        }

        public void Return<T>(ComponentReference<T> reference, T obj) where T : Component => 
            Return(reference.AssetGUID, obj.gameObject);
        
        public void ReturnAll<T>(ComponentReference<T> reference)
        {
            if (Pool.ContainsKey(reference.AssetGUID))
            {
                for (int i = 0; i < Pool[reference.AssetGUID].Count; i++)
                {
                    if (Pool[reference.AssetGUID][i].activeSelf)
                        Return(reference.AssetGUID, Pool[reference.AssetGUID][i]);
                }
            }
        }
        
        public void ReturnFirst(AssetReference reference)
        {
            if (Pool.ContainsKey(reference.AssetGUID) == false)
                throw new Exception("Trying to return object with no appending in pool");

            if (Pool[reference.AssetGUID].Count > 0)
            {
                for (int i = 0; i < Pool[reference.AssetGUID].Count; i++)
                {
                    if (Pool[reference.AssetGUID][i].activeSelf)
                    {
                        Return(reference.AssetGUID, Pool[reference.AssetGUID][i]);
                        return;
                    }
                }
            }
        }
        
        public void ReturnLast(AssetReference reference)
        {
            if (Pool.ContainsKey(reference.AssetGUID) == false)
                throw new Exception("Trying to return object with no appending in pool");

            if (Pool[reference.AssetGUID].Count > 0)
            {
                for (int i = Pool[reference.AssetGUID].Count - 1; i >= 0; i--)
                {
                    if (Pool[reference.AssetGUID][i].activeSelf)
                    {
                        Return(reference.AssetGUID, Pool[reference.AssetGUID][i]);
                        return;
                    }
                }
            }
        }

        private GameObject FindAvailable(AssetReference reference)
        {
            for (int i = 0; i < Pool[reference.AssetGUID].Count; i++)
            {
                if (Pool[reference.AssetGUID][i].activeSelf == false)
                    return Pool[reference.AssetGUID][i];
            }
            return null;
        }
        
        private async UniTask<GameObject> AddNewInstance(AssetReference reference)
        {
            GameObject element = await _assets.Get(reference) as GameObject;
            Pool[reference.AssetGUID].Add(element);
            return element;
        }

        public void Remove(AssetReference reference)
        {
            if (Pool.ContainsKey(reference.AssetGUID) == true)
            {
                for (int i = 0; i < Pool[reference.AssetGUID].Count; i++)
                    Object.Destroy(Pool[reference.AssetGUID][i].gameObject);
                Pool.Remove(reference.AssetGUID);
            }
        }
    }
}