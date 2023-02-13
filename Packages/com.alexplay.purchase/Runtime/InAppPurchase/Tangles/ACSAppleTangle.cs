using System;
using System.IO;
using UnityEngine;
using UnityEngine.Purchasing.Security;

namespace ACS.IAP.InAppPurchase.Tangles
{
    public class ACSAppleTangle
    {
        private const string appleCertPath = "Packages/com.unity.purchasing/Editor/AppleIncRootCertificate.cer";

        private TangleObfuscator _tangleObfuscator;
        
        private byte[] data;
        private int[] order;
        private int key;

        private readonly bool _prepared = false;

        public ACSAppleTangle()
        {
            try
            {
                _tangleObfuscator = new TangleObfuscator();
                
                byte[] bytes = File.ReadAllBytes(appleCertPath);
                
                order = new int[bytes.Length / 20 + 1];
                data = _tangleObfuscator.Obfuscate(bytes, order, out int rawKey);
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
