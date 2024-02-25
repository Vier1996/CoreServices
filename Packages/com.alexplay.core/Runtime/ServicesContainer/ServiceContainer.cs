using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ACS.Core.ServicesContainer
{
    public class ServiceContainer : MonoBehaviour
    {
        public static ServiceContainer Global
        {
            get
            {
                if (_global != null) return _global;

                throw new NullReferenceException("You can not use GlobalContext without Core instance");
            }
        }
        
        private static ServiceContainer _global;
        private readonly ServiceManager _services = new();
        
        private static List<GameObject> _rootSceneGameObjects;
        private static Dictionary<Scene, ServiceContainer> _sceneContainers;

        internal void AsGlobal()
        {
            if (_global == this)
                Debug.LogWarning($"ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
            else if (_global != null)
                Debug.LogWarning($"ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global", this);
            else
                _global = this;
        }

        internal void AsScene()
        {
            Scene scene = gameObject.scene;

            if (_sceneContainers.ContainsKey(scene))
            {
                Debug.LogWarning($"ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene", this);
                return;
            }
            
            _sceneContainers.Add(scene, this);
        }

        public static ServiceContainer For(MonoBehaviour behaviour)
        {
            Scene scene = behaviour.gameObject.scene;

            if (_sceneContainers.TryGetValue(scene, out ServiceContainer container) && container != behaviour)
                return container;

            _rootSceneGameObjects.Clear();
            
            scene.GetRootGameObjects(_rootSceneGameObjects);

            foreach (GameObject go in _rootSceneGameObjects.Where(go => go.GetComponent<ServiceContainerLocal>() != null))
            {
                if (go.TryGetComponent(out ServiceContainerLocal bootstrapper) && bootstrapper.Container != behaviour)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return Global;
        }

        public ServiceContainer Register<T>(T service)
        {
            _services.Register(service);
            return this;
        }
        
        public ServiceContainer Register(Type type, object service)
        {
            _services.Register(type, service);
            return this;
        }

        public ServiceContainer Get<T>(out T service) where T : class
        {
            if (TryGetService(out service)) return this;

            if (TryGetNextInHierarchy(out ServiceContainer container) == false)
                throw new ArgumentException($"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered");
            
            container.Get(out service);
            
            return this;
        }
        
        public bool TryGetService<T>(out T service) where T : class => _services.TryGet(out service);

        public bool TryGetNextInHierarchy(out ServiceContainer container)
        {
            if (this == _global)
            {
                container = null;
                return false;
            }

            if (transform.parent != null)
            {
                ServiceContainer nextContainer = transform.parent.GetComponentInParent<ServiceContainer>();

                if (nextContainer != null)
                    container = nextContainer;
                else
                    container = For(this);
            }
            else
            {
                container = null;
            }

            return container != null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _global = null;
            _sceneContainers = new Dictionary<Scene, ServiceContainer>();
            _rootSceneGameObjects = new List<GameObject>();
        }

        private void OnDestroy()
        {
            if (this == _global)
                _global = null;
            else if (_sceneContainers != null && _sceneContainers.ContainsValue(this))
            {
                _sceneContainers.Remove(gameObject.scene);
            }
        }

#if UNITY_EDITOR
        [MenuItem("ServiceContainer/Create local scene container")]
        static void AddScene() => new GameObject("ServiceContainer [Local]", typeof(ServiceContainerLocal));
#endif
    }
}