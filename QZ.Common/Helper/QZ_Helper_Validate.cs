using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QZ.Common
{
    public class QZ_Helper_Validate
    {
        /// <summary>
        /// 根据参数生成签名
        /// </summary>
        /// <param name="parameters">多少参数</param>
        /// <param name="secret">appkey</param>
        /// <returns></returns>
        public static string GetSignature(IDictionary<string, string> parameters, string secret, out string baseString)
        {
            //先将参数以其参数名的字典序升序进行排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            var arrKeys = sortedParams.Keys.ToArray();
            Array.Sort(arrKeys, string.CompareOrdinal);//ASCII码从小到大排序

            //遍历排序后的字典，将所有参数按"key=value"格式拼接在一起
            StringBuilder basestring = new StringBuilder();
            foreach (var key in arrKeys)
            {
                string value = sortedParams[key];
                if (!string.IsNullOrWhiteSpace(key.Trim()) && !string.IsNullOrEmpty(value))
                {
                    if (key.Equals("openid", StringComparison.OrdinalIgnoreCase))
                    {
                        basestring.Append(key).Append("=").Append(value.Replace(" ", "+"));
                    }
                    else if (key.Equals("unionid", StringComparison.OrdinalIgnoreCase))
                    {
                        basestring.Append(key).Append("=").Append(value.Replace(" ", "+"));
                    }
                    else if (key.Equals("encryptedData", StringComparison.OrdinalIgnoreCase))
                    {
                        basestring.Append(key).Append("=").Append(value.Replace(" ", "+"));
                    }
                    else if (key.Equals("UserToken", StringComparison.OrdinalIgnoreCase))
                    {
                        basestring.Append(key).Append("=").Append(value.Replace(" ", "+"));
                    }
                    else if (value.Equals("undefined", StringComparison.OrdinalIgnoreCase))
                    {
                        //undefined不参与签名
                        continue;
                    }
                    else
                    {
                        basestring.Append(key).Append("=").Append(value); //Replace("", "+")
                    }

                }
            }

            basestring.Append(secret);
            System.Security.Cryptography.MD5 md5__1 = System.Security.Cryptography.MD5.Create();
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(basestring.ToString());
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                baseString = basestring.ToString();
                return ret.ToLower();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 根据参数生成签名(undefined不参与签名)
        /// </summary>
        /// <param name="parameters">多少参数</param>
        /// <param name="secret">appkey</param>
        /// <returns></returns>
        public static string GetSignatureNoUndefined(IDictionary<string, string> parameters, string secret, out string baseString)
        {
            //先将参数以其参数名的字典序升序进行排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            var arrKeys = sortedParams.Keys.ToArray();
            Array.Sort(arrKeys, string.CompareOrdinal);//ASCII码从小到大排序

            //遍历排序后的字典，将所有参数按"key=value"格式拼接在一起
            StringBuilder basestring = new StringBuilder();
            foreach (var key in arrKeys)
            {
                string value = sortedParams[key];
                if (!string.IsNullOrWhiteSpace(key.Trim()) && !string.IsNullOrEmpty(value))
                {
                    if (key.Equals("openid", StringComparison.OrdinalIgnoreCase))
                    {
                        basestring.Append(key).Append("=").Append(value.Replace(" ", "+"));
                    }
                    else if (key.Equals("unionid", StringComparison.OrdinalIgnoreCase))
                    {
                        basestring.Append(key).Append("=").Append(value.Replace(" ", "+"));
                    }
                    else if (key.Equals("encryptedData", StringComparison.OrdinalIgnoreCase))
                    {
                        basestring.Append(key).Append("=").Append(value.Replace(" ", "+"));
                    }
                    else if (key.Equals("UserToken", StringComparison.OrdinalIgnoreCase))
                    {
                        basestring.Append(key).Append("=").Append(value.Replace(" ", "+"));
                    }
                    else if (key.Equals("pwd", StringComparison.OrdinalIgnoreCase))
                    {
                        basestring.Append(key).Append("=").Append(value.Replace(" ", "+"));
                    }
                    else if (key.Equals("password", StringComparison.OrdinalIgnoreCase))
                    {
                        basestring.Append(key).Append("=").Append(value.Replace(" ", "+"));
                    }
                    else if (key.Equals("newpwd", StringComparison.OrdinalIgnoreCase))
                    {
                        basestring.Append(key).Append("=").Append(value.Replace(" ", "+"));
                    }
                    else if (value.Equals("undefined", StringComparison.OrdinalIgnoreCase))
                    {
                        //undefined不参与签名
                        continue;
                    }
                    else
                    {
                        basestring.Append(key).Append("=").Append(value); //Replace("", "+")
                    }

                }
            }

            basestring.Append(secret);
            System.Security.Cryptography.MD5 md5__1 = System.Security.Cryptography.MD5.Create();
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(basestring.ToString());
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                baseString = basestring.ToString();
                return ret.ToLower();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 根据参数生成签名(undefined不参与签名)
        /// </summary>
        /// <param name="parameters">多少参数</param>
        /// <param name="secret">appkey</param>
        /// <returns></returns>
        public static string GetSignatureNoReplace(IDictionary<string, string> parameters, string secret, out string baseString)
        {
            //先将参数以其参数名的字典序升序进行排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            var arrKeys = sortedParams.Keys.ToArray();
            Array.Sort(arrKeys, string.CompareOrdinal);//ASCII码从小到大排序

            //遍历排序后的字典，将所有参数按"key=value"格式拼接在一起
            StringBuilder basestring = new StringBuilder();
            foreach (var key in arrKeys)
            {
                string value = sortedParams[key];
                if (!string.IsNullOrWhiteSpace(key.Trim()) && !string.IsNullOrEmpty(value))
                {
                    if (value.Equals("undefined", StringComparison.OrdinalIgnoreCase))
                    {
                        //undefined不参与签名
                        continue;
                    }
                    else
                    {
                        basestring.Append(key).Append("=").Append(value); //Replace("", "+")
                    }

                }
            }

            basestring.Append(secret);
            System.Security.Cryptography.MD5 md5__1 = System.Security.Cryptography.MD5.Create();
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(basestring.ToString());
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                baseString = basestring.ToString();
                return ret.ToLower();
            }
            catch
            {
                throw;
            }
        }
    }
}
