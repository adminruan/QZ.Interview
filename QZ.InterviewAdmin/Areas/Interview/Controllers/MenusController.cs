using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QZ.Common.Expand;
using QZ.Interface.Interview_IService;
using QZ.InterviewAdmin.Controllers;
using QZ.Model.Interview;

namespace QZ.InterviewAdmin.Areas.Interview.Controllers
{
    public class MenusController : BaseController
    {
        public MenusController(QZ_In_IMenuService iMenuService)
        {
            this._iMenuService = iMenuService;
        }
        private readonly static object _obj = new object();
        private readonly QZ_In_IMenuService _iMenuService;

        #region 菜单信息
        /// <summary>
        /// 菜单信息
        /// </summary>
        /// <returns></returns>
        [Area("Interview")]
        public IActionResult MenuInfos()
        {
            return View();
        }

        /// <summary>
        /// 获取菜单信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="menuName">菜单名称</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        [Area("Interview")]
        public IActionResult GetMenuInfos(int page, int limit, string menuName, int? pid, byte? status)
        {
            base.InitPageParameters(ref page, ref limit);
            Expression<Func<QZ_Model_In_Menu, bool>> where = p => true;
            if (!string.IsNullOrWhiteSpace(menuName))
            {
                where = where.And(p => p.MenuName == menuName);
            }
            if (pid != null)
            {
                if (pid > 0)
                {
                    where = where.And(p => p.ParentID == pid);
                }
                else
                {
                    where = where.And(p => p.ParentID == 0);
                }
            }
            if (status != null)
            {
                where = where.And(p => p.Status == status);
            }
            var data = _iMenuService.Query<QZ_Model_In_Menu>(where).OrderBy(p => p.SortNumber);
            List<QZ_Model_In_Menu> list = new List<QZ_Model_In_Menu>();
            if (data.Any())
            {
                list = data.Skip((page - 1) * limit).Take(limit).ToList();
            }
            var result = new { code = 0, msg = string.Empty, data = list, count = data.Count() };
            return ContentResult(result);
        }

        /// <summary>
        /// 获取当前ID的父ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Area("Interview")]
        public int GetParentIDByID(int id)
        {
            QZ_Model_In_Menu menu = _iMenuService.Find<QZ_Model_In_Menu>(id);
            return menu?.ParentID ?? 0;
        }

        /// <summary>
        /// 获取当前菜单所有父菜单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Area("Interview")]
        public IActionResult GetParentsByID(int id)
        {
            var parents = _iMenuService.GetParentsByID(id);
            if (parents == null || parents.Count == 0)
            {
                parents = new List<QZ_Model_In_Menu>();
            }
            var data = parents.Select(p => ValueTuple.Create(p.ID, p.MenuName)).ToList();
            return ContentResult(data);
        }
        #endregion

        #region 菜单编辑
        /// <summary>
        /// 编辑视图
        /// </summary>
        /// <param name="id">需要编辑的菜单ID</param>
        /// <param name="pid">菜单所属父ID</param>
        /// <returns></returns>
        [Area("Interview")]
        public IActionResult MenuEdit(int id, int pid)
        {
            QZ_Model_In_Menu menu = new QZ_Model_In_Menu();
            if (id > 0)
            {
                menu = _iMenuService.Find<QZ_Model_In_Menu>(id);
            }
            menu = menu ?? new QZ_Model_In_Menu();
            ViewBag.PID = pid;
            if (menu.SortNumber == 0)
            {
                menu.SortNumber = _iMenuService.GetMaxSortNumberByParentID(pid) + 1;
            }
            return View(menu);
        }

        /// <summary>
        /// 编辑保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Area("Interview")]
        public IActionResult EditSaveChange()
        {
            QZ_Model_In_Menu data = null;
            //参数声明[FromBody]无法接受到参数，改用以下方式，原因未知
            Request.EnableBuffering();
            Request.Body.Position = 0;
            StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
            string requstJson = reader.ReadToEndAsync().Result;
            data = JsonConvert.DeserializeObject<QZ_Model_In_Menu>(requstJson);

            lock (_obj)
            {
                if (data == null)
                {
                    return ContentTips(EnumResponseCode.Error, "提交的数据为空");
                }
                if (string.IsNullOrWhiteSpace(data.MenuName))
                {
                    return ContentTips(EnumResponseCode.Error, "请填写菜单名称");
                }
                if (_iMenuService.Any<QZ_Model_In_Menu>(p => p.ParentID == data.ParentID && p.ID != data.ID && p.MenuName == data.MenuName))
                {
                    return ContentTips(EnumResponseCode.Error, "已存在此菜单");
                }
                if (data.ID > 0)
                {
                    QZ_Model_In_Menu oldModel = _iMenuService.Find<QZ_Model_In_Menu>(data.ID);
                    if (oldModel == null)
                    {
                        return ContentTips(EnumResponseCode.Error, "提交的数据有误");
                    }
                    oldModel.MenuName = data.MenuName;
                    oldModel.PathUrl = data.PathUrl;
                    oldModel.Status = data.Status;
                    oldModel.Status = data.Status;
                    oldModel.ParentID = data.ParentID;
                    oldModel.Icon = data.Icon;
                    //排序序号变动，校验新设置的排序序号是否被使用，使用则两者互换
                    if (oldModel.SortNumber != data.SortNumber)
                    {
                        var thisSortMenu = _iMenuService.FirstOrDefault<QZ_Model_In_Menu>(p => p.ParentID == data.ParentID && p.SortNumber == data.SortNumber);
                        if (thisSortMenu != null)
                        {
                            thisSortMenu.SortNumber = oldModel.SortNumber;
                            _iMenuService.Update(thisSortMenu);
                        }
                        oldModel.SortNumber = data.SortNumber;
                    }
                    _iMenuService.Update(oldModel);
                }
                else
                {
                    //新增
                    data.SortNumber = _iMenuService.GetMaxSortNumberByParentID(data.ParentID) + 1;
                    data.CreateTime = DateTime.Now;
                    _iMenuService.Insert(data);
                }
                return ContentTips(EnumResponseCode.Success);
            }
        }
        #endregion
    }
}