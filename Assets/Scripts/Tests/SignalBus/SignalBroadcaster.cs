using System.Collections.Generic;
using ACS.Core;
using ACS.SignalBus.SignalBus.Parent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tests.SignalBus
{
    public class SignalBroadcaster : MonoBehaviour
    {

        [Button] private void Broadcast()
        {
            Core.Instance.SignalBusService.Fire(new RSignal());
        }
    }
   
    [Signal] public struct RSignal { }
}