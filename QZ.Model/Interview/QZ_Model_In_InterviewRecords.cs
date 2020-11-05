using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QZ.Model.Interview
{
    /// <summary>
    /// 面试记录实体
    /// </summary>
    [Table("In_InterviewRecords")]
    public class QZ_Model_In_InterviewRecords
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 面试进度
        /// 参照枚举：QZ_Enum_Schedules
        public int Schedule { get; set; }

        /// <summary>
        /// 面试评语
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 面试提交时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 面试结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 面试管理员ID
        /// 多个用‘|’隔开
        /// </summary>
        public string InterviewerAdminIds { get; set; }
        
        /// <summary>
        /// 转正薪资
        /// </summary>
        public decimal? RealOffer { get; set; }

        /// <summary>
        /// 试用薪资
        /// </summary>
        public decimal? TryOffer { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime? EntryTime { get; set; }

        /// <summary>
        /// 应聘岗位
        /// </summary>
        public int ApplyJob { get; set; }
    }
}
