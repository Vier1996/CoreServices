using System;

namespace IS.IronSource.Scripts
{
    public interface IUnitySegment
    {
        event Action<String> OnSegmentRecieved;
    }
}
