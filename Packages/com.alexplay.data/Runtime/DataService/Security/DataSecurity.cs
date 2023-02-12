using System.Text;
using ACS.Data.DataService.Config;

namespace ACS.Data.DataService.Security
{
    public class DataSecurity
    {
        private int _cryptoSalt = 33;
        private bool _ignoreCrypt;
        
        public DataSecurity(bool ignoreCrypt = false) => _ignoreCrypt = ignoreCrypt;

        public string Encrypt(string data) => Encoding.UTF8.GetString(RosAlgorithm(Encoding.UTF8.GetBytes(data)));

        public string Decrypt(string data) => Encoding.UTF8.GetString(RosAlgorithm(Encoding.UTF8.GetBytes(data)));

        private byte[] RosAlgorithm(byte[] bytes)
        {
            if (!_ignoreCrypt)
            {
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] = (byte) (Crypt(bytes[i]));
            }

            return bytes;
        }

        private int Crypt(int input) => input ^ _cryptoSalt;
    }
}
