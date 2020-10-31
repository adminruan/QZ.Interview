using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QZ.Model.Interview
{
    /// <summary>
    /// 菜单实体
    /// </summary>
    [Table("In_Menu")]
    public class QZ_Model_In_Menu
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 页面路径
        /// </summary>
        public string PathUrl { get; set; }

        /// <summary>
        /// 菜单排序
        /// </summary>
        public int SortNumber { get; set; }

        /// <summary>
        /// 状态
        /// 0：可用、
        /// 1：禁用
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }
    }
}
