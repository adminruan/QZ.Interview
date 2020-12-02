using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QZ.Common;
using QZ.Common.Enums;
using QZ.Common.Expand;
using QZ.Common.Helper;
using QZ.Interface.Interview_IService;
using QZ.Interview.Api.Bases;
using QZ.Model.Expand;
using QZ.Model.Expand.Interview;
using QZ.Model.Interview;

namespace QZ.Interview.Api
{
    [Route("api/InterviewRecord/[action]")]
    //[ApiController]
    public class InterviewRecordController : InterviewControllerBase
    {

        public InterviewRecordController(QZ_In_IAdminInfoService iAdminInfoService, QZ_In_IInterviewRecordsService iInterviewRecordsService, QZ_In_IPositionsService iPositionsService,
            QZ_In_IUserBasicInfoService iUserBasicInfoService) : base(iAdminInfoService)
        {
            this._iAdminInfoService = iAdminInfoService;
            this._iInterviewRecordsService = iInterviewRecordsService;
            this._iPositionsService = iPositionsService;
            this._iUserBasicInfoService = iUserBasicInfoService;
        }
        private readonly static object _obj = new object();
        private readonly QZ_In_IInterviewRecordsService _iInterviewRecordsService;
        private readonly QZ_In_IAdminInfoService _iAdminInfoService;
        private readonly QZ_In_IPositionsService _iPositionsService;
        private readonly QZ_In_IUserBasicInfoService _iUserBasicInfoService;

        #region 获取面试信息列表
        /// <summary>
        /// 获取面试信息列表
        /// </summary>
        /// <param name="AdminID">管理员ID</param>
        /// <param name="AdminToken">管理员令牌</param>
        /// <param name="Type">类型（True：已处理、False：待处理）</param>
        /// <param name="Page">页码</param>
        /// <param name="Scheduls">已处理的状态类型（301：不合适、302：备用、303：合适）</param>
        /// <returns></returns>
        public JsonResult GetInterviewInfos(int AdminID, string AdminToken, bool Type, int Page, int Limit, int? Scheduls)
        {
            if (!base.ValidAdminUser(AdminID, AdminToken, out QZ_Model_In_AdminInfo adminInfo))
            {
                return base.Write(EnumResponseCode.NotSignIn, "未登录");
            }
            Page = Page < 1 ? 1 : Page;
            Limit = Limit < 1 ? 10 : Limit;
            var data = _iInterviewRecordsService.GetDataInterview();
            if (!data.Any())
            {
                return base.Write(EnumResponseCode.Error, "暂无数据");
            }
            Expression<Func<QZ_Model_In_UserBasicInfo, bool>> awaitExpression = p => true;
            if (!Type)
            {
                //待处理
                switch (adminInfo.Position)
                {
                    case (int)QZ_Enum_Positions.Administrative:
                        {
                            //行政-待分配、需要我处理的面试
                            awaitExpression = awaitExpression.And(p => p.ExtSchedule < (int)QZ_Enum_Schedules.PendingApproval || (p.ExtAdminIds.EndsWith(adminInfo.AdminID + "|") && p.ExtSchedule <= (int)QZ_Enum_Schedules.PendingApproval));
                            data = data.Where(awaitExpression);
                        }
                        break;
                    case (int)QZ_Enum_Positions.Boss:
                        {
                            //总经理-可查看所有权限
                            awaitExpression = awaitExpression.And(p => p.ExtSchedule <= (int)QZ_Enum_Schedules.PendingApproval);
                            data = data.Where(awaitExpression);
                        }
                        break;
                    default:
                        {
                            //其它角色-需要我处理的面试
                            awaitExpression = awaitExpression.And(p => p.ExtAdminIds.EndsWith(adminInfo.AdminID + "|") && p.ExtSchedule <= (int)QZ_Enum_Schedules.PendingApproval);
                            data = data.Where(awaitExpression);
                        }
                        break;
                }
            }
            else
            {
                //已处理
                Expression<Func<QZ_Model_In_UserBasicInfo, bool>> where = p => p.ExtAdminIds.Contains(adminInfo.AdminID.ToString()) && p.ExtSchedule > (int)QZ_Enum_Schedules.PendingApproval;
                if (Scheduls != null)
                {
                    where = where.And(p => p.ExtSchedule == Scheduls);
                }
                data = data.Where(where);
            }
            if (!data.Any())
            {
                return base.Write(EnumResponseCode.Error, "暂无数据");
            }
            List<QZ_Model_In_UserBasicInfo> list = data.Skip((Page - 1) * Limit).Take(Limit).ToList();
            //获取面试人历史记录
            List<int> userIds = list.Select(p => p.UserID).GroupBy(p => p).Select(p => p.Key).ToList();
            List<(int, int)> interviewTimes = _iInterviewRecordsService.GetInterviewTimesByUIDS(userIds);
            list.ForEach(x =>
            {
                int items = interviewTimes.FirstOrDefault(p => p.Item1 == x.UserID).Item2;
                x.ExtInterviewTimes = items == 0 ? 0 : items - 1;
            });

            int totalPage = data.Count().CalculateTotalPageNumber(20);
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            //我的待处理数量、已处理数量
            pairs.Add("pendings", _iInterviewRecordsService.GetDataInterview().Where(awaitExpression).Count().ToString());
            pairs.Add("prosesseds", _iInterviewRecordsService.GetDataInterview().Where(p => p.ExtAdminIds.Contains(adminInfo.AdminID.ToString()) && p.ExtSchedule > (int)QZ_Enum_Schedules.PendingApproval).Count().ToString());
            pairs.Add("page", Page.ToString());
            pairs.Add("limit", "20");
            pairs.Add("totalPages", totalPage.ToString());

            return base.Writes(list, appoints: "ID|RealName|Gender|Age|BirthDate|Education|ApplyJob|ExtInterviewDate|ExtSchedule|ExtInterviewID|ExtAdminIds|ExtInterviewTimes", data: pairs);
        }
        #endregion

