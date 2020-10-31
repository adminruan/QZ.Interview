using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Common
{
    public static class QZ_Helper_CookieHelper
    {
        /// <summary>
        /// 获取cookie
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <param name="cookieName">cookie名称</param>
        /// <returns></returns>
        public static string ReadCookie(HttpContext context, string cookieName)
        {
            try
            {
                return System.Net.WebUtility.UrlDecode(context.Request.Cookies[cookieName]);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 写入cookie
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <param name="cookieName">cookie名称</param>
        /// <param name="value">cookie值</param>
        /// <param name="days">有效天数</param>
        public static void WriteCookie(HttpContext context, string cookieName, string value, int days)
        {
            DateTime expires = DateTime.Today.AddDays(days);
            context.Response.Cookies.Append(cookieName, value, new CookieOptions { Expires = expires });
        }

        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <param name="cookieName">cookie名称</param>
        public static void DeleteCookie(HttpContext context, string cookieName)
        {
            context.Response.Cookies.Delete(cookieName);
        }
    }
}
