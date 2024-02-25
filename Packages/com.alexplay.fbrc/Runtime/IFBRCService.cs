using Cysharp.Threading.Tasks;

namespace ACS.FBRC
{
    public interface IFBRCService
    {
        public UniTask EnsureInitialized();
        public UniTask EnsureFetchedDataActivated();
    }
}