        #region 管理员查看简历详情
        /// <summary>
        /// 管理员查看简历详情
        /// </summary>
        /// <param name="AdminID">管理员ID</param>
        /// <param name="AdminToken">管理员令牌</param>
        /// <param name="InterviewID">面试记录ID</param>
        /// <returns></returns>
        public JsonResult GetResumeByInterviewID(int AdminID, string AdminToken, int InterviewID)
        {
            if (!base.ValidAdminUser(AdminID, AdminToken, out QZ_Model_In_AdminInfo adminInfo))
            {
                return base.Write(EnumResponseCode.NotSignIn, "未登录");
            }
            if (InterviewID < 1)
            {
                return base.Write(EnumResponseCode.Error, "请求参数有误");
            }
            QZ_Model_In_InterviewRecords interviewInfo = _iInterviewRecordsService.Find<QZ_Model_In_InterviewRecords>(InterviewID);
            if (interviewInfo == null)
            {
                return base.Write(EnumResponseCode.Error, "未找到面试记录");
            }
            QZ_Model_In_UserBasicInfo basicInfo = _iUserBasicInfoService.GetBasicInfo(interviewInfo.UserID);
            if (basicInfo == null)
            {
                return base.Write(EnumResponseCode.Error, "未找到用户信息");
            }
            if (!string.IsNullOrWhiteSpace(basicInfo.Educations))
            {
                basicInfo.ExtEducations = JsonConvert.DeserializeObject<List<QZ.Model.Expand.Interview_UserEducation>>(basicInfo.Educations);
            }
            if (!string.IsNullOrWhiteSpace(basicInfo.Jobs))
            {
                basicInfo.ExtJobs = JsonConvert.DeserializeObject<List<Interview_UserHistoryJob>>(basicInfo.Jobs);
            }
            if (!string.IsNullOrEmpty(interviewInfo.Remarks))
            {
                basicInfo.ExtRemarkList = JsonConvert.DeserializeObject<List<Interview_InterviewerRemark>>(interviewInfo.Remarks);
            }
            basicInfo.ExtSchedule = interviewInfo.Schedule;
            basicInfo.ExtInterviewID = interviewInfo.ID;
            if (interviewInfo.Schedule == (int)QZ_Enum_Schedules.Qualified)
            {
                basicInfo.ExtRealOffer = interviewInfo.RealOffer ?? 0M;
                basicInfo.ExtTryOffer = interviewInfo.TryOffer ?? 0M;
                basicInfo.ExtEntryTime = interviewInfo.EntryTime;
            }
            return base.Write(basicInfo, "Educations|Jobs|ExtInterviewDate|ExtScheduleText|ExtAdminIds|ExtRemarks|ExtHistoryInterviews|ExtResumeSource|ExtArriveTime|ExtApplyJob|ExtFirstDate|ExtSecondDate|extRemarks", false);
        }
        #endregion

