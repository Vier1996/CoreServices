using System;

namespace ject
{
    public class DiService : Attribute
    {
        public bool IsDeclareLazy => _declareLazy;
        
        private readonly bool _declareLazy;
     
        public DiService(bool lazy = false)
        {
            _declareLazy = lazy;
        }
    }
}