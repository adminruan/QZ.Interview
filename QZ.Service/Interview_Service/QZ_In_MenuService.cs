using Microsoft.EntityFrameworkCore;
using QZ.Interface.Interview_IService;
using QZ.Model.DBContext;
using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QZ.Service.Interview_Service
{
    public class QZ_In_MenuService : BaseService, QZ_In_IMenuService
    {
        private readonly DbSet<QZ_Model_In_Menu> _Menus;
        public QZ_In_MenuService(Interview_DB_EFContext dbContext) : base(dbContext)
        {
            this._Menus = dbContext.Menus;
        }

        #region 读取
        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public List<QZ_Model_In_Menu> GetMenu()
        {
            return _Menus.Where(p => p.Status == 0).ToList();
        }

        /// <summary>
        /// 获取当前父ID下最大的排序ID
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public int GetMaxSortNumberByParentID(int pid)
        {
            try
            {
                return _Menus.Where(p => p.ParentID == pid).Max(p => p.SortNumber);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion
    }
}