        #region 获取历史面试记录
        public JsonResult HistoryInterviews(int interviewID)
        {
            if (interviewID < 1)
            {
                return Write(EnumResponseCode.Error);
            }
            QZ_Model_In_InterviewRecords interviewInfo = _iInterviewRecordsService.Find<QZ_Model_In_InterviewRecords>(interviewID);
            if (interviewInfo == null)
            {
                return Write(EnumResponseCode.Error, "未找到面试信息");
            }
            List<QZ_Model_In_UserBasicInfo> list = _iInterviewRecordsService.GetDataInterview().ToList().Where(p => p.UserID == interviewInfo.UserID && p.ID != interviewInfo.ID && p.ExtSchedule > (int)QZ_Enum_Schedules.PendingApproval).ToList();
            if (list == null || list.Count == 0)
            {
                return Write(EnumResponseCode.Error, "无历史记录");
            }
            foreach (var item in list)
            {
                if (!string.IsNullOrWhiteSpace(item.Educations))
                {
                    item.ExtEducations = JsonConvert.DeserializeObject<List<QZ.Model.Expand.Interview_UserEducation>>(item.Educations);
                }
                if (!string.IsNullOrWhiteSpace(item.Jobs))
                {
                    item.ExtJobs = JsonConvert.DeserializeObject<List<Interview_UserHistoryJob>>(item.Jobs);
                }
                if (!string.IsNullOrEmpty(item.ExtRemarks))
                {
                    item.ExtRemarkList = JsonConvert.DeserializeObject<List<Interview_InterviewerRemark>>(item.ExtRemarks);
                }
            }
            return Writes(list, "RealName|Gender|Age|BirthDate|Education|ApplyJob|ExtInterviewDate|ExtSchedule|ExtInterviewID|UserID|ExtEducations|ExtJobs|ExtRemarkList");
        }
        #endregion

        #region 变更用户信息相关功能

        #region 更改用户基本信息
        [NotSignVerify]
        public JsonResult AlterUserBasicInfo(Interview_UserBasicInfoNew model)
        {
            if (model == null || model.ID < 1)
            {
                return base.Write(EnumResponseCode.Error, "请求参数有误");
            }
            if (!QZ_Helper_RegularRegex.CheckPhoneNumber(model.Moblie))
            {
                return base.Write(EnumResponseCode.Error, "联系方式有误");
            }
            if (!QZ_Helper_RegularRegex.CheckPhoneNumber(model.EmergencyMobile))
            {
                return base.Write(EnumResponseCode.Error, "紧急联系方式有误");
            }
            if (!_iUserBasicInfoService.Any<QZ_Model_In_UserBasicInfo>(p => p.ID == model.ID))
            {
                return base.Write(EnumResponseCode.Error, "用户信息不存在");
            }
            if (!_iUserBasicInfoService.UpdateBasicInfo(model))
            {
                return base.Write(EnumResponseCode.Error, "保存失败");
            }
            return base.Write(EnumResponseCode.Success, "成功");
        }
        #endregion

        #region 更改用户岗位及薪资要求
        public JsonResult AlterUserApplyJobs(Interview_UserApplyJobs model)
        {
            if (model == null || model.ApplyJob < 1)
            {
                return base.Write(EnumResponseCode.Error, "请求参数有误");
            }
            if (!_iUserBasicInfoService.Any<QZ_Model_In_UserBasicInfo>(p => p.ID == model.ID))
            {
                return base.Write(EnumResponseCode.Error, "用户信息不存在");
            }
            if (!_iUserBasicInfoService.UpdateUserApplyJobs(model))
            {
                return base.Write(EnumResponseCode.Error, "保存失败");
            }
            return base.Write(EnumResponseCode.Success, "成功");
        }
        #endregion

        #region 更改用户教育经历
        [NotSignVerify]
        [HttpPost]
        public JsonResult AlterEducations([FromBody]Interview_UserEducationsNew models)
        {
            if (models == null || models.ID < 1 || models.Educations == null || models.Educations.Count < 1)
            {
                return base.Write(EnumResponseCode.Error, "请求参数有误");
            }
            if (!_iUserBasicInfoService.Any<QZ_Model_In_UserBasicInfo>(p => p.ID == models.ID))
            {
                return base.Write(EnumResponseCode.Error, "用户信息不存在");
            }
            if (!_iUserBasicInfoService.UpdateUserEduactions(models))
            {
                return base.Write(EnumResponseCode.Error, "保存失败");
            }
            return base.Write(EnumResponseCode.Success, "成功");
        }
        #endregion

