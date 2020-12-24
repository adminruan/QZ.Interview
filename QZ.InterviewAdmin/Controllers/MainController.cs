using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QZ.Interface.Interview_IService;
using QZ.Model.Interview;

namespace QZ.InterviewAdmin.Controllers
{
    public class MainController : BaseController
    {
        public MainController(QZ_In_IInterviewRecordsService iInterviewRecordsService, QZ_In_IPositionsService iPositionsService)
        {
            this._iInterviewRecordsService = iInterviewRecordsService;
            this._iPositionsService = iPositionsService;
        }
        private readonly QZ_In_IInterviewRecordsService _iInterviewRecordsService;
        private readonly QZ_In_IPositionsService _iPositionsService;
        #region 主页信息
        public IActionResult Index()
        {
            if (!base.ValidUser(out QZ_Model_In_AdminInfo adminInfo))
            {
                return Redirect("/Login/Index");
            }
            else
            {
                ViewBag.UserName = adminInfo.RealName;
                return View(this.GetIndexInfo());
            }
        }

        /// <summary>
        /// 获取主页信息
        /// </summary>
        /// <returns>Item1：本月应聘人数、Itme2：今日应聘岗位信息、Item3：今日应聘总人数</returns>
        private Tuple<int, Dictionary<string, int>, int> GetIndexInfo()
        {
            //职位
            List<QZ_Model_In_Positions> positions = _iPositionsService.GetPositions();
            if (positions == null)
            {
                return Tuple.Create(0, new Dictionary<string, int>(), 0);
            }
            //今日应聘信息
            List<QZ_Model_In_InterviewRecords> interviewRecords = _iInterviewRecordsService.GetTodayInterviewInfo() ?? new List<QZ_Model_In_InterviewRecords>();
            //本月应聘人数
            int thisMonthNum = _iInterviewRecordsService.GetThisMonthInterviewNumber();

            Dictionary<string, int> pairs = new Dictionary<string, int>();
            int totalNum = 0;
            foreach (var item in positions)
            {
                int num = interviewRecords.Where(p => p.ApplyJob == item.ID).Count();
                totalNum += num;
                pairs.Add(item.PositionName, num);
            }
            return Tuple.Create(thisMonthNum, pairs, totalNum);
        }
        #endregion
    }
}