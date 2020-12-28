using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using QZ.Common;
using QZ.Common.Enums;
using QZ.Common.Expand;
using QZ.Interface;
using QZ.Interface.Interview_IService;
using QZ.InterviewAdmin.Controllers;
using QZ.Model.Interview;

namespace QZ.InterviewAdmin.Areas.QuestionsManage.Controllers
{
    public class InterviewQuestionsController : BaseController
    {
        private readonly QZ_In_IAdminInfoService _iAdminInfoService;
        private readonly QZ_In_IInterviewQuestionsService _iInterviewQuestionsService;
        private readonly IMemoryCache _memoryCache;

        public InterviewQuestionsController(QZ_In_IAdminInfoService iAdminInfoService, QZ_In_IInterviewQuestionsService iInterviewQuestionsService, IMemoryCache memoryCache)
        {
            this._iAdminInfoService = iAdminInfoService;
            this._iInterviewQuestionsService = iInterviewQuestionsService;
            this._memoryCache = memoryCache;
        }

        #region 面试题信息
        [Area("QuestionsManage")]
        public IActionResult Index()
        {
            string stradmin = QZ_Helper_CookieHelper.ReadCookie(this.HttpContext, "CurrentUser");
            if (!string.IsNullOrEmpty(stradmin))
            {
                QZ_Model_In_AdminInfo Admin = JsonConvert.DeserializeObject<QZ_Model_In_AdminInfo>(stradmin);
                ViewBag.GroupID = 0;
                return View();
            }
            return View("/Login/Index");
        }
        /// <summary>
        /// 获取面试题信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [Area("QuestionsManage")]
        public IActionResult GetInterQuestionList(int page, int limit, string stval, int? verific)
        {
            int total = 0;
            List<QZ_Model_In_InterviewQuestions> nlist = new List<QZ_Model_In_InterviewQuestions>();
            string stradmin = QZ_Helper_CookieHelper.ReadCookie(this.HttpContext, "CurrentUser");
            if (!string.IsNullOrEmpty(stradmin))
            {
                QZ_Model_In_AdminInfo Admin = JsonConvert.DeserializeObject<QZ_Model_In_AdminInfo>(stradmin);
                string sql = "select * from In_InterviewQuestions where 1=1 ";
                string sqlid = "select id from In_InterviewQuestions where 1=1 ";
                if (!string.IsNullOrWhiteSpace(stval))
                {
                    if (IsNumeric(stval))
                    {
                        sql += " and id =" + int.Parse(stval);
                        sqlid += " and id =" + int.Parse(stval);
                    }
                    else
                    {
                        sql += " and title like '%" + stval.Trim() + "%'";
                        sqlid += " and title like '%" + stval.Trim() + "%'";
                    }
                }
                if (verific != null)
                {
                    sql += " and verific =" + verific;
                    sqlid += " and verific =" + verific;
                }
                sql += " order by id desc, updatetime desc";
                sqlid += " order by id desc, updatetime desc";

                PageResult<QZ_Model_In_InterviewQuestions> data = this._iInterviewQuestionsService.ExcuteQueryPage<QZ_Model_In_InterviewQuestions>(sql, limit, page);
                if (data != null)
                {
                    nlist = data.DataList;
                    total = data.TotalCount;
                }
                List<int> idlist = this._iInterviewQuestionsService.ExcuteQuery<QZ_Model_In_InterviewQuestions>(sqlid).Select(s => s.id).ToList();
                if (idlist != null)
                {
                    string ids = string.Join(',', idlist) + ",";
                    this._memoryCache.Set("InterQuestionids_" + Admin.AdminID, ids, new DateTimeOffset(DateTime.Now.AddDays(1)));
                }
            }
            var jsonData = new
            {
                code = 0,
                msg = "",
                data = nlist,
                count = total
            };
            return ContentResult(jsonData);
        }
        #endregion

        [Area("Interview")]
        public IActionResult GetAdminInfo(int page, int limit, string userName, string mobile, int adminID, string realName)
        {
            base.InitPageParameters(ref page, ref limit);
            Expression<Func<QZ_Model_In_AdminInfo, bool>> where = p => p.AdminID > 0;
            if (!string.IsNullOrWhiteSpace(userName))
            {
                where = where.And(p => p.UserName.Equals(userName));
            }
            if (!string.IsNullOrWhiteSpace(mobile))
            {
                where = where.And(p => p.Mobile.Equals(mobile));
            }
            if (adminID > 0)
            {
                where = where.And(p => p.AdminID == adminID);
            }
            if (!string.IsNullOrWhiteSpace(realName))
            {
                where = where.And(p => p.RealName.Equals(realName));
            }
            var pageResult = _iAdminInfoService.QueryPage(where, limit, page, o => o.CreateTime);
            var list = pageResult?.DataList ?? new List<QZ_Model_In_AdminInfo>();
            list.ForEach(x =>
            {
                x.ExtPositionName = ((QZ_Enum_Positions)x.Position).GetEnumDescription();
            });
            var json = new
            {
                code = 0,
                msg = string.Empty,
                data = pageResult?.DataList ?? new List<QZ_Model_In_AdminInfo>(),
                count = pageResult?.TotalCount ?? 0
            };
            return ContentResult(json);
        }
    }
}