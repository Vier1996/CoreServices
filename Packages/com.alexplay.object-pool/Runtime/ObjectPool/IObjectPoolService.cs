using ACS.ObjectPool.ObjectPool.Addressable;
using ACS.ObjectPool.ObjectPool.Default;

namespace ACS.ObjectPool.ObjectPool
{
    public interface IObjectPoolService
    {
        public IAddressableObjectsPool AddressableObjectPool { get; set; }
        public IObjectPool StandardObjectPool { get; set; }
    }
}