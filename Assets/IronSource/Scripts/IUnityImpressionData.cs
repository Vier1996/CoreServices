using System;

namespace IS.IronSource.Scripts
{
    public interface IUnityImpressionData
    {
        event Action<IronSourceImpressionData> OnImpressionDataReady;

        event Action<IronSourceImpressionData> OnImpressionSuccess;
    }
}
