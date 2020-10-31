using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QZ.Common
{
    public static class QZ_Helper_IP
    {
        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientUserIp(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }

        /// <summary>
        /// 获取服务端IP
        /// </summary>
        /// <returns></returns>
        public static string GetServiceIP()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
    .Select(p => p.GetIPProperties())
    .SelectMany(p => p.UnicastAddresses)
    .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address) && p.Address.ToString().Contains("192.168.1."))
    .FirstOrDefault()?.Address.ToString();
        }
    }
}
