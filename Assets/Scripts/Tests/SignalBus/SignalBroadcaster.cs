using System.Collections.Generic;
using ACS.Core;
using ACS.SignalBus.SignalBus.Parent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tests.SignalBus
{
    public class SignalBroadcaster : MonoBehaviour
    {
    }
   
    [Signal] public class RSignal { }
}