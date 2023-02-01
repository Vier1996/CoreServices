using System;
using Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Data.DataService.Config
{
    [Serializable]
    public class DataServiceConfig : ServiceConfigBase
    {
        [ShowIf("@IsEnabled == true")]
        public CryptoKey CryptoKey = new CryptoKey();
    }

    [Serializable]
    public class CryptoKey
    {
        [Range(1, 9)] public int CryptoPartA;
        [Range(1, 9)] public int CryptoPartB;
        [Range(1, 9)] public int CryptoPartC;
        [Range(1, 9)] public int CryptoPartD;
    }
}
