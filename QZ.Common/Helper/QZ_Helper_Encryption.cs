using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QZ.Common
{
    /// <summary>
    /// 加密/解密操作类
    /// </summary>
    public class QZ_Helper_Encryption
    {
        #region Base64位加密解密
        /// <summary>
        /// 将字符串转换成base64格式,使用UTF8字符集
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <returns></returns>
        public static string Base64Encode(string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return content;
                }
                byte[] bytes = Encoding.UTF8.GetBytes(content);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                return content;
            }
        }
        /// <summary>
        /// 将base64格式，转换utf8
        /// </summary>
        /// <param name="content">解密内容</param>
        /// <returns></returns>
        public static string Base64Decode(string content)
        {
            byte[] bytes = Convert.FromBase64String(content);
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region AES 加密、解密
        /// <summary>
        /// AES 解密操作方法 （适用于解密微信字符串）
        /// </summary>
        /// <param name="encryptedDataStr"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AES_Decrypt(string encryptedDataStr, string key, string iv)
        {
            RijndaelManaged rijalg = new RijndaelManaged();
            //-----------------      
            //设置 cipher 格式 AES-128-CBC      

            rijalg.KeySize = 128;

            rijalg.Padding = PaddingMode.PKCS7;
            rijalg.Mode = CipherMode.CBC;

            try
            {
                rijalg.IV = Convert.FromBase64String(iv.Replace(" ", "+"));
            }
            catch (Exception)
            {
                var imgData = iv;
                //过滤特殊字符即可   
                string dummyData = imgData.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                if (dummyData.Length % 4 > 0)
                {
                    dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                }
                rijalg.IV = Convert.FromBase64String(dummyData);
            }

            try
            {
                rijalg.Key = Convert.FromBase64String(key.Replace(" ", "+"));
            }
            catch (Exception ex)
            {
                var imgData = key;
                //过滤特殊字符即可   
                string dummyData = imgData.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                if (dummyData.Length % 4 > 0)
                {
                    dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                }
                rijalg.Key = Convert.FromBase64String(dummyData);
            }


            byte[] encryptedData = null;
            try
            {
                encryptedData = Convert.FromBase64String(encryptedDataStr.Replace(" ", "+"));
            }
            catch (Exception ex)
            {
                var imgData = encryptedDataStr;
                //过滤特殊字符即可   
                string dummyData = imgData.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                if (dummyData.Length % 4 > 0)
                {
                    dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                }
                encryptedData = Convert.FromBase64String(dummyData);
            }

            ICryptoTransform decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);

            string result;

            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        result = srDecrypt.ReadToEnd();
                    }
                }
            }
            return result;
        }

        //默认密钥向量
        private static byte[] AES_Keys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };


        #endregion

        #region DES加密
        //默认密钥向量
        private static byte[] DES_Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
        public static string DES_Encode(string encryptString, string encryptKey)
        {
            encryptKey = QZ_Helper_StringPlus.GetSubString(encryptKey, 8, "");
            encryptKey = encryptKey.PadRight(8, ' ');
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = DES_Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());

        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string DES_Decode(string decryptString, string decryptKey)
        {
            try
            {
                decryptKey = QZ_Helper_StringPlus.GetSubString(decryptKey, 8, "");
                decryptKey = decryptKey.PadRight(8, ' ');
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = DES_Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();

                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string DES_DecodeWXOpenID(string decryptString, string decryptKey)
        {
            try
            {
                decryptKey = QZ_Helper_StringPlus.GetSubString(decryptKey, 8, "");
                decryptKey = decryptKey.PadRight(8, ' ');
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = { 1, 2, 3, 4, 5, 6, 7, 8 };
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();

                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
        public static string DES_EncodeWXOpenID(string encryptString, string encryptKey)
        {
            encryptKey = QZ_Helper_StringPlus.GetSubString(encryptKey, 8, "");
            encryptKey = encryptKey.PadRight(8, ' ');
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());

        }
        #endregion

        #region MD5加密
        /// <summary>
        /// MD5加密返回32位字符串
        /// </summary>
        /// <param name="source">需要加密的字符串</param>
        /// <returns></returns>
        public static string Get32MD5String(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return null;
            }

            MD5 md5 = MD5.Create();
            byte[] ss = Encoding.Default.GetBytes(source);
            byte[] md5Data = md5.ComputeHash(ss);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Data.Length; i++)
            {
                sb.Append(md5Data[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// MD5加密返回16位字符串
        /// </summary>
        /// <param name="source">需要加密的字符串</param>
        /// <returns></returns>
        public static string Get16MD5String(string source)
        {
            MD5 md5 = MD5.Create();
            byte[] ss = Encoding.Default.GetBytes(source);
            byte[] md5Data = md5.ComputeHash(ss);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Data.Length; i++)
            {
                sb.Append(md5Data[i].ToString("X2"));
            }
            return sb.ToString().Substring(8, 16);
        }
        #endregion
    }
}
