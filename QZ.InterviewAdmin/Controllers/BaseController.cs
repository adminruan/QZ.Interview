using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using QZ.Common;
using QZ.Model.Interview;

namespace QZ.InterviewAdmin.Controllers
{
    //[ApiController]
    public class BaseController : Controller
    {
        /// <summary>
        /// 响应类型枚举
        /// </summary>
        public enum EnumResponseCode
        {
            /// <summary>
            /// 失败
            /// </summary>
            [Description("失败")]
            Error = 0,

            /// <summary>
            /// 成功
            /// </summary>
            [Description("成功")]
            Success = 1
        }

        #region 初始分页参数
        protected void InitPageParameters(ref int page, ref int limit)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 20 : limit;
        }
        #endregion

        #region 获取当前登录的用户信息
        /// <summary>
        /// 获取登录的用户信息
        /// true：成功
        /// </summary>
        /// <param name="adminInfo">当前登录的用户信息</param>
        /// <returns></returns>
        protected virtual bool ValidUser(out QZ_Model_In_AdminInfo adminInfo)
        {
            QZ_Model_In_AdminInfo Admin = null;
            adminInfo = null;
            string cookieStr = QZ.Common.QZ_Helper_CookieHelper.ReadCookie(this.HttpContext, "CurrentUser");
            if (string.IsNullOrWhiteSpace(cookieStr))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(cookieStr))
            {
                Admin = JsonConvert.DeserializeObject<QZ_Model_In_AdminInfo>(cookieStr);
            }
            if (Admin == null || Admin.AdminID <= 0)
                return false;
            else
            {
                adminInfo = Admin;
                return true;
            }
        }
        #endregion

        #region 返回数据
        /// <summary>
        /// 返回响应数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual IActionResult ContentResult(object obj)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            string json = JsonConvert.SerializeObject(obj, timeConverter);
            return Content(json);
        }

        /// <summary>
        /// 返回提示信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual IActionResult ContentTips(EnumResponseCode code, string message = "")
        {
            var json = new
            {
                code = (int)code,
                message = string.IsNullOrWhiteSpace(message) ? code.GetEnumDescription() : message
            };
            return Json(json);
        }
        #endregion
        #region 判断是数字还是文字
        public bool IsNumeric(string str) //接收一个string类型的参数,保存到str里
        {
            if (str == null || str.Length == 0)    //验证这个参数是否为空
                return false;                           //是，就返回False
            ASCIIEncoding ascii = new ASCIIEncoding();//new ASCIIEncoding 的实例
            byte[] bytestr = ascii.GetBytes(str);         //把string类型的参数保存到数组里

            foreach (byte c in bytestr)                   //遍历这个数组里的内容
            {
                if (c < 48 || c > 57)                          //判断是否为数字
                {
                    return false;                              //不是，就返回False
                }
            }
            return true;                                        //是，就返回True
        }
        #endregion
    }
}