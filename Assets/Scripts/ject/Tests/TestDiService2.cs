using UnityEngine;

namespace ject
{
    [DiService(lazy: true)]
    public class TestDiService2
    {
        public void SendMessage()
        {
            Debug.Log("Sample log 2");
        }
    }
}