using UnityEngine;

namespace ACS.Core.ServicesContainer
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceContainer))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        public ServiceContainer Container => _container ??= _container = GetComponent<ServiceContainer>();
        
        private ServiceContainer _container;
        private bool _hasBeenBootstrapped;
        
        private void Awake() => BootstrapOnDemand();

        public void BootstrapOnDemand()
        {
            if(_hasBeenBootstrapped) return;

            _hasBeenBootstrapped = true;

            Bootstrap();
        }

        protected abstract void Bootstrap();
    }
}