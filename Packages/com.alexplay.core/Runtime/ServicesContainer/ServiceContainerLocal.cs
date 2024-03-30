using UnityEngine;

namespace ACS.Core.ServicesContainer
{
    [DefaultExecutionOrder(-11000)]
    public class ServiceContainerLocal : Bootstrapper
    {
        protected override void Bootstrap() => Container.AsScene();
    }
}