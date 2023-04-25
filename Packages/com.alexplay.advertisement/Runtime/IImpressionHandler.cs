using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ACS.Ads
{
    public interface IImpressionHandler
    {
        UniTaskVoid HandleImpression(Dictionary<string, string> impressionParam, double revenue);
    }
}