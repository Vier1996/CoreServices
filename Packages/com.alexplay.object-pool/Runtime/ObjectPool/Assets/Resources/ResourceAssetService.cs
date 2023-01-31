using Object = UnityEngine.Object;

namespace ACS.ObjectPool.ObjectPool.Assets.Resources
{
    public class ResourceAssetService
    {
        public T Get <T>(string path) where T : Object => UnityEngine.Resources.Load<T>(path);
    }
}
