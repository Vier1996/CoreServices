using UnityEngine;

namespace ject
{
    public class vJectTestsStarter : MonoBehaviour
    {
        private vJect _vJect;
        
        private void Awake()
        {
            _vJect = new vJect();
        }
    }
}