using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QZ.Common;
using QZ.Common.Enums;
using QZ.Common.Helper;
using QZ.Interface.Interview_IService;
using QZ.Interview.Api.Bases;
using QZ.Model.Interview;

namespace QZ.Interview.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewRecordController : InterviewControllerBase
    {

        public InterviewRecordController(QZ_In_IAdminInfoService iAdminInfoService, QZ_In_IInterviewRecordsService iInterviewRecordsService) : base(iAdminInfoService)
        {
            this._iAdminInfoService = iAdminInfoService;
            this._iInterviewRecordsService = iInterviewRecordsService;
        }
        private readonly QZ_In_IInterviewRecordsService _iInterviewRecordsService;
        private readonly QZ_In_IAdminInfoService _iAdminInfoService;

        #region 待处理面试信息
        /// <summary>
        /// 待处理面试信息
        /// </summary>
        /// <param name="AdminID">管理员ID</param>
        /// <param name="AdminToken">管理员令牌</param>
        /// <param name="Type">类型（True：已处理、False：待处理）</param>
        /// <param name="Page">页码</param>
        /// <param name="Scheduls">已处理的状态类型（4：不合适、5：备用、6：合适）</param>
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
                data = data.Where(p => p.ExtAdminIds.Contains(adminInfo.AdminID.ToString()) && p.ExtSchedule > (int)QZ_Enum_Schedules.PendingApproval);
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

            return base.Write(list, data: pairs);
        }
        #endregion

        #region 获取职位
        public JsonResult GetPositions()
        {
            var pairs = QZ_Helper_EnumHelper.ToPairs(typeof(QZ_Enum_Positions));
            Dictionary<string, string> newPairs = new Dictionary<string, string>();
            foreach (var item in pairs)
            {
                newPairs.Add(item.Key, item.Value.ToString());
            }
            return base.Write(EnumResponseCode.Success, data: newPairs);
        }
        #endregion
    }
}