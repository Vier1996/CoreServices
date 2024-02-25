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

                throw new NullReferenceException("You can not use GlobalContainer without Core instance");
            }
        }
        
        public static ServiceContainer Core
        {
            get
            {
                if (_core != null) return _core;

                throw new NullReferenceException("You can not use CoreContainer without Core instance");
            }
        }
        
        private static ServiceContainer _core;
        private static ServiceContainer _global;
        private static Dictionary<Scene, ServiceContainer> _sceneContainers;
        
        private readonly ServiceManager _services = new();
        private static List<GameObject> _rootSceneGameObjects;

        internal void AsCore()
        {
            if (_core == this)
                Debug.LogWarning($"ServiceLocator.AsCore: Already configured as core", this);
            else if (_global != null)
                Debug.LogWarning($"ServiceLocator.AsCore: Another ServiceContainer is already configured as core", this);
            else
                _core = this;
        }
        
        public void AsGlobal(bool dontDestroyOnLoad)
        {
            if (_global == this)
                Debug.LogWarning($"ServiceLocator.AsGlobal: Already configured as global", this);
            else if (_global != null)
                Debug.LogWarning($"ServiceLocator.AsGlobal: Another ServiceContainer is already configured as global", this);
            else
            {
                _global = this;
                
                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
        }

        public void AsScene()
        {
            Scene scene = gameObject.scene;

            if (_sceneContainers.ContainsKey(scene))
            {
                Debug.LogWarning($"ServiceLocator.AsScene: Another ServiceLocator is already configured for this scene", this);
                return;
            }
            
            _sceneContainers.Add(scene, this);
        }

        public static ServiceContainer For(MonoBehaviour behaviour)
        {
            Scene scene = behaviour.gameObject.scene;

            if (_sceneContainers.TryGetValue(scene, out ServiceContainer container) && container != behaviour)
                return container;

            return SearchInScene(scene);
        }
        
        public static ServiceContainer ForCurrentScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            
            if (_sceneContainers.TryGetValue(scene, out ServiceContainer container))
                return container;

            return SearchInScene(scene);
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

        private bool TryGetNextInHierarchy(out ServiceContainer container)
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
        
        private static ServiceContainer SearchInScene(Scene scene)
        {
            _rootSceneGameObjects.Clear();
            
            scene.GetRootGameObjects(_rootSceneGameObjects);

            foreach (GameObject go in _rootSceneGameObjects.Where(go => go.GetComponent<ServiceContainerLocal>() != null))
            {
                if (go.TryGetComponent(out ServiceContainerLocal bootstrapper))
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return Global;
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