using System;
using System.Collections.Generic;
using System.Linq;
using ACS.SignalBus.SignalBus.Parent;
using UniRx;
#if COM_ALEXPLAY_ZENJECT_EXTENSION
using Zenject;
#endif

namespace ACS.SignalBus.SignalBus
{
    public class SignalBusService : ISignalBusService, IDisposable
    {
        private const string _sharpAssemblyName = "Assembly-CSharp";
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private System.Reflection.Assembly _sharpAssembly;
        
#if COM_ALEXPLAY_ZENJECT_EXTENSION
        private DiContainer _diContainer;
        private Zenject.SignalBus _signalBus;
        
        public SignalBusService(DiContainer diContainer)
        {
            _diContainer = diContainer;
            _sharpAssembly = AppDomain.CurrentDomain.GetAssemblies().First(atr => atr.GetName().Name.Equals(_sharpAssemblyName));
        }
#else
        private NativeSignalBus.SignalBus _signalBus;
        
        public SignalBusService()
        {
            _sharpAssembly = AppDomain.CurrentDomain.GetAssemblies().First(atr => atr.GetName().Name.Equals(_sharpAssemblyName));
        }
#endif
        
        public void PrepareService()
        {
            InstallSignalBus();
            DeclareSignals();
        }
        
#if COM_ALEXPLAY_ZENJECT_EXTENSION
        public void Subscribe<TSignalType>(Action<TSignalType> callback)
#else
        public void Subscribe<TSignalType>(Action<TSignalType> callback)
#endif
        {
            _signalBus.Subscribe(callback);
        }

        public void Unsubscribe<TSignalType>(Action<TSignalType> callback) => _signalBus.TryUnsubscribe(callback);
        public void Fire<TSignalType>(TSignalType signalMessage) => _signalBus.TryFire(signalMessage);
        public void IsSignalDeclared<TSignalType>(TSignalType signalMessage) => _signalBus.IsSignalDeclared<TSignalType>();


        private void InstallSignalBus()
        {
#if COM_ALEXPLAY_ZENJECT_EXTENSION
            SignalBusInstaller.Install(_diContainer);
            _signalBus = _diContainer.Resolve<Zenject.SignalBus>();
#else
            _signalBus = new NativeSignalBus.SignalBus();
#endif
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
