using System;
using System.Collections.Generic;
using System.Linq;
using ACS.SignalBus.SignalBus.Parent;
using UniRx;
using Zenject;

namespace ACS.SignalBus.SignalBus
{
    public class SignalBusService : ISignalBusService, IDisposable
    {
        private const string _sharpAssemblyName = "Assembly-CSharp";
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private System.Reflection.Assembly _sharpAssembly;
        private DiContainer _diContainer;
        private Zenject.SignalBus _signalBus;
        
        public SignalBusService(DiContainer diContainer)
        {
            _diContainer = diContainer;
            _sharpAssembly = AppDomain.CurrentDomain.GetAssemblies().First(atr => atr.GetName().Name.Equals(_sharpAssemblyName));
        }
        
        public void PrepareService()
        {
            InstallSignalBus();
            DeclareSignals();
        }
        
        public void Subscribe<TSignalType>(Action<TSignalType> callback) => _signalBus.Subscribe(callback);
        public void Unsubscribe<TSignalType>(Action<TSignalType> callback) => _signalBus.TryUnsubscribe(callback);
        public void Fire<TSignalType>(TSignalType signalMessage) => _signalBus.TryFire(signalMessage);

        private void InstallSignalBus()
        {
            SignalBusInstaller.Install(_diContainer);
            _signalBus = _diContainer.Resolve<Zenject.SignalBus>();
        }
        
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

        public void Dispose() => _disposables.Dispose();
    }
}