        #region 更改用户工作经历
        [NotSignVerify]
        [HttpPost]
        public JsonResult AlterUserPastJobs([FromBody]Interview_UserPastJobs models)
        {
            if (models == null || models.ID < 1 || models.UserPastJobs == null || models.UserPastJobs.Count < 1)
            {
                return base.Write(EnumResponseCode.Error, "请求参数有误");
            }
            if (!_iUserBasicInfoService.Any<QZ_Model_In_UserBasicInfo>(p => p.ID == models.ID))
            {
                return base.Write(EnumResponseCode.Error, "用户信息不存在");
            }
            if (!_iUserBasicInfoService.UpdateUserPastJobs(models))
            {
                return base.Write(EnumResponseCode.Error, "保存失败");
            }
            return base.Write(EnumResponseCode.Success, "成功");
        }
        #endregion

        #endregion

        #region 职位信息
        public JsonResult GetPositions()
        {
            List<QZ_Model_In_Positions> positions = _iPositionsService.GetPositions();
            if (positions == null || positions.Count == 0)
            {
                return base.Write(EnumResponseCode.Error, "暂无职位信息");
            }
            return base.Writes<QZ_Model_In_Positions>(positions, appoints: "ID|PositionName");
            //var pairs = QZ_Helper_EnumHelper.ToPairs(typeof(QZ_Enum_Positions));
            //Dictionary<string, string> newPairs = new Dictionary<string, string>();
            //foreach (var item in pairs)
            //{
            //    newPairs.Add(item.Value.ToString(), item.Key);
            //}
            //return base.Write(EnumResponseCode.Success, data: newPairs);
        }
        #endregion

        #region 简历来源平台
        public JsonResult RecruitPlatforms()
        {
            Dictionary<string, int> pairs = QZ_Helper_EnumHelper.ToPairs(typeof(QZ_Enum_RecruitPlatform));
            return base.Writes(pairs.Select(p => new { value = p.Value, name = p.Key }).ToList());
        }
        #endregion

        #region 教育类型
        public JsonResult EducationTeyps()
        {
            Dictionary<string, int> pairs = QZ_Helper_EnumHelper.ToPairs(typeof(QZ_Enum_EducationTypes));
            return base.Writes(pairs.Select(p => new { name = p.Key }).ToList());
        }
        #endregion

        #region 获取管理员信息
        /// <summary>
        /// 获取管理员信息
        /// </summary>
        /// <param name="AdminID">管理员ID</param>
        /// <param name="AdminToken">管理员令牌</param>
        /// <returns></returns>
        public JsonResult GetAdminInfo(int AdminID, string AdminToken)
        {
            if (!base.ValidAdminUser(AdminID, AdminToken, out QZ_Model_In_AdminInfo adminInfo))
            {
                return base.Write(EnumResponseCode.NotSignIn, "未登录");
            }
            List<QZ_Model_In_AdminInfo> admins = _iAdminInfoService.GetAdminList();
            if (admins == null || admins.Count == 0)
            {
                return base.Write(EnumResponseCode.Error, "暂无数据");
            }
            return base.Writes<QZ_Model_In_AdminInfo>(admins, appoints: "AdminID|RealName|Position");
        }
        #endregion

