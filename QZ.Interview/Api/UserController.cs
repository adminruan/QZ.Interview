using QZ.Interview.Api.Bases;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QZ.Common;
using QZ.Model.Expand.Wechat;
using QZ.Model.Interview;
using QZ.Interface.Interview_IService;
using QZ.Model.Expand;
using Newtonsoft.Json;
using QZ.Common.Enums;
using System.Linq;

namespace QZ.Interview.Api
{
    [Route("api/User/[action]")]
    //[ApiController]
    public class UserController : InterviewControllerBase
    {
        public UserController(QZ_In_IUserService iUserService, QZ_In_IUserBasicInfoService iUserBasicInfoService, QZ_In_IInterviewRecordsService iInterviewRecordsService,
            QZ_In_IAdminInfoService iAdminInfoService, QZ_In_IPositionsService iPositionsService) : base(iUserService)
        {
            this._iUserService = iUserService;
            this._iUserBasicInfoService = iUserBasicInfoService;
            this._iInterviewRecordsService = iInterviewRecordsService;
            this._iAdminInfoService = iAdminInfoService;
            this._iPositionsService = iPositionsService;
        }
        /// <summary>
        /// 面试小程序APPID
        /// </summary>
        private readonly static string _APPID = "wx2f1f506db7ae8667";
        /// <summary>
        /// 面试小程序APPSECRET
        /// </summary>
        private readonly static string _APPSECRET = "845e0898515eba9fb760b13398fcd5e1";
        private readonly object _obj = new object();
        private readonly QZ_In_IUserService _iUserService;
        private readonly QZ_In_IUserBasicInfoService _iUserBasicInfoService;
        private readonly QZ_In_IInterviewRecordsService _iInterviewRecordsService;
        private readonly QZ_In_IAdminInfoService _iAdminInfoService;
        private readonly QZ_In_IPositionsService _iPositionsService;

        #region 普通用户登录、注册
        /// <summary>
        /// 普通用户登录注册
        /// </summary>
        /// <param name="EncryptedData">加密后的用户信息</param>
        /// <param name="Code">微信一次性凭证</param>
        /// <param name="IV">解密填充</param>
        /// <param name="Source">来源</param>
        /// <returns></returns>
        [NotSignVerify]
        public JsonResult MinProgramAuthority(string EncryptedData, string Code, string IV, string Source)
        {
            if (string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(EncryptedData) || string.IsNullOrEmpty(IV) || string.IsNullOrWhiteSpace(Source))
            {
                return base.Write(EnumResponseCode.Error, "参数有误");
            }
            lock (_obj)
            {
                string wxJson = string.Empty;
                wxJson = QZ_Helper_Wechat.GetAppSessionKey(_APPID, _APPSECRET, Code);
                if (string.IsNullOrWhiteSpace(wxJson))
                {
                    return base.Write(EnumResponseCode.Error, "未获取到微信Session_Key信息");
                }
                if (!wxJson.Contains("openid"))
                {
                    return base.Write(EnumResponseCode.Error, "Code有误或已失效");
                }
                WeChat_Model_XCXLoginApiJson wxModel = QZ_Helper_Json.Deserialize<WeChat_Model_XCXLoginApiJson>(wxJson);
                WeChat_Model_WXXCXUserInfo wxUserInfo = null;
                try
                {
                    string wxUserInfoStr = QZ.Common.QZ_Helper_Encryption.AES_Decrypt(EncryptedData, wxModel.session_key, IV);//解密后微信用户信息
                    wxUserInfo = QZ.Common.QZ_Helper_Json.Deserialize<WeChat_Model_WXXCXUserInfo>(wxUserInfoStr);
                }
                catch (Exception e)
                {
                    return base.Write(EnumResponseCode.Error, e.Message);
                }
                if (wxUserInfo == null)
                {
                    return base.Write(EnumResponseCode.Error, "EncryptedData解密失败");
                }

                QZ_Model_In_User userInfo = null;
                if (!string.IsNullOrWhiteSpace(wxModel.unionid))
                {
                    userInfo = _iUserService.GetUserByUnionID(wxModel.unionid);
                }
                else
                {
                    userInfo = _iUserService.GetUserByOpenID(wxModel.openid);
                }
                if (userInfo == null)
                {
                    //注册
                    userInfo = new QZ_Model_In_User();
                    userInfo.OpenID = wxUserInfo.openId;
                    userInfo.UnionID = wxUserInfo.unionId;
                    userInfo.NickName = wxUserInfo.nickName;
                    userInfo.AvatarUrl = wxUserInfo.avatarUrl;
                    switch (wxUserInfo.gender)
                    {
                        case 1:
                            userInfo.Gender = true;
                            break;
                        case 2:
                            userInfo.Gender = false;
                            break;
                        default:
                            break;
                    }
                    userInfo.Country = wxUserInfo.country;
                    userInfo.Province = wxUserInfo.province;
                    userInfo.City = wxUserInfo.city;
                    userInfo.AddTime = DateTime.Now;
                    if (_iUserService.Insert(userInfo).UserID < 1)
                    {
                        return base.Write(EnumResponseCode.Error, "注册失败");
                    }
                }
                else
                {
                    //登录
                    userInfo.OpenID = wxUserInfo.openId;
                    userInfo.UnionID = wxUserInfo.unionId;
                    userInfo.NickName = wxUserInfo.nickName;
                    userInfo.AvatarUrl = wxUserInfo.avatarUrl;
                    switch (wxUserInfo.gender)
                    {
                        case 1:
                            userInfo.Gender = true;
                            break;
                        case 2:
                            userInfo.Gender = false;
                            break;
                        default:
                            break;
                    }
                    userInfo.Country = wxUserInfo.country;
                    userInfo.Province = wxUserInfo.province;
                    userInfo.City = wxUserInfo.city;
                    _iUserService.Update(userInfo);
                }
                Dictionary<string, string> pairs = new Dictionary<string, string>();
                pairs.Add("userID", userInfo.UserID.ToString());
                pairs.Add("nickName", userInfo.NickName);
                pairs.Add("avatarUrl", userInfo.AvatarUrl);
                pairs.Add("gender", userInfo.Gender == null ? "未知" : userInfo.Gender == true ? "男" : "女");
                pairs.Add("regDate", userInfo.AddTime.ToString("yyyy-MM-dd HH:mm:ss"));
                pairs.Add("userToken", _iUserService.GetUserToken(userInfo));
                return base.Write(EnumResponseCode.Success);
            }
        }
        #endregion

