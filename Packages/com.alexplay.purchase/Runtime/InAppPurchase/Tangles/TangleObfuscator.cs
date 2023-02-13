using System;
using System.Linq;
using UnityEngine;

namespace ACS.IAP.InAppPurchase.Tangles
{
    public class TangleObfuscator
    {
        private class InvalidOrderArray : Exception
        {
            public InvalidOrderArray(string e) => Debug.LogError(e);
        }

        public byte[] Obfuscate(byte[] data, int[] order, out int rkey)
        {
            var rnd = new System.Random();
            int key = rnd.Next(2, 255);
            byte[] res = new byte[data.Length];
            int slices = data.Length / 20 + 1;

            if (order == null || order.Length < slices)
                throw new InvalidOrderArray("Seem like your order is null or lenght lower than number of slice");

            Array.Copy(data, res, data.Length);
            for (int i = 0; i < slices - 1; i++)
            {
                int j = rnd.Next(i, slices - 1);
                order[i] = j;
                int sliceSize = 20;
                var tmp = res.Skip(i * 20).Take(sliceSize).ToArray();
                Array.Copy(res, j * 20, res, i * 20, sliceSize);
                Array.Copy(tmp, 0, res, j * 20, sliceSize);
            }
            order[slices - 1] = slices - 1;

            rkey = key;
            return res.Select<byte, byte>(x => (byte)(x ^ key)).ToArray();
        }
    }
}