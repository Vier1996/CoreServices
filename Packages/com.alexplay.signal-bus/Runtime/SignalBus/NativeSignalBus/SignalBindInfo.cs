using System;

namespace ACS.SignalBus.SignalBus.NativeSignalBus
{
    public class SignalBindInfo
    {
        private object _callback;
        
        public SignalBindInfo Setup<TSignal>(Action<TSignal> callback)
        {
            _callback = callback;
            return this;
        }
        
        public void Dispatch<TSignal>(TSignal callback)
        {
            if (_callback is Action<TSignal> typedCallback) 
                typedCallback?.Invoke(callback);
        }
    }
}