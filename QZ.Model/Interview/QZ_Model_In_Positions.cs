using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QZ.Model.Interview
{
    /// <summary>
    /// 职务岗位实体
    /// </summary>
    [Table("In_Positions")]
    public class QZ_Model_In_Positions
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 职务名称
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// 状态
        /// true：启用
        /// false：禁用
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 增添时间
        /// </summary>
        public DateTime AddTime { get; set; }
    }
}
