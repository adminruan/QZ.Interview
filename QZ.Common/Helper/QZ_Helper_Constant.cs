using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Common
{
    public static class QZ_Helper_Constant
    {
        /// <summary>
        /// 链接字符
        /// </summary>
        public static readonly string Interview_Conn = QZ_Helper_ConfigHelper.GetSectionValue("ConnectionStrings:Interview_Conn");
        /// <summary>
        /// 是否记录全局日志 1：记录 否则不记录
        /// </summary>
        public static readonly string isErrorLog = QZ_Helper_ConfigHelper.GetSectionValue("StymeSetup:IsErrorLog");
        /// <summary>
        /// 是否加密数据库连接字符串 1：加密，0：不加密
        /// </summary>
        public static readonly string isPwd = QZ_Helper_ConfigHelper.GetSectionValue("StymeSetup:IsPwd");
        /// <summary>
        /// 页面解密路径Key
        /// </summary>
        public static readonly string passWordKey = QZ_Helper_ConfigHelper.GetSectionValue("StymeSetup:PassWordKey");
    }
}
