﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Object = UnityEngine.Object;

namespace ACS.ObjectPool.ObjectPool.Default
{
    public class ObjectPool : IObjectPool, IPool
    {
        public Dictionary<string, List<GameObject>> Pool { get; set; } = new();

        private readonly Dictionary<string, GameObject> _poolOfInstances = new();
        
        private Transform _defaultParent = null;

        public void SetDefaultParent(Transform defaultParent) => _defaultParent = defaultParent;

        public bool Contains(string poolID) => Pool.ContainsKey(poolID);

        public void Append(string poolID, GameObject objectInstance)
        {
            if (Pool.ContainsKey(poolID) == false)
            {
                Pool.Add(poolID, new List<GameObject>());
                _poolOfInstances.Add(poolID, objectInstance);
            }
        }

        public GameObject Get(string poolID)
        {
            if (Pool.ContainsKey(poolID) == false)
            {
                throw new ArgumentException($"You try to get pool element with poolID: {poolID}, that not present in the Dictionary, " +
                                            $"you can try call 'Append (method)' and try it another one.");
            }
            
            if (Pool[poolID].Count > 0)
            {
                GameObject element = FindAvailable(poolID);

                if (element != null)
                {
                    element.gameObject.SetActive(true);
                    return element;
                }
            }
            
            return AddNewInstance(poolID);
        }
        
        public void Return(string poolID, GameObject objectInstance)
        {
            if (Pool.ContainsKey(poolID) == false)
                throw new Exception("Trying to return object with no appending in pool");
            
            objectInstance.gameObject.SetActive(false);
            objectInstance.transform.SetParent(_defaultParent);
        }

        public void ReturnFirst(string poolID)
        {
            if (Pool.ContainsKey(poolID) == false)
                throw new Exception("Trying to return object with no appending in pool");

            if (Pool[poolID].Count > 0)
            {
                for (int i = 0; i < Pool[poolID].Count; i++)
                {
                    if (Pool[poolID][i].activeSelf)
                    {
                        Return(poolID, Pool[poolID][i]);
                        return;
                    }
                }
            }
        }
        
        public void ReturnLast(string poolID)
        {
            if (Pool.ContainsKey(poolID) == false)
                throw new Exception("Trying to return object with no appending in pool");

            if (Pool[poolID].Count > 0)
            {
                for (int i = Pool[poolID].Count - 1; i >= 0; i--)
                {
                    if (Pool[poolID][i].activeSelf)
                    {
                        Return(poolID, Pool[poolID][i]);
                        return;
                    }
                }
            }
        }

        public void ReturnAll(string poolID)
        {
            if (Pool.ContainsKey(poolID) == false)
                throw new Exception("Trying to return object with no appending in pool");

            if (Pool[poolID].Count > 0)
            {
                for (int i = 0; i < Pool[poolID].Count; i++)
                {
                    if (Pool[poolID][i].activeSelf)
                    {
                        Return(poolID, Pool[poolID][i]);
                    }
                }
            }
        }

        public void Remove(string poolID)
        {
            if (Pool.ContainsKey(poolID) == true)
            {
                for (int i = 0; i < Pool[poolID].Count; i++)
                    Object.Destroy(Pool[poolID][i].gameObject);
                Pool.Remove(poolID);
            }
        }
        
        public void PartialRelease()
        {
            foreach (var poolElement in Pool)
            {
                if(poolElement.Value.Count <= 2) continue;

                while (poolElement.Value.Count > 2)
                {
                    GameObject gameObject = Pool[poolElement.Key][2].gameObject;
                    Pool[poolElement.Key].Remove(gameObject);
                    Object.Destroy(gameObject);
                }
            }
        }
        
        public void FullRelease()
        {
            while (Pool.Count > 0) 
                Remove(Pool.First().Key);
        }
        
        private GameObject AddNewInstance(string poolID)
        {
            GameObject element = UnityEngine.Object.Instantiate(_poolOfInstances[poolID]);
            
            Pool[poolID].Add(element);
            
            return element;
        }

        private GameObject FindAvailable(string poolID)
        {
            for (int i = 0; i < Pool[poolID].Count; i++)
            {
                if (Pool[poolID][i] != null && Pool[poolID][i].activeSelf == false)
                    return Pool[poolID][i];
            }
            return null;
        }
    }
}