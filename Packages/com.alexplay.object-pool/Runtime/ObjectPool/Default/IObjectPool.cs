using UnityEngine;

namespace ACS.ObjectPool.ObjectPool.Default
{
    public interface IObjectPool
    {
        public void Append(string poolID, GameObject objectInstance);
        public GameObject Get(string poolID);
        public void Return(string poolID, GameObject objectInstance);
        public void ReturnFirst(string poolID);
        public void ReturnLast(string poolID);
        public void Remove(string poolID);
    }
}