        #region 人事选择分配面试官
        /// <summary>
        /// 人事选择分配面试官
        /// </summary>
        /// <param name="AdminID">管理员ID</param>
        /// <param name="AdminToken">管理员令牌</param>
        /// <param name="InterviewID">面试记录id</param>
        /// <param name="InterviewAdminID">下轮面试管理员ID</param>
        /// <returns></returns>
        public JsonResult ArrangeInterviewer(int AdminID, string AdminToken, int InterviewID, int InterviewAdminID)
        {
            lock (_obj)
            {
                if (!base.ValidAdminUser(AdminID, AdminToken, out QZ_Model_In_AdminInfo adminInfo))
                {
                    return base.Write(EnumResponseCode.NotSignIn, "未登录");
                }
                if (InterviewID < 1 || InterviewAdminID < 1)
                {
                    return base.Write(EnumResponseCode.Error, "错误请求");
                }
                QZ_Model_In_InterviewRecords interviewInfo = _iInterviewRecordsService.Find<QZ_Model_In_InterviewRecords>(InterviewID);
                if (interviewInfo == null)
                {
                    return base.Write(EnumResponseCode.Error, "记录不存在");
                }
                if (interviewInfo.Schedule >= (int)QZ_Enum_Schedules.Fail && interviewInfo.Schedule != (int)QZ_Enum_Schedules.Spare)
                {
                    return base.Write(EnumResponseCode.Error, "面试已结束");
                }
                QZ_Model_In_AdminInfo interviewAdmin = _iAdminInfoService.Find<QZ_Model_In_AdminInfo>(InterviewAdminID);
                if (interviewAdmin == null || interviewAdmin.Status == 1)
                {
                    return base.Write(EnumResponseCode.Error, "该面试官暂不可用");
                }
                if (_iInterviewRecordsService.ArrangeInterviewer(interviewInfo, InterviewAdminID, QZ_Enum_Schedules.InterviewFirst))
                {
                    //通知面试官面试模板
                    if (QZ_Helper_Wechat.GetAccessToken(out string accessToken))
                    {
                        QZ_Model_In_UserBasicInfo basicInfo = _iUserBasicInfoService.FirstOrDefault<QZ_Model_In_UserBasicInfo>(p => p.UserID == interviewInfo.UserID);
                        if (basicInfo != null)
                        {
                            Dictionary<string, string> pairs = new Dictionary<string, string>();
                            pairs.Add("first", $"您好！应聘“{_iPositionsService.GetInfoByID(interviewInfo.ApplyJob)?.PositionName ?? "-"}”职位的候选人已到达，请您尽快前往会议室面试。");
                            pairs.Add("keyword1", $"{basicInfo.RealName} | {basicInfo.Gender} | {basicInfo.Age} | {basicInfo.Education}");
                            pairs.Add("keyword2", $"-");
                            pairs.Add("keyword3", $"{DateTime.Now.Date.ToString("yyyy-MM-dd")}");
                            pairs.Add("keyword4", $"{((QZ_Enum_Schedules)interviewInfo.Schedule).GetEnumDescription()}");
                            pairs.Add("keyword5", $"{adminInfo.RealName}");
                            pairs.Add("remark", "");
                            QZ_Helper_Wechat.SendTemplateMessage(accessToken, QZ_Helper_Wechat.AwaitInterviewTemplate(adminInfo.OpenID, pairs, QZ_Helper_Constant.PartnerAPPID, "/pages/manager/pages/handleInfo/handleInfo", type: 2));
                        }
                    }

                    return base.Write(EnumResponseCode.Success, "分配成功");
                }
                return base.Write(EnumResponseCode.Error, "分配失败");
            }
        }
        #endregion

