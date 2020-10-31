using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace QZ.Common
{
    public class QZ_Helper_HttpMethods
    {
        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static string HttpGet(string url, int Timeout = 15000)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Proxy = null;
            request.Timeout = Timeout;
            request.AllowAutoRedirect = false;
            request.KeepAlive = true;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                return "500";
            }
            finally
            {
                request = null;
                response = null;
            }

            return responseStr;
        }

        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="data">请求数据</param>
        /// <param name="timeOut">请求超时时间 单位/毫秒</param>
        /// <param name="contentType">请求数据内容类型</param>
        /// <returns></returns>
        public static string HttpPost(string url, string data, int timeOut = 1500, string contentType = "application/json;charset=UTF-8")
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = contentType;
            request.Accept = "*/*";
            request.Proxy = null;
            request.Timeout = timeOut;
            request.AllowAutoRedirect = false;

            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(data);
                requestStream.Close();

                try
                {
                    response = request.GetResponse();
                }
                catch (WebException err)
                {
                    response = (HttpWebResponse)err.Response;
                }

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }

            return responseStr;
        }
    }
}
