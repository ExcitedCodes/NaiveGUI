using System;
using System.IO;
using System.Net;
using System.Text;
using System.Reflection;
using System.Security.Cryptography;

namespace NaiveGUI
{
    public class Utils
    {
        public static string DefaultUserAgent = "NaiveGUI/" + Assembly.GetExecutingAssembly().GetName().Version + " (Potato NT) not AppleWebKit (not KHTML, not like Gecko) not Chrome not Safari";

        public static string HttpGetString(string url, Encoding encoding = null, int timeoutMs = -1, bool redirect = false, IWebProxy proxy = null)
        {
            if(encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetString(HttpGetBytes(url, timeoutMs, redirect, proxy));
        }

        public static byte[] HttpGetBytes(string url, int timeoutMs = -1, bool redirect = false, IWebProxy proxy = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if(url.StartsWith("//"))
            {
                url = "https:" + url;
            }
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.AllowAutoRedirect = redirect;
            if(proxy != null)
            {
                request.Proxy = proxy;
            }
            if(timeoutMs > 0)
            {
                request.Timeout = timeoutMs;
            }
            using(var response = request.GetResponse() as HttpWebResponse)
            {
                if(response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Bad HTTP Status(" + url + "):" + response.StatusCode + " " + response.StatusDescription);
                }
                using(var ms = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

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

        public static byte[] EncodeByteArray(string data) => data == null ? null : Encoding.UTF8.GetBytes(data);
    }
}
