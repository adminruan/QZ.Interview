using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QZ.Common;
using QZ.Common.Enums;
using QZ.Common.Expand;
using QZ.Interface.Interview_IService;
using QZ.InterviewAdmin.Controllers;
using QZ.Model.Expand;
using QZ.Model.Expand.Interview;
using QZ.Model.Interview;

namespace QZ.InterviewAdmin.Areas.Interview.Controllers
{
    public class UserController : BaseController
    {
        public UserController(QZ_In_IInterviewRecordsService iInterviewRecordsService, QZ_In_IUserBasicInfoService iUserBasicInfoService, QZ_In_IPositionsService iPositionsService)
        {
            this._iInterviewRecordsService = iInterviewRecordsService;
            this._iUserBasicInfoService = iUserBasicInfoService;
            this._iPositionsService = iPositionsService;
        }
        private readonly QZ_In_IInterviewRecordsService _iInterviewRecordsService;
        private readonly QZ_In_IUserBasicInfoService _iUserBasicInfoService;
        private readonly QZ_In_IPositionsService _iPositionsService;

        #region 面试信息
        /// <summary>
        /// 面试信息视图
        /// </summary>
        /// <returns></returns>
        [Area("Interview")]
        public IActionResult InterviewInfo()
        {
            Dictionary<string, int> pairs = QZ_Helper_EnumHelper.ToPairs(typeof(QZ_Enum_Schedules));
            ViewBag.Schedules = pairs;
            ViewBag.Positions = _iPositionsService.GetPositions();
            return View();
        }

        /// <summary>
        /// 获取面试信息
        /// </summary>
        /// <returns></returns>
        [Area("Interview")]
        public IActionResult GetInterviewInfo(int page, int limit, string mobileOrIDNumber, DateTime date, int shedules, int position)
        {
            base.InitPageParameters(ref page, ref limit);
            Expression<Func<QZ_Model_In_UserBasicInfo, bool>> where = p => true;
            if (!string.IsNullOrWhiteSpace(mobileOrIDNumber))
            {
                where = where.And(p => p.Moblie.Contains(mobileOrIDNumber) || p.IdentityNumber.Contains(mobileOrIDNumber) || p.RealName.Contains(mobileOrIDNumber));
            }
            if (date > DateTime.MinValue)
            {
                where = where.And(p => p.ExtInterviewDate >= date.Date && p.ExtInterviewDate < date.AddDays(1).Date);
            }
            if (shedules > 0)
            {
                where = where.And(p => p.ExtSchedule == shedules);
            }
            if (position > 0)
            {
                where = where.And(p => p.ApplyJob == position);
            }
            IQueryable<QZ_Model_In_UserBasicInfo> data = _iInterviewRecordsService.GetData();
            if (!data.Any())
            {
                return ContentResult(new { code = 0, msg = string.Empty, data = new List<QZ_Model_In_UserBasicInfo>(), count = 0 });
            }
            var positions = _iPositionsService.GetPositions() ?? new List<QZ_Model_In_Positions>();
            List<QZ_Model_In_UserBasicInfo> list = data.Where(where).Skip((page - 1) * limit).Take(limit).ToList();
            list.ForEach(x =>
            {
                x.ExtResumeSource = ((QZ_Enum_RecruitPlatform)x.ResumeSource).GetEnumDescription();
                x.ExtScheduleText = ((QZ_Enum_Schedules)x.ExtSchedule).GetEnumDescription();
                x.ExtApplyJob = positions.FirstOrDefault(p => p.ID == x.ApplyJob)?.PositionName ?? "未知";
                if (!string.IsNullOrWhiteSpace(x.ExtRemarks))
                {
                    var remarks = JsonConvert.DeserializeObject<List<Interview_InterviewerRemark>>(x.ExtRemarks);
                    if (remarks != null && remarks.Count > 0)
                    {
                        if (remarks.Count > 1)
                        {
                            //二面完成
                            x.ExtSecondDate = remarks.OrderByDescending(p => p.AddTime).First().AddTime;
                        }
                        //一面完成
                        x.ExtFirstDate = remarks.First().AddTime;
                    }
                }
            });
            return ContentResult(new { code = 0, msg = "", data = list, count = data.Count() });
        }
        #endregion

        #region 面试信息编辑
        /// <summary>
        /// 面试信息编辑
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [Area("Interview")]
        public IActionResult InterviewEdit(int id)
        {
            QZ_Model_In_UserBasicInfo data = new QZ_Model_In_UserBasicInfo();
            if (id > 0)
            {
                data = _iInterviewRecordsService.GetInterviewInfoByInterviewID(id);
            }
            if (data != null)
            {
                data.ExtResumeSource = ((QZ_Enum_RecruitPlatform)data.ResumeSource).GetEnumDescription();
                //教育经历
                if (!string.IsNullOrWhiteSpace(data.Educations))
                {
                    data.ExtEducations = JsonConvert.DeserializeObject<List<Interview_UserEducation>>(data.Educations);
                }
                else
                {
                    data.ExtEducations = new List<Interview_UserEducation>();
                }
                //工作经历
                if (!string.IsNullOrWhiteSpace(data.Jobs))
                {
                    data.ExtJobs = JsonConvert.DeserializeObject<List<Interview_UserHistoryJob>>(data.Jobs);
                }
                else
                {
                    data.ExtJobs = new List<Interview_UserHistoryJob>();
                }
                //面试评语
                if (!string.IsNullOrWhiteSpace(data.ExtRemarks))
                {
                    data.ExtRemarkList = JsonConvert.DeserializeObject<List<Interview_InterviewerRemark>>(data.ExtRemarks);
                }
                else
                {
                    data.ExtRemarkList = new List<Interview_InterviewerRemark>();
                }
                //历史面试记录
                if (data.ExtInterviewID > 0)
                {
                    //历史记录
                    List<QZ_Model_In_UserBasicInfo> historys = _iInterviewRecordsService.GetHistoryInterviews(data.ExtInterviewID, data.UserID);
                    //岗位信息
                    List<QZ_Model_In_Positions> positions = _iPositionsService.GetPositions();
                    if (historys != null && historys.Count > 0)
                    {
                        foreach (var item in historys)
                        {
                            item.ExtRemarkList = JsonConvert.DeserializeObject<List<Interview_InterviewerRemark>>(item.ExtRemarks);
                            item.ExtApplyJob = positions.FirstOrDefault(p => p.ID == item.ApplyJob)?.PositionName ?? "未知";
                            item.ExtScheduleText = ((QZ_Enum_Schedules)item.ExtSchedule).GetEnumDescription();
                        }
                    }
                    data.ExtHistoryInterviews = historys;
                }
            }
            else
            {
                data = new QZ_Model_In_UserBasicInfo();
                data.ExtEducations = new List<Interview_UserEducation>();
                data.ExtJobs = new List<Interview_UserHistoryJob>();
                data.ExtRemarkList = new List<Interview_InterviewerRemark>();
            }
            ViewBag.Schedules = QZ_Helper_EnumHelper.ToPairs(typeof(QZ_Enum_Schedules));
            return View(data);
        }
        #endregion
    }
}