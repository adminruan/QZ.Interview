using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using QZ.Common;
using QZ.Interface.Interview_IService;
using QZ.Model.Interview;

namespace QZ.InterviewAdmin.Controllers
{
    public class LoginController : Controller
    {
        public LoginController(QZ_In_IAdminInfoService iAdminInfoService)
        {
            this._iAdminInfoService = iAdminInfoService;
        } 
        public readonly QZ_In_IAdminInfoService _iAdminInfoService;

        #region 登录首页
        /// <summary>
        /// 登录首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            try
            {
                string stradmin = QZ_Helper_CookieHelper.ReadCookie(this.HttpContext, "CurrentUser");
                if (!string.IsNullOrEmpty(stradmin))
                {
                    QZ_Model_In_AdminInfo Admin = JsonConvert.DeserializeObject<QZ_Model_In_AdminInfo>(stradmin);
                    if (Admin != null)
                    {
                        Admin = ValidUser(Admin.UserName, Admin.Password, out string msg);
                        if (Admin != null)
                        {
                            //如果验证成功了就序列化对象并保存进cookie  并跳转
                            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                            string result = JsonConvert.SerializeObject(Admin, timeFormat);
                            QZ_Helper_CookieHelper.WriteCookie(this.HttpContext, "CurrentUser", result, 1);

                            return Redirect("/Main/Index");
                        }
                    }
                }
                //如果验证失败就清理掉Cookie
                QZ_Helper_CookieHelper.DeleteCookie(this.HttpContext, "CurrentUser");
            }
            catch (Exception ex)
            {
                ViewBag.LogMsg = ex.Message;
            }
            return View();
        }
        #endregion

        #region 登录校验
        public JsonResult SignIn(string userName, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    return Json(new { code = 0, msg = "请输入有效帐号密码" });
                }
                QZ_Model_In_AdminInfo adminInfo = ValidUser(userName, password, out string msg);
                if (adminInfo == null)
                {
                    return Json(new { code = 0, msg = msg });
                }
                //如果验证成功了就序列化对象并保存进cookie  并跳转
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                string result = JsonConvert.SerializeObject(adminInfo, timeFormat);
                QZ_Helper_CookieHelper.WriteCookie(this.HttpContext, "CurrentUser", result, 1);
                return Json(new { code = 1, msg = "登录成功" });
            }
            catch (Exception e)
            {
                return Json(new { code = 0, msg = $"未知错误，请联系管理员 {e.Message}" });
            }
        }
        #endregion

        /// <summary>
        /// 校验用户名和密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="message">提示信息</param>
        /// <returns></returns>
        private QZ_Model_In_AdminInfo ValidUser(string userName, string password, out string message)
        {
            QZ_Model_In_AdminInfo adminInfo = _iAdminInfoService.GetUserByUserNameOrMobile(userName);
            if (adminInfo == null)
            {
                message = "用户不存在";
                return null;
            }
            if (adminInfo.Password.ToLower() != QZ_Helper_Encryption.Get32MD5String(password).ToLower() && adminInfo.Password.ToLower() != QZ_Helper_Encryption.Get16MD5String(password).ToLower()
                && adminInfo.Password.ToLower() != password.ToLower())
            {
                message = "用户名或密码有误";
                return null;
            }
            if (adminInfo.Status != 0)
            {
                message = "账号已锁定";
                return null;
            }
            message = "登录成功";
            return adminInfo;
        }
    }
}