        #region 面试官提交面试评语等
        /// <summary>
        /// 面试官提交面试评语等
        /// </summary>
        /// <param name="AdminID">管理员ID</param>
        /// <param name="AdminToken">管理员令牌</param>
        /// <param name="InterviewID">面试记录ID</param>
        /// <param name="Remark">面试评语</param>
        /// <param name="InterviewAdminID">下轮面试管理员ID</param>
        /// <param name="Status">本轮面试结果（301：不合适、302：备用、303：合适）</param>
        /// <returns></returns>
        public JsonResult InterviewerRemark(int AdminID, string AdminToken, int InterviewID, string Remark, int InterviewAdminID, int Status)
        {
            lock (_obj)
            {
                if (!base.ValidAdminUser(AdminID, AdminToken, out QZ_Model_In_AdminInfo adminInfo))
                {
                    return base.Write(EnumResponseCode.NotSignIn, "未登录");
                }
                if (InterviewID < 1 || string.IsNullOrWhiteSpace(Remark) || Status < 1)
                {
                    return base.Write(EnumResponseCode.Error, "错误请求");
                }
                QZ_Model_In_InterviewRecords interviewInfo = _iInterviewRecordsService.Find<QZ_Model_In_InterviewRecords>(InterviewID);
                if (interviewInfo == null)
                {
                    return base.Write(EnumResponseCode.Error, "记录不存在");
                }
                if (interviewInfo.Schedule < (int)QZ_Enum_Schedules.InterviewFirst)
                {
                    return base.Write(EnumResponseCode.Error, "请先分配面试");
                }
                if (interviewInfo.Schedule >= (int)QZ_Enum_Schedules.PendingApproval)
                {
                    return base.Write(EnumResponseCode.Error, "面试已结束");
                }
                QZ_Model_In_AdminInfo nextAdminInfo = null;
                switch (Status)
                {
                    case 301:
                        //不合适
                        interviewInfo.Schedule = (int)QZ_Enum_Schedules.Fail;
                        interviewInfo.EndTime = DateTime.Now;
                        break;
                    case 302:
                        //备用
                        interviewInfo.Schedule = (int)QZ_Enum_Schedules.Spare;
                        interviewInfo.EndTime = DateTime.Now;
                        break;
                    default:
                        //通过本轮
                        {
                            nextAdminInfo = _iAdminInfoService.GEtUserInfoByAdminID(InterviewAdminID);
                            if (nextAdminInfo == null)
                            {
                                return base.Write(EnumResponseCode.Error, "下轮面试官不存在");
                            }
                            switch (interviewInfo.Schedule)
                            {
                                case (int)QZ_Enum_Schedules.InterviewSencond:
                                    {
                                        //二面通过，进入总经理审批入职
                                        interviewInfo.Schedule = (int)QZ_Enum_Schedules.PendingApproval;
                                        if (InterviewAdminID < 1)
                                        {
                                            return base.Write(EnumResponseCode.Error, "请选择最终审批人");
                                        }
                                        interviewInfo.InterviewerAdminIds += $"{InterviewAdminID}|";
                                    }
                                    break;
                                default:
                                    {
                                        //一面通过，进入二面
                                        interviewInfo.Schedule = (int)QZ_Enum_Schedules.InterviewSencond;
                                        if (InterviewAdminID < 1)
                                        {
                                            return base.Write(EnumResponseCode.Error, "请选择下轮面试官");
                                        }
                                        interviewInfo.InterviewerAdminIds += $"{InterviewAdminID}|";
                                    }
                                    break;
                            }
                        }
                        break;
                }
                //增添面试点评
                Interview_InterviewerRemark remark = new Interview_InterviewerRemark();
                remark.AdminID = AdminID;
                remark.AdminRealName = adminInfo.RealName;
                remark.Remark = Remark;
                remark.AddTime = DateTime.Now;
                List<Interview_InterviewerRemark> remarks = new List<Interview_InterviewerRemark>();
                if (string.IsNullOrWhiteSpace(interviewInfo.Remarks))
                {
                    remarks.Add(remark);
                    interviewInfo.Remarks = JsonConvert.SerializeObject(remarks);
                }
                else
                {
                    remarks = JsonConvert.DeserializeObject<List<Interview_InterviewerRemark>>(interviewInfo.Remarks);
                    remarks.Add(remark);
                    interviewInfo.Remarks = JsonConvert.SerializeObject(remarks);
                }


                //下轮面试官发送微信公众号消息提醒
                if (Status == 303 && QZ_Helper_Wechat.GetAccessToken(out string accessToken))
                {
                    QZ_Model_In_UserBasicInfo basicInfo = _iUserBasicInfoService.FirstOrDefault<QZ_Model_In_UserBasicInfo>(p => p.UserID == interviewInfo.UserID);
                    if (basicInfo != null)
                    {
                        string path = "";
                        Dictionary<string, string> pairs = new Dictionary<string, string>();
                        if (interviewInfo.Schedule == (int)QZ_Enum_Schedules.PendingApproval)
                        {
                            pairs.Add("first", $"您好！应聘“{_iPositionsService.GetInfoByID(interviewInfo.ApplyJob)?.PositionName ?? "-"}”职位的候选人已通过前两轮面试，请您尽快批准。");
                        }
                        else
                        {
                            pairs.Add("first", $"您好！应聘“{_iPositionsService.GetInfoByID(interviewInfo.ApplyJob)?.PositionName ?? "-"}”职位的候选人已到达，请您尽快前往会议室面试。");
                            path = "/pages/manager/pages/handleInfo/handleInfo";
                        }
                        pairs.Add("keyword1", $"{basicInfo.RealName} | {basicInfo.Gender} | {basicInfo.Age} | {basicInfo.Education}");
                        pairs.Add("keyword2", $"-");
                        pairs.Add("keyword3", $"{DateTime.Now.Date.ToString("yyyy-MM-dd")}");
                        pairs.Add("keyword4", $"{((QZ_Enum_Schedules)interviewInfo.Schedule).GetEnumDescription()}");
                        pairs.Add("keyword5", $"{nextAdminInfo.RealName}");
                        pairs.Add("remark", "");
                        QZ_Helper_Wechat.SendTemplateMessage(accessToken, QZ_Helper_Wechat.AwaitInterviewTemplate(nextAdminInfo.OpenID, pairs, QZ_Helper_Constant.PartnerAPPID, path, type: 2));
                    }
                }

                _iInterviewRecordsService.Update(interviewInfo);
                return base.Write(EnumResponseCode.Success);
            }
        }
        #endregion

