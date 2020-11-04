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
using QZ.Model.Expand.Interview;
using QZ.Model.Interview;

namespace QZ.Interview.Api
{
    [Route("api/InterviewRecord/[action]")]
    [ApiController]
    public class InterviewRecordController : InterviewControllerBase
    {

        public InterviewRecordController(QZ_In_IAdminInfoService iAdminInfoService, QZ_In_IInterviewRecordsService iInterviewRecordsService, QZ_In_IPositionsService iPositionsService) : base(iAdminInfoService)
        {
            this._iAdminInfoService = iAdminInfoService;
            this._iInterviewRecordsService = iInterviewRecordsService;
            this._iPositionsService = iPositionsService;
        }
        private readonly static object _obj = new object();
        private readonly QZ_In_IInterviewRecordsService _iInterviewRecordsService;
        private readonly QZ_In_IAdminInfoService _iAdminInfoService;
        private readonly QZ_In_IPositionsService _iPositionsService;

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
        public JsonResult GetInterviewInfos(int AdminID, string AdminToken, bool Type, int Page, byte? Scheduls)
        {
            if (!base.ValidAdminUser(AdminID, AdminToken, out QZ_Model_In_AdminInfo adminInfo))
            {
                return base.Write(EnumResponseCode.NotSignIn, "未登录");
            }
            Page = Page < 1 ? 1 : Page;
            var data = _iInterviewRecordsService.GetDataInterview();
            if (!data.Any())
            {
                return base.Write(EnumResponseCode.Error, "暂无数据");
            }
            if (!Type)
            {
                //待处理
                switch (adminInfo.Position)
                {
                    case (int)QZ_Enum_Positions.Administrative:
                        {
                            //行政-待分配的面试
                            data = data.Where(p => p.ExtSchedule < (int)QZ_Enum_Schedules.PendingApproval);
                        }
                        break;
                    case (int)QZ_Enum_Positions.Boss:
                        {
                            //总经理-可查看所有权限
                            data = data.Where(p => p.ExtSchedule <= (int)QZ_Enum_Schedules.PendingApproval);
                        }
                        break;
                    default:
                        {
                            //其它角色-我负责的待处理面试
                            data = data.Where(p => p.ExtAdminIds.EndsWith(adminInfo.AdminID + "|"));
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
            List<QZ_Model_In_UserBasicInfo> list = data.Skip((Page - 1) * 20).Take(20).ToList();
            int totalPage = data.Count().CalculateTotalPageNumber(20);
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("page", Page.ToString());
            pairs.Add("limit", "20");
            pairs.Add("totalPages", totalPage.ToString());

            return base.Writes(list, appoints: "RealName|Gender|Age|BirthDate|Education|ApplyJob|ExtInterviewDate|ExtSchedule|ExtInterviewID|ExtAdminIds", data: pairs);
        }
        #endregion

        #region 获取职位
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
                if (interviewInfo.Schedule >= (int)QZ_Enum_Schedules.Fail)
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
                        QZ_Helper_Wechat.SendTemplateMessage(accessToken, QZ_Helper_Wechat.AwaitInterviewTemplate(adminInfo.OpenID, "appid", "页面路由", type: 2));
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
                switch (Status)
                {
                    case 301:
                        //不合适
                        interviewInfo.Schedule = (int)QZ_Enum_Schedules.Fail;
                        break;
                    case 302:
                        //备用
                        interviewInfo.Schedule = (int)QZ_Enum_Schedules.Spare;
                        break;
                    default:
                        //通过本轮
                        {
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

                //面试通过给面试者、下轮面试官发送微信公众号消息提醒
                //if (QZ_Helper_Wechat.GetAccessToken(out string accessToken))
                //{
                //    QZ_Helper_Wechat.SendTemplateMessage(accessToken, QZ_Helper_Wechat.AwaitInterviewTemplate(adminInfo.OpenID, "appid", "页面路由", type: 2));
                //}
                //if (QZ_Helper_Wechat.GetAccessToken(out string accessToken))
                //{
                //    QZ_Helper_Wechat.SendTemplateMessage(accessToken, QZ_Helper_Wechat.AwaitInterviewTemplate(adminInfo.OpenID, "appid", "页面路由", type: 3));
                //}

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
                interviewInfo.RealOffer = RealOffer;
                interviewInfo.TryOffer = TryOffer;
                interviewInfo.EntryTime = EntryTime;
                _iInterviewRecordsService.Update(interviewInfo);

                //给面试者发送微信公众号消息提醒

                return base.Write(EnumResponseCode.Success, "审批成功");
            }
        }
        #endregion
    }
}