using UnityEngine.Purchasing.Security;

namespace UnityPurchasing.generated {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("");
        private static int[] order = new int[] { 0 };
        private static int key = 237;

        public static readonly bool IsPopulated = false;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
