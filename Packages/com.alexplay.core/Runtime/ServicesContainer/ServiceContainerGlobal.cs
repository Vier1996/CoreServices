using UnityEngine;

namespace ACS.Core.ServicesContainer
{
    [DefaultExecutionOrder(-11500)]
    public class ServiceContainerGlobal : Bootstrapper
    {
        protected override void Bootstrap() => Container.AsGlobal(true);
    }
}