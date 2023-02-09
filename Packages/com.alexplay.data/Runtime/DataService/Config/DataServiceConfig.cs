using System;
using Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Data.DataService.Config
{
    [Serializable]
    public class DataServiceConfig : ServiceConfigBase
    {
        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.data";

        [ShowIf("@IsEnabled == true")]
        public CryptoKey CryptoKey = new CryptoKey();
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
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
