using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QZ.Common;
using QZ.Interface.Interview_IService;
using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QZ.InterviewAdmin.Components
{
    [ViewComponent(Name = "Navigation")]
    public class NavigationViewComponent : ViewComponent
    {
        private readonly QZ_In_IMenuService _iMenuService;
        public NavigationViewComponent(QZ_In_IMenuService iMenuService)
        {
            this._iMenuService = iMenuService;
        }

        public IViewComponentResult Invoke()
        {
            string stradmin = QZ_Helper_CookieHelper.ReadCookie(this.HttpContext, "CurrentUser");
            QZ_Model_In_AdminInfo Admin = null;
            if (!string.IsNullOrEmpty(stradmin))
            {
                Admin = JsonConvert.DeserializeObject<QZ_Model_In_AdminInfo>(stradmin);
            }
            List<QZ_Model_In_Menu> menus = new List<QZ_Model_In_Menu>();
            if (Admin != null)
            {
                menus = _iMenuService.GetMenu();
                if (menus == null || menus.Count == 0)
                {
                    try
                    {
                        menus = new List<QZ_Model_In_Menu>();
                        //初始化菜单配置
                        QZ_Model_In_Menu init = new QZ_Model_In_Menu();
                        init.MenuName = "系统设置";
                        init.SortNumber = 1;
                        init.Status = 0;
                        init.CreateTime = DateTime.Now;
                        init.ParentID = 0;
                        init.Icon = "layui-icon layui-icon-set-fill";
                        menus.Add(_iMenuService.Insert(init));
                        QZ_Model_In_Menu menu = new QZ_Model_In_Menu();
                        menu.MenuName = "菜单配置";
                        menu.PathUrl = "/Interview/Menus/MenuInfos";
                        menu.SortNumber = 1;
                        menu.Status = 0;
                        menu.CreateTime = DateTime.Now;
                        menu.ParentID = init.ID;
                        menu.Icon = "layui-icon layui-icon-menu-fill";
                        menus.Add(_iMenuService.Insert(menu));
                    }
                    catch (Exception)
                    {
                        //初始化失败
                    }
                }
            }
            return View(menus);
        }
    }
}
