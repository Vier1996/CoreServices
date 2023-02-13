using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor.Purchasing;
#endif
using UnityEngine;
using UnityEngine.Purchasing.Security;

namespace ACS.IAP.InAppPurchase.Tangles
{
    public class ACSAppleTangle
    {
        private const string appleCertPath = "Packages/com.unity.purchasing/Editor/AppleIncRootCertificate.cer";

        private byte[] data;
        private int[] order;
        private int key;

        private readonly bool _prepared = false;

        public ACSAppleTangle()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(appleCertPath);
                
                order = new int[bytes.Length / 20 + 1];
                data = TangleObfuscator.Obfuscate(bytes, order, out int rawKey);
                key = rawKey;
                
                _prepared = data.Length != 0;
            }
            catch (Exception e)
            {
                Debug.LogError("Invalid Apple Root Certificate. Generating incomplete credentials file. " + e);
            }
        }
        
        public byte[] Data()
        {
            if (_prepared == false)
                return null;
            
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
