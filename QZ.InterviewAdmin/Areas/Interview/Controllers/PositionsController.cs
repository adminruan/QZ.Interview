using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QZ.Common.Expand;
using QZ.Interface.Interview_IService;
using QZ.InterviewAdmin.Controllers;
using QZ.Model.Interview;

namespace QZ.InterviewAdmin.Areas.Interview.Controllers
{
    public class PositionsController : BaseController
    {
        public PositionsController(QZ_In_IPositionsService iPositionsService)
        {
            this._iPositionsService = iPositionsService;
        }
        private readonly QZ_In_IPositionsService _iPositionsService;

        #region 职位信息
        [Area("Interview")]
        public IActionResult PositionInfos()
        {
            return View();
        }

        [Area("Interview")]
        public IActionResult GetPositionInfos(int page, int limit, bool? status)
        {
            base.InitPageParameters(ref page, ref limit);
            Expression<Func<QZ_Model_In_Positions, bool>> where = p => true;
            if (status != null)
            {
                where = where.And(p => p.State == status);
            }
            var data = _iPositionsService.Query<QZ_Model_In_Positions>(where).OrderByDescending(p => p.AddTime);
            if (!data.Any())
            {
                return ContentResult(new { code = 0, msg = "", data = new List<QZ_Model_In_Positions>(), count = 0 });
            }
            List<QZ_Model_In_Positions> list = data.Skip((page - 1) * limit).Take(limit).ToList();
            return ContentResult(new { code = 0, msg = "", data = list, count = data.Count() });
        }
        #endregion

        #region 职位编辑
        [Area("Interview")]
        public IActionResult EditPositions(int id)
        {
            QZ_Model_In_Positions data = new QZ_Model_In_Positions();
            if (id > 0)
            {
                data = _iPositionsService.Find<QZ_Model_In_Positions>(id);
            }
            return View(data);
        }

        [Area("Interview")]
        [HttpPost]
        public IActionResult SaveChange(QZ_Model_In_Positions model)
        {
            if (model == null)
            {
                return ContentTips(EnumResponseCode.Error, "提交的数据为空");
            }
            if (!ModelState.IsValid)
            {
                return ContentTips(EnumResponseCode.Error, "提交的数据有误");
            }
            if (_iPositionsService.Any<QZ_Model_In_Positions>(p => p.ID != model.ID && p.State && p.PositionName == model.PositionName))
            {
                return ContentTips(EnumResponseCode.Error, "请勿重复配置同一职位");
            }
            if (model.ID > 0)
            {
                //编辑
                _iPositionsService.Update(model);
            }
            else
            {
                //新增
                model.AddTime = DateTime.Now;
                _iPositionsService.Insert(model);
            }
            return ContentTips(EnumResponseCode.Success, "操作成功");
        }
        #endregion
    }
}