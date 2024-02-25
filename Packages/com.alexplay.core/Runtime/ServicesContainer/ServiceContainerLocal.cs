using Sirenix.OdinInspector;

namespace ACS.Core.ServicesContainer
{
    public class ServiceContainerLocal : Bootstrapper
    {
        [ReadOnly, ShowInInspector] private readonly string _alert = "Используйте как родительский класс, где каждый наследник представляет бутстрап своей сцены";
        
        protected override void Bootstrap() => Container.AsScene();
    }
}