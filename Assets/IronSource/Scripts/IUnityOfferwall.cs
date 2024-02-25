using System;
using System.Collections.Generic;

namespace IS.IronSource.Scripts
{
    public interface IUnityOfferwall
    {

        event Action<IronSourceError> OnOfferwallShowFailed;

        event Action OnOfferwallOpened;

        event Action OnOfferwallClosed;

        event Action<IronSourceError> OnGetOfferwallCreditsFailed;

        event Action<Dictionary<string, object>> OnOfferwallAdCredited;

        event Action<bool> OnOfferwallAvailable;

    }
}
