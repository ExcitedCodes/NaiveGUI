using System.Text;
using System.Security.Cryptography;

namespace NaiveGUI
{
    public class Utils
    {
        public static string Md5(byte[] data)
        {
            try
            {
                StringBuilder Result = new StringBuilder();
                foreach(byte Temp in new MD5CryptoServiceProvider().ComputeHash(data))
                {
                    if(Temp < 16)
                    {
                        Result.Append("0");
                        Result.Append(Temp.ToString("x"));
                    }
                    else
                    {
                        Result.Append(Temp.ToString("x"));
                    }
                }
                return Result.ToString();
            }
            catch
            {
                return "0000000000000000";
            }
        }

        public static string Md5(string Data) => Md5(EncodeByteArray(Data));

        public static byte[] EncodeByteArray(string data)
        {
            return data == null ? null : Encoding.UTF8.GetBytes(data);
        }
    }
}
