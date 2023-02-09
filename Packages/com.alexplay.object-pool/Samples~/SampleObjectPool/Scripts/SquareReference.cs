using ACS.ObjectPool.ObjectPool.Assets.Reference;

namespace Alexplay.Samples.GameObjectPool.Scripts
{
    [System.Serializable]
    public class SquareReference : ComponentReference<Square>
    {
        public SquareReference(string guid) : base(guid)
        {
        }
    }
}