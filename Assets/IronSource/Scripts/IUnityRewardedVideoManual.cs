using System;

namespace IS.IronSource.Scripts
{
    public interface IUnityRewardedVideoManual
    {
        event Action OnRewardedVideoAdReady;

        event Action<IronSourceError> OnRewardedVideoAdLoadFailed;

    }
}