using System;
using System.Collections.Generic;
using System.Linq;

namespace ACS.SignalBus.SignalBus.NativeSignalBus
{
    public class SignalBus
    {
        private readonly Dictionary<Type, List<SignalBindInfo>> _declaredSignals;

        public SignalBus() => _declaredSignals = new Dictionary<Type, List<SignalBindInfo>>();

        public void Subscribe<TSignal>(Action<TSignal> callback) => 
            _declaredSignals[typeof(TSignal)].Add(new SignalBindInfo().Setup(callback));

        public void TryUnsubscribe<TSignal>(Action<TSignal> callback)
        {
            Type signalType = typeof(TSignal);
            
            if (_declaredSignals.ContainsKey(signalType))
            {
                SignalBindInfo bindInfo = _declaredSignals[signalType].FirstOrDefault(sgn => sgn.IsEqualCallback(callback));
                
                if(bindInfo != default)
                    _declaredSignals[signalType].Remove(bindInfo);
            }
        }
        
        public void TryUnsubscribeAllSignalByType<TSignal>()
        {
            Type signalType = typeof(TSignal);
            
            if (_declaredSignals.ContainsKey(signalType)) 
                _declaredSignals[signalType].Clear();
        }

        public void TryFire<TSignal>(TSignal signal)
        {
            Type signalType = typeof(TSignal);
            
            if (_declaredSignals.ContainsKey(signalType))
            {
                List<SignalBindInfo> listenersOfType = _declaredSignals[signalType];
                
                for (int i = 0; i < listenersOfType.Count; i++)
                {
                    if (listenersOfType[i] != null) listenersOfType[i].Dispatch(signal);
                    else listenersOfType.RemoveAt(i);
                }
            }
        }

        public void DeclareSignal(Type sType)
        {
            if(!_declaredSignals.ContainsKey(sType))
                _declaredSignals.Add(sType, new List<SignalBindInfo>());
        }
        
        public bool IsSignalDeclared<TSignal>() => _declaredSignals.ContainsKey(typeof(TSignal));

        public void ClearAll() => _declaredSignals.Clear();
    }
}