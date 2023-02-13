using System;
using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.Purchasing.Security;

namespace ACS.IAP.InAppPurchase.Tangles
{
    public class ACSGooglePlayTangle
    {
        private byte[] data;
        private int[] order;
        private int key;

        private readonly bool _prepared = false;

        public ACSGooglePlayTangle(string googlePlayPublicKey)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(googlePlayPublicKey);
                order = new int[bytes.Length / 20 + 1];
                data = TangleObfuscator.Obfuscate(bytes, order, out int rawKey);
                key = rawKey;
                _prepared = data.Length != 0;
            }
            catch (Exception e)
            {
                Debug.LogError("Invalid Google Play Public Key. Generating incomplete credentials file. " + e + "" +
                               "\n -The Google Play License Key is invalid. GooglePlayTangle was generated with incomplete credentials.");
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
