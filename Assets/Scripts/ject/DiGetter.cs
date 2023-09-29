using System;
using UnityEngine.Events;

namespace ject
{
    public class DiGetter
    {
        public event UnityAction<Type> Created;
        
        public DiGetter()
        {
            Created?.Invoke(GetType());
        }
    }
}