        #region 获取我的基本信息等
        public JsonResult MyInfos(int UserID, string UserToken)
        {
            if (!base.ValidUser(UserID, UserToken, out QZ_Model_In_User userInfo))
            {
                return base.Write(EnumResponseCode.NotSignIn);
            }
            QZ_Model_In_UserBasicInfo basicInfo = _iUserBasicInfoService.GetBasicInfo(UserID);
            if (basicInfo == null)
            {
                return base.Write(EnumResponseCode.Error, "暂无信息");
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(basicInfo.Educations))
                {
                    List<Interview_UserEducation> educations = JsonConvert.DeserializeObject<List<Interview_UserEducation>>(basicInfo.Educations);
                    basicInfo.ExtEducations = educations;
                }
                if (!string.IsNullOrWhiteSpace(basicInfo.Jobs))
                {
                    List<Interview_UserHistoryJob> jobs = JsonConvert.DeserializeObject<List<Interview_UserHistoryJob>>(basicInfo.Jobs);
                    basicInfo.ExtJobs = jobs;
                }
            }
            catch (Exception)
            {
            }
            return base.Write(basicInfo, "ID|Educations|Jobs|ExtInterviewID|ExtInterviewDate|ExtSchedule|ExtAdminIds", false);
        }
        #endregion

