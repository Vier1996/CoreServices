using System.Text;
using ACS.Data.DataService.Config;

namespace ACS.Data.DataService.Security
{
    public class DataSecurity
    {
        private CryptoKey _key;
        
        public DataSecurity(CryptoKey key)
        {
            _key = key;
        }
        
        public string Encrypt(string data) => Encoding.UTF8.GetString(RosAlgorithm(Encoding.UTF8.GetBytes(data)));

        public string Decrypt(string data) => Encoding.UTF8.GetString(RosAlgorithm(Encoding.UTF8.GetBytes(data)));

        private byte[] RosAlgorithm(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++) 
                bytes[i] = (byte) (Crypt(bytes[i]));

            return bytes;
        }

        private int Crypt(int input) => 
            input ^ _key.CryptoPartA ^ _key.CryptoPartB ^ _key.CryptoPartC ^ _key.CryptoPartD;
    }
}
