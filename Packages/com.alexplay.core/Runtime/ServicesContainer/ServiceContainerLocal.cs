namespace ACS.Core.ServicesContainer
{
    public class ServiceContainerLocal : Bootstrapper
    {
        protected override void Bootstrap() => Container.AsScene();
    }
}