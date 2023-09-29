using UnityEngine;

namespace ject
{
    [DiService]
    public class TestDiService
    {
        public void SendMessage()
        {
            Debug.Log("Sample log 1");
        }
    }
}