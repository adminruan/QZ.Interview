using QZ.Model.Interview;
using System;
using System.Collections.Generic;
using System.Text;

namespace QZ.Interface.Interview_IService
{
    public interface QZ_In_IMenuService : IBaseService
    {
        #region 读取
        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        List<QZ_Model_In_Menu> GetMenu();

        /// <summary>
        /// 获取当前父ID下最大的排序ID
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        int GetMaxSortNumberByParentID(int pid);

        /// <summary>
        /// 通过ID向上递归获取父菜单信息
        /// 包含本机菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<QZ_Model_In_Menu> GetParentsByID(int id);
        #endregion
    }
}
