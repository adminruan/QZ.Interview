using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Common
{
    public static class QZ_Helper_Json
    {
        /// <summary>  
        /// 反序列化
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="json"></param>  
        /// <returns></returns>  
        public static T Deserialize<T>(this string json)
        {
            T obj = Activator.CreateInstance<T>();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
