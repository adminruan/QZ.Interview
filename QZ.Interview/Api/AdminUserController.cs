﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QZ.Common;
using QZ.Common.Enums;
using QZ.Interface.Interview_IService;
using QZ.Interview.Api.Bases;
using QZ.Model.Interview;

namespace QZ.Interview.Api
{
    /// <summary>
    /// 管理员信息相关
    /// </summary>
    [Route("api/AdminUser/[action]")]
    [ApiController]
    public class AdminUserController : InterviewControllerBase
    {
        public AdminUserController(QZ_In_IAdminInfoService iAdminInfoService) : base(iAdminInfoService)
        {
            this._iAdminInfoService = iAdminInfoService;
        }
        private readonly QZ_In_IAdminInfoService _iAdminInfoService;

        #region 管理员登录
        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="userName">用户名或手机号码</param>
        /// <param name="password">登录密码</param>
        /// <returns></returns>
        public JsonResult SignIn(string UserName, string Password)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                return base.Write(EnumResponseCode.Error, "用户名或密码有误");
            }
            QZ_Model_In_AdminInfo adminInfo = _iAdminInfoService.GetUserByUserNameOrMobile(UserName);
            if (adminInfo == null)
            {
                return base.Write(EnumResponseCode.Error, "用户名或密码有误");
            }
            if (adminInfo.Status != 0)
            {
                return base.Write(EnumResponseCode.Error, "账号异常，请联系管理员");
            }
            if (adminInfo.Password.ToLower() != QZ_Helper_Encryption.Get32MD5String(Password).ToLower())
            {
                return base.Write(EnumResponseCode.Error, "用户名或密码有误");
            }
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("adminID", adminInfo.AdminID.ToString());
            pairs.Add("realName", adminInfo.RealName);
            pairs.Add("mobile", adminInfo.Mobile);
            pairs.Add("userName", adminInfo.UserName);
            pairs.Add("position", adminInfo.Position.ToString());
            pairs.Add("positionName", ((QZ_Enum_Positions)adminInfo.Position).GetEnumDescription());
            pairs.Add("createTime", adminInfo.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            pairs.Add("adminToken", _iAdminInfoService.GetAdminUserToken(adminInfo));
            return base.Write(EnumResponseCode.Success, data: pairs);
        }
        #endregion
    }
}