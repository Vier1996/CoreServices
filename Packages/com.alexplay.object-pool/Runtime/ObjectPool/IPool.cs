using System.Collections.Generic;
using UnityEngine;

namespace ACS.ObjectPool.ObjectPool
{
    public interface IPool
    {
        public Dictionary<string, List<GameObject>> Pool { get; set; }
        public void SetDefaultParent(Transform defaultParent);
    }
}