        #region 提交我的基本信息等
        /// <summary>
        /// 提交我的基本信息等
        /// </summary>
        /// <param name="model">提交信息</param>
        /// <returns></returns>
        [HttpPost]
        [NotSignVerify]
        public JsonResult SubmitBasicInfo([FromBody]Interview_UserBasicInfo model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return base.Write(EnumResponseCode.Error);
            }
            if (!base.ValidUser(model.UserID, model.UserToken, out QZ_Model_In_User userInfo))
            {
                return base.Write(EnumResponseCode.NotSignIn);
            }
            if (!string.IsNullOrWhiteSpace(model.IdentityNumber) && !QZ_Helper_RegularRegex.CheckIdentity(model.IdentityNumber))
            {
                return base.Write(EnumResponseCode.Error, "请输入有效证件号~");
            }
            if (!QZ_Helper_RegularRegex.CheckPhoneNumber(model.Moblie))
            {
                return base.Write(EnumResponseCode.Error, "请输入有效手机号码~");
            }
            if (string.IsNullOrEmpty(model.EducationsJson))
            {
                return base.Write(EnumResponseCode.Error, "请输入教育经历");
            }
            else
            {
                try
                {
                    model.Educations = JsonConvert.DeserializeObject<List<Interview_UserEducation>>(model.EducationsJson);
                }
                catch (Exception)
                {
                    return base.Write(EnumResponseCode.Error, "教育经历有误");
                }
            }
            if (string.IsNullOrWhiteSpace(model.JobsJson))
            {
                try
                {
                    model.Jobs = JsonConvert.DeserializeObject<List<Interview_UserHistoryJob>>(model.JobsJson);
                }
                catch (Exception)
                {
                    return base.Write(EnumResponseCode.Error, "工作经历有误");
                }
            }
            //model.EducationsJson = model.Educations != null ? JsonConvert.SerializeObject(model.Educations) : string.Empty;
            //model.JobsJson = model.Jobs != null ? JsonConvert.SerializeObject(model.Jobs) : string.Empty;
            if (!_iUserBasicInfoService.SubmitBasicInfo(model, out QZ_Model_In_UserBasicInfo basicInfo))
            {
                return base.Write(EnumResponseCode.Error, "哦噢，遇到了未知错误~");
            }
            var admins = _iAdminInfoService.Query<QZ_Model_In_AdminInfo>(p => p.Position == (int)QZ_Enum_Positions.Administrative && p.Status == 0 && !string.IsNullOrWhiteSpace(p.OpenID)).Select(p => new { p.OpenID, p.AdminID }).ToList();
            if (admins == null || admins.Count == 0)
            {
                return base.Write(EnumResponseCode.Error, "无可用行政信息");
            }
            //用户不存在还在处理中的面试，则自动给用户提交面试信息
            if (!_iInterviewRecordsService.Any<QZ_Model_In_InterviewRecords>(p => p.UserID == model.UserID && p.Schedule <= (int)QZ_Enum_Schedules.PendingApproval)
                && !_iInterviewRecordsService.SubmitInterviewRecord(model.UserID, admins.First().AdminID, model.ApplyJob))
            {
                return base.Write(EnumResponseCode.Error, "面试申请提交失败");
            }
            //给人事发送微信公众号消息通知
            if (QZ_Helper_Wechat.GetAccessToken(out string accessToken, QZ_Helper_Constant.PartnerAPPID, QZ_Helper_Constant.PartnerAPPSecret))
            {
                List<QZ_Model_In_Positions> positions = _iPositionsService.GetPositions() ?? new List<QZ_Model_In_Positions>();
                foreach (var item in admins)
                {
                    Dictionary<string, string> pairs = new Dictionary<string, string>();
                    pairs.Add("first", "您收到一封新简历");
                    pairs.Add("keyword1", $"{model.RealName}");
                    Interview_UserEducation deucationInfo = model.Educations.OrderByDescending(p => p.AdmissionTime).FirstOrDefault();
                    pairs.Add("keyword2", $"{deucationInfo?.School ?? "-"}");
                    pairs.Add("keyword3", $"{deucationInfo?.Major ?? "-"}");
                    pairs.Add("keyword4", $"{model.Education}");
                    pairs.Add("keyword5", $"{positions.FirstOrDefault(p => p.ID == model.ApplyJob)?.PositionName ?? "-"}");
                    pairs.Add("remark", "点击详情，分配面试");
                    QZ_Helper_Wechat.SendTemplateMessage(accessToken, QZ_Helper_Wechat.AwaitInterviewTemplate(item.OpenID, pairs, QZ_Helper_Constant.PartnerAPPID, "", type: 1));
                }
            }

            return base.Write(EnumResponseCode.Success, "提交成功");
        }
        #endregion

        #region 用户开通面试进度提醒二维码
        public JsonResult SchduleQRCode(int UserID, string UserToken)
        {
            if (!base.ValidUser(UserID, UserToken, out QZ_Model_In_User userInfo))
            {
                return base.Write(EnumResponseCode.NotSignIn);
            }
            //使用题多多合伙人appid和appsecret
            if (!QZ_Helper_Wechat.GetAccessToken(out string accessToken, "wx0d13958f771fb415", "b4a9317fc2f33f915a4f154e68e6050b"))
            {
                return base.Write(EnumResponseCode.Error, "AccessToken 获取异常");
            }
            //获取生成二维码票据
            try
            {
                string ticket = QZ_Helper_Wechat.GetQRCodeTicket(accessToken, UserID.ToString());
                if (string.IsNullOrWhiteSpace(ticket))
                {
                    return base.Write(EnumResponseCode.Error, "Ticket 获取异常");
                }
                //获取二维码的地址，该请求返回为图片文件
                string qrCodeUrl = $"https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={Uri.EscapeDataString(ticket)}";
                return base.Write(EnumResponseCode.Success, "成功", new Dictionary<string, string>() { { "qrCodeUrl", qrCodeUrl } });
            }
            catch (Exception e)
            {
                return base.Write(EnumResponseCode.Error, "位置异常", new Dictionary<string, string>() { { "errMsg", e.Message } });
            }
        }
        #endregion

        #region 绑定用户在中公众号OpenID
        /// <summary>
        /// 绑定用户在中公众号OpenID
        /// </summary>
        /// <param name="OffOpenID">公众号openid</param>
        /// <param name="UID">小程序中对应用户ID</param>
        /// <returns></returns>
        [NotSignVerify]
        public JsonResult BindOfficialAccountOpenID(string OffOpenID, int UID)
        {
            if (string.IsNullOrWhiteSpace(OffOpenID) || UID < 1)
            {
                return base.Write(EnumResponseCode.Error, "请求参数有误");
            }
            if (!_iUserService.Any<QZ_Model_In_User>(p => p.UserID == UID))
            {
                return base.Write(EnumResponseCode.Error, "未找到用户信息");
            }
            if (!_iUserService.BindOfficialOpenID(UID, OffOpenID))
            {
                return base.Write(EnumResponseCode.Error, "开通消息提醒失败，请重试扫码开通");
            }
            return base.Write(EnumResponseCode.Success, "开通成功");
        }
        #endregion
    }
}
