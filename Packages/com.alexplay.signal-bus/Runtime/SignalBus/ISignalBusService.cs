using System;

namespace ACS.SignalBus.SignalBus
{
    public interface ISignalBusService
    {
        public void Subscribe<TSignalType>(Action<TSignalType> callback);
        public void Unsubscribe<TSignalType>(Action<TSignalType> callback);
        public void Fire<TSignalType>(TSignalType signalMessage);
        public void IsSignalDeclared<TSignalType>(TSignalType signalMessage);
    }
}