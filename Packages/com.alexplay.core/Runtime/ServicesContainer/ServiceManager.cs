using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ACS.Core.ServicesContainer
{
    public class ServiceManager : IDisposable
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public IEnumerable<object> RegisteredServices => _services.Values;
        
        public bool TryGet<T>(out T service) where T : class
        {
            Type type = typeof(T);

            if (_services.TryGetValue(type, out object @object))
            {
                service = @object as T;
                return true;
            }

            service = null;
            return false;
        }

        public T Get<T>() where T : class
        {
            Type type = typeof(T);

            if (_services.TryGetValue(type, out object service))
                return service as T;

            throw new ArgumentException($"ServiceManager.Register: Service of type {type.FullName} not registered");
        }

        public List<Type> GetAllRegisterTypes() => _services.Keys.ToList();

        public ServiceManager Register<T>(T service)
        {
            Type type = typeof(T);

            if (!_services.TryAdd(type, service))
                Debug.Log($"ServiceManager.Register: Service of type {type.FullName} already registered");

            return this;
        }

        public ServiceManager Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
                throw new ArgumentException("Type of service does not match type of service interface",
                    nameof(service));
            
            if (!_services.TryAdd(type, service))
                Debug.Log($"ServiceManager.Register: Service of type {type.FullName} already registered");

            return this;
        }

        public void Dispose()
        {
            foreach (KeyValuePair<Type, object> service in _services)
                if (service.Value is IDisposable disposableService) 
                    disposableService.Dispose();
        }
    }
}
