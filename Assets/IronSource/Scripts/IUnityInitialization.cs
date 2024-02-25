using System;

namespace IS.IronSource.Scripts
{
     public interface IUnityInitialization
     {
          event Action OnSdkInitializationCompletedEvent;
     }
}
