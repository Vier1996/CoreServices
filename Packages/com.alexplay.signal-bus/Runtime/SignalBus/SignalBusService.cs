using System;
using System.Collections.Generic;
using System.Linq;
using ACS.SignalBus.SignalBus.Parent;

namespace ACS.SignalBus.SignalBus
{
    public class SignalBusService : ISignalBusService, IDisposable
    {
        private const string SharpAssemblyName = "Assembly-CSharp";
        
        private readonly System.Reflection.Assembly _sharpAssembly;
        private NativeSignalBus.SignalBus _signalBus;
        
        public SignalBusService() => 
            _sharpAssembly = AppDomain.CurrentDomain.GetAssemblies().First(atr => atr.GetName().Name.Equals(SharpAssemblyName));

        public void PrepareService()
        {
            _signalBus = new NativeSignalBus.SignalBus();
            
            DeclareSignals();
        }
        public void Subscribe<TSignalType>(Action<TSignalType> callback) => _signalBus.Subscribe(callback);

        public void Unsubscribe<TSignalType>(Action<TSignalType> callback) => _signalBus.TryUnsubscribe(callback);
        
        public void TryUnsubscribeAllSignalByType<TSignal>() => _signalBus.TryUnsubscribeAllSignalByType<TSignal>();

        public void Fire<TSignalType>(TSignalType signalMessage) => _signalBus.TryFire(signalMessage);
        
        public void IsSignalDeclared<TSignalType>(TSignalType signalMessage) => _signalBus.IsSignalDeclared<TSignalType>();
        
        private void DeclareSignals()
        {
           List<Type> signalTypes = GetTypesWithAttribute<SignalAttribute, Signal>();

           for (int i = 0; i < signalTypes.Count; i++)
           {
               Type sType = signalTypes[i];
               _signalBus.DeclareSignal(sType);
           }
        }
        
        private List<Type> GetTypesWithAttribute<TAttribute, TType>()
        {
            List<Type> types = new List<Type>();
 
            foreach(Type type in _sharpAssembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TAttribute), true).Length > 0 &&
                    !type.Name.Equals(typeof(TType).Name))
                    types.Add(type);
            }
 
            return types;
        }

        public void Dispose() => _signalBus.ClearAll();
    }
}
