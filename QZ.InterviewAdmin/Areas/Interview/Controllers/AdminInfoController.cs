using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QZ.Common;
using QZ.Common.Enums;
using QZ.Common.Expand;
using QZ.Interface.Interview_IService;
using QZ.InterviewAdmin.Controllers;
using QZ.Model.Interview;

namespace QZ.InterviewAdmin.Areas.Interview.Controllers
{
    public class AdminInfoController : BaseController
    {
        public AdminInfoController(QZ_In_IAdminInfoService iAdminInfoService)
        {
            this._iAdminInfoService = iAdminInfoService;
        }
        private readonly QZ_In_IAdminInfoService _iAdminInfoService;

        #region 管理员信息
        /// <summary>
        /// 管理员信息视图
        /// </summary>
        /// <returns></returns>
        [Area("Interview")]
        public IActionResult AdminInfo()
        {
            return View();
        }

        /// <summary>
        /// 获取管理员信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="userName"></param>
        /// <param name="mobile"></param>
        /// <param name="adminID"></param>
        /// <param name="realName"></param>
        /// <returns></returns>
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
        #endregion

        #region 管理员信息编辑
        [Area("Interview")]
        public IActionResult AdminInfoEdit(int adminID)
        {
            QZ_Model_In_AdminInfo adminInfo = new QZ_Model_In_AdminInfo();
            if (adminID > 0)
                adminInfo = _iAdminInfoService.Find<QZ_Model_In_AdminInfo>(adminID);
            ViewBag.Positions = this.GetPositions();
            return View(adminInfo ?? new QZ_Model_In_AdminInfo());
        }

        /// <summary>
        /// 编辑保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Area("Interview")]
        [HttpPost]
        public IActionResult EditSaveChange(QZ_Model_In_AdminInfo entity)
        {
            if (entity == null)
            {
                return ContentTips(EnumResponseCode.Error, "提交的数据为空");
            }
            if (string.IsNullOrEmpty(entity.RealName) || string.IsNullOrWhiteSpace(entity.UserName))
            {
                return ContentTips(EnumResponseCode.Error, "请求数据有误");
            }
            if (_iAdminInfoService.Any<QZ_Model_In_AdminInfo>(p => p.UserName == entity.UserName && p.Status == 0 && p.AdminID != entity.AdminID))
            {
                return ContentTips(EnumResponseCode.Error, "用户名重复");
            }
            if (_iAdminInfoService.Any<QZ_Model_In_AdminInfo>(p => p.Mobile == entity.Mobile && p.Status == 0 && p.AdminID != entity.AdminID))
            {
                return ContentTips(EnumResponseCode.Error, "联系方式重复");
            }
            if (entity.AdminID > 0)
            {
                //编辑
                QZ_Model_In_AdminInfo oldAdminInfo = _iAdminInfoService.Find<QZ_Model_In_AdminInfo>(entity.AdminID);
                oldAdminInfo.RealName = entity.RealName;
                oldAdminInfo.Mobile = entity.Mobile;
                oldAdminInfo.UserName = entity.UserName;
                oldAdminInfo.Position = entity.Position;
                oldAdminInfo.Status = entity.Status;
                oldAdminInfo.OpenID = entity.OpenID;
                if (oldAdminInfo.Password != entity.Password)
                {
                    //密码更新
                    oldAdminInfo.Password = QZ_Helper_Encryption.Get32MD5String(entity.Password).ToLower();
                }
                _iAdminInfoService.Update(oldAdminInfo);
            }
            else
            {
                //新增
                entity.Password = QZ_Helper_Encryption.Get32MD5String(entity.Password).ToLower();
                entity.CreateTime = DateTime.Now;
                _iAdminInfoService.Insert(entity);
            }
            return ContentTips(EnumResponseCode.Success, "操作成功");
        }
        #endregion

        #region 职位信息
        /// <summary>
        /// 职位信息
        /// </summary>
        /// <returns></returns>
        private IDictionary<string, int> GetPositions()
        {
            var pairs = QZ_Helper_EnumHelper.ToPairs(typeof(QZ_Enum_Positions));
            return pairs;
        }
        #endregion
    }
}