        #region 入职审批
        /// <summary>
        /// 入职审批
        /// </summary>
        /// <param name="AdminID">管理员ID</param>
        /// <param name="AdminToken">管理员令牌</param>
        /// <param name="InterviewID">面试记录ID</param>
        /// <param name="RealOffer">转正薪资</param>
        /// <param name="TryOffer">试用薪资</param>
        /// <param name="EntryTime">入职时间</param>
        /// <returns></returns>
        public JsonResult ApplayApproval(int AdminID, string AdminToken, int InterviewID, decimal RealOffer, decimal TryOffer, DateTime EntryTime)
        {
            lock (_obj)
            {
                if (!base.ValidAdminUser(AdminID, AdminToken, out QZ_Model_In_AdminInfo adminInfo))
                {
                    return base.Write(EnumResponseCode.NotSignIn, "未登录");
                }
                if (InterviewID < 1 || RealOffer < 1 || TryOffer < 1 || EntryTime == DateTime.MinValue)
                {
                    return base.Write(EnumResponseCode.Error, "错误请求");
                }
                QZ_Model_In_InterviewRecords interviewInfo = _iInterviewRecordsService.Find<QZ_Model_In_InterviewRecords>(InterviewID);
                if (interviewInfo == null)
                {
                    return base.Write(EnumResponseCode.Error, "记录不存在");
                }
                if (interviewInfo.Schedule < (int)QZ_Enum_Schedules.InterviewFirst)
                {
                    return base.Write(EnumResponseCode.Error, "请先分配面试");
                }
                if (interviewInfo.Schedule >= (int)QZ_Enum_Schedules.Fail)
                {
                    return base.Write(EnumResponseCode.Error, "面试完成");
                }
                interviewInfo.Schedule = (int)QZ_Enum_Schedules.Qualified;//标记通过面试
                interviewInfo.EndTime = DateTime.Now;
                interviewInfo.RealOffer = RealOffer;
                interviewInfo.TryOffer = TryOffer;
                interviewInfo.EntryTime = EntryTime;
                _iInterviewRecordsService.Update(interviewInfo);

                if (QZ_Helper_Wechat.GetAccessToken(out string accessToken, QZ_Helper_Constant.PartnerAPPID, QZ_Helper_Constant.PartnerAPPSecret))
                {
                    QZ_Model_In_UserBasicInfo basicInfo = _iUserBasicInfoService.FirstOrDefault<QZ_Model_In_UserBasicInfo>(p => p.UserID == interviewInfo.UserID);
                    //给面试者发送微信公众号消息提醒
                    Dictionary<string, string> pairs = new Dictionary<string, string>();
                    pairs.Add("first", $"{basicInfo?.RealName ?? "-"}，您好！恭喜您通过面试，被我公司录用");
                    pairs.Add("keyword1", $"长沙求知");
                    pairs.Add("keyword2", $"{_iPositionsService.GetInfoByID(interviewInfo.ApplyJob)?.PositionName ?? "-"}");
                    pairs.Add("keyword3", "录取");
                    pairs.Add("remark", "我们期待您的加入！");
                    QZ_Helper_Wechat.SendTemplateMessage(accessToken, QZ_Helper_Wechat.AwaitInterviewTemplate(adminInfo.OpenID, pairs, QZ_Helper_Constant.PartnerAPPID, "/pages/interviewer/pages/notification/notification", type: 3));

                }

                return base.Write(EnumResponseCode.Success, "审批成功");
            }
        }
        #endregion

        #region 更改备用面试记录进度状态
        /// <summary>
        /// 更改备用面试记录进度状态
        /// </summary>
        /// <param name="AdminID">当前管理员ID</param>
        /// <param name="AdminToken">当前管理员令牌</param>
        /// <param name="InterviewID">面试记录</param>
        /// <param name="Schedule">进度类型</param>
        /// <param name="NextAdminID">下轮操作管理员</param>
        /// <returns></returns>
        public JsonResult AlterSpareInterview(int AdminID, string AdminToken, int InterviewID, QZ_Enum_Schedules Schedule, int NextAdminID)
        {
            if (!base.ValidAdminUser(AdminID, AdminToken, out QZ_Model_In_AdminInfo adminInfo))
            {
                return base.Write(EnumResponseCode.NotSignIn, "未登录");
            }
            if (InterviewID < 1 || NextAdminID < 1 || Schedule == default)
            {
                return base.Write(EnumResponseCode.Error, "请求参数有误");
            }
            QZ_Model_In_InterviewRecords interviewInfo = _iInterviewRecordsService.Find<QZ_Model_In_InterviewRecords>(InterviewID);
            if (interviewInfo == null)
            {
                return base.Write(EnumResponseCode.Error, "记录不存在");
            }
            if (interviewInfo.Schedule != (int)QZ_Enum_Schedules.Spare)
            {
                return base.Write(EnumResponseCode.Error, "异常操作，面试不属于备用状态");
            }
            QZ_Model_In_AdminInfo nextAdminInfo = _iAdminInfoService.GEtUserInfoByAdminID(NextAdminID);
            if (nextAdminInfo == null)
            {
                return base.Write(EnumResponseCode.Error, "未找到下轮操作管理员");
            }
            switch (Schedule)
            {
                case QZ_Enum_Schedules.InterviewSencond:
                    {
                        //进入二面
                        interviewInfo.Schedule = (int)Schedule;
                        interviewInfo.InterviewerAdminIds += $"{NextAdminID}|";
                    }
                    break;
                case QZ_Enum_Schedules.PendingApproval:
                    {
                        //进入入职审批
                        interviewInfo.Schedule = (int)Schedule;
                        interviewInfo.InterviewerAdminIds += $"{NextAdminID}|";
                    }
                    break;
                default:
                    return base.Write(EnumResponseCode.Error, "面试进度有误");
            }
            //下轮面试官发送微信公众号消息提醒
            if (QZ_Helper_Wechat.GetAccessToken(out string accessToken))
            {
                QZ_Model_In_UserBasicInfo basicInfo = _iUserBasicInfoService.FirstOrDefault<QZ_Model_In_UserBasicInfo>(p => p.UserID == interviewInfo.UserID);
                if (basicInfo != null)
                {
                    string path = "";
                    Dictionary<string, string> pairs = new Dictionary<string, string>();
                    if (interviewInfo.Schedule == (int)QZ_Enum_Schedules.PendingApproval)
                    {
                        pairs.Add("first", $"您好！应聘“{_iPositionsService.GetInfoByID(interviewInfo.ApplyJob)?.PositionName ?? "-"}”职位的候选人已通过前两轮面试，请您尽快批准。");
                    }
                    else
                    {
                        pairs.Add("first", $"您好！应聘“{_iPositionsService.GetInfoByID(interviewInfo.ApplyJob)?.PositionName ?? "-"}”职位的候选人已到达，请您尽快前往会议室面试。");
                        path = "/pages/manager/pages/handleInfo/handleInfo";
                    }
                    pairs.Add("keyword1", $"{basicInfo.RealName} | {basicInfo.Gender} | {basicInfo.Age} | {basicInfo.Education}");
                    pairs.Add("keyword2", $"-");
                    pairs.Add("keyword3", $"{DateTime.Now.Date.ToString("yyyy-MM-dd")}");
                    pairs.Add("keyword4", $"{((QZ_Enum_Schedules)interviewInfo.Schedule).GetEnumDescription()}");
                    pairs.Add("keyword5", $"{nextAdminInfo.RealName}");
                    pairs.Add("remark", "");
                    QZ_Helper_Wechat.SendTemplateMessage(accessToken, QZ_Helper_Wechat.AwaitInterviewTemplate(nextAdminInfo.OpenID, pairs, QZ_Helper_Constant.PartnerAPPID, path, type: 2));
                }
            }

            _iInterviewRecordsService.Update(interviewInfo);
            return base.Write(EnumResponseCode.Success);
        }
        #endregion